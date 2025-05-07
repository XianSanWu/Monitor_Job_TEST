using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using System.Data.Common;
using Utilities.Utilities;
using Models.Enums;

namespace Repository.Implementations
{
    /// <summary>
    /// DB連線的共用類
    /// </summary>
    public sealed class DbHelper : IDbHelper
    {
        private readonly string? _dbConnString;
        private readonly DbConnection _conn;
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// 選擇 appsettings.json 中 不同的DB連線
        /// </summary>
        /// <param name="ConnectionType"></param>
        public DbHelper(IConfiguration _config, DBConnectionEnum ConnectionType = DBConnectionEnum.DefaultConnection)
        {
            switch (ConnectionType)
            {
                case DBConnectionEnum.Cdp:
                    {
                        _dbConnString = _config.GetConnectionString(DBConnectionEnum.Cdp.ToString());
                    }
                    break;
                case DBConnectionEnum.Mail_hunter:
                    {
                        _dbConnString = _config.GetConnectionString(DBConnectionEnum.Mail_hunter.ToString());
                    }
                    break;
                default:
                    {
                        _dbConnString = _config.GetConnectionString(DBConnectionEnum.DefaultConnection.ToString());
                    }
                    break;
            }

            #region 建立和管理 SqlConnection 類別使用之連接字串的內容      
            var key = _config["EncryptionSettings:AESKey"]!;
            var iv = _config["EncryptionSettings:AESIV"]!;
            var dbConnString = CryptoUtil.Decrypt(Base64Util.Decode((_dbConnString ?? string.Empty)), key, iv);
            SqlConnectionStringBuilder SqlConnBuilder = new(dbConnString)
            {
                Pooling = true,
                PoolBlockingPeriod = PoolBlockingPeriod.NeverBlock,
                ConnectRetryCount = 3,
                ConnectRetryInterval = 10,  //Seconds
                ConnectTimeout = 300
            };
            #endregion

            _conn = new SqlConnection(SqlConnBuilder.ConnectionString);  // 使用 Microsoft.Data.SqlClient.SqlConnection
            _unitOfWork = new UnitOfWork(_conn);
        }

        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public DbConnection GetDbConnection()
        {
            return _conn;
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _conn?.Dispose();
        }
    }

    public class UnitOfWork(DbConnection? connection = null) : IUnitOfWork
    {
        private readonly Guid _id = Guid.NewGuid();
        private DbTransaction? _transaction;
        public DbConnection Connection => connection ?? throw new InvalidOperationException("Connection is null.");
        public DbTransaction? Transaction => _transaction;

        Guid IUnitOfWork.Id
        {
            get { return _id; }
        }

        public void Begin()
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Cannot begin transaction: Connection is null.");
            }

            connection?.Open();
            _transaction = connection?.BeginTransaction();
        }
        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction is null.");
            }
            await _transaction.CommitAsync().ConfigureAwait(false);
        }
        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction is null.");
            }
            await _transaction.RollbackAsync().ConfigureAwait(false);
        }
        public void Dispose()
        {
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
