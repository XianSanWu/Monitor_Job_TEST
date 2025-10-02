using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using System.Data;
using System.Data.Common;
using Utilities.Utilities;
using Models.Enums;

namespace Repository.Implementations
{
    /// <summary>
    /// DB連線的共用類
    /// </summary>
    public sealed class DbHelper : IDbHelper, IDisposable, IAsyncDisposable
    {
        private readonly DbConnection _conn;
        private readonly UnitOfWork _unitOfWork;

        public DbHelper(IConfiguration config, DBConnectionEnum connectionType = DBConnectionEnum.DefaultConnection)
        {
            string connStr = connectionType switch
            {
                DBConnectionEnum.Cdp => config.GetConnectionString(DBConnectionEnum.Cdp.ToString())!,
                DBConnectionEnum.Mail_hunter => config.GetConnectionString(DBConnectionEnum.Mail_hunter.ToString())!,
                _ => config.GetConnectionString(DBConnectionEnum.DefaultConnection.ToString())!
            };

            // 解密連線字串
            var key = config["EncryptionSettings:AESKey"]!;
            var iv = config["EncryptionSettings:AESIV"]!;
            var decryptedConnStr = CryptoUtil.Decrypt(Base64Util.Decode(connStr), key, iv);

            var builder = new SqlConnectionStringBuilder(decryptedConnStr)
            {
                Pooling = true,
                PoolBlockingPeriod = PoolBlockingPeriod.NeverBlock,
                ConnectRetryCount = 3,
                ConnectRetryInterval = 10,
                ConnectTimeout = 300
            };

            _conn = new SqlConnection(builder.ConnectionString);
            _unitOfWork = new UnitOfWork(_conn);
        }

        public UnitOfWork UnitOfWork => _unitOfWork;
        public DbConnection GetDbConnection() => _conn;

        public void Dispose()
        {
            _unitOfWork.Dispose();
            if (_conn.State != ConnectionState.Closed)
                _conn.Close();
            _conn.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await _unitOfWork.DisposeAsync();
            if (_conn.State != ConnectionState.Closed)
                await _conn.CloseAsync();
            await _conn.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }

    public class UnitOfWork(DbConnection? connection = null) : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly DbConnection? _connection = connection;
        private DbTransaction? _transaction;
        private readonly Guid _id = Guid.NewGuid();

        public DbConnection Connection => _connection ?? throw new InvalidOperationException("Connection is null.");
        public DbTransaction? Transaction => _transaction;
        Guid IUnitOfWork.Id => _id;

        public void Begin()
        {
            if (_connection == null)
                throw new InvalidOperationException("Cannot begin transaction: Connection is null.");

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            _transaction = _connection.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction is null.");

            await _transaction.CommitAsync().ConfigureAwait(false);
            await CleanupAsync().ConfigureAwait(false);
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction is null.");

            await _transaction.RollbackAsync().ConfigureAwait(false);
            await CleanupAsync().ConfigureAwait(false);
        }

        private async Task CleanupAsync()
        {
            _transaction?.Dispose();
            _transaction = null;

            if (_connection == null)
                throw new InvalidOperationException("Cannot begin transaction: Connection is null.");

            if (_connection.State == ConnectionState.Open)
                await _connection.CloseAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            _transaction?.Dispose();
            _transaction = null;

            if (_connection == null)
                throw new InvalidOperationException("Cannot begin transaction: Connection is null.");

            if (_connection.State == ConnectionState.Open)
                await _connection.CloseAsync();

            await _connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        #region BulkCopy
        public async Task ExecuteBulkCopyAsync(DataTable table, string tableName, CancellationToken cancellationToken = default)
        {
            if (_connection is not SqlConnection sqlConn)
                throw new InvalidOperationException("連線必須是 SqlConnection.");

            if (_transaction is not SqlTransaction sqlTran)
                throw new InvalidOperationException("交易必須是 SqlTransaction.");

            using var bulkCopy = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, sqlTran)
            {
                DestinationTableName = tableName,
                BulkCopyTimeout = 1800
            };

            foreach (DataColumn col in table.Columns)
                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

            if (sqlConn.State != ConnectionState.Open)
                await sqlConn.OpenAsync(cancellationToken);

            await bulkCopy.WriteToServerAsync(table, cancellationToken);
        }

        public async Task ExecuteBulkCopyAsync(IDataReader reader, string tableName, CancellationToken cancellationToken = default)
        {
            if (_connection is not SqlConnection sqlConn)
                throw new InvalidOperationException("連線必須是 SqlConnection.");

            if (_transaction is not SqlTransaction sqlTran)
                throw new InvalidOperationException("交易必須是 SqlTransaction.");

            using var bulkCopy = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default, sqlTran)
            {
                DestinationTableName = tableName,
                BulkCopyTimeout = 1800,
                BatchSize = 5000
            };

            for (int i = 0; i < reader.FieldCount; i++)
                bulkCopy.ColumnMappings.Add(reader.GetName(i), reader.GetName(i));

            if (sqlConn.State != ConnectionState.Open)
                await sqlConn.OpenAsync(cancellationToken);

            await bulkCopy.WriteToServerAsync(reader, cancellationToken);
        }
        #endregion
    }
}
