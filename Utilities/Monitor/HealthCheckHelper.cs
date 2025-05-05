using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.Common;

namespace Utilities.Monitor
{
    public class HealthCheckHelper
    {
        /*
         * https://learn.microsoft.com/zh-tw/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
         * https://learn.microsoft.com/zh-tw/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health?source=recommendations
         *      
         */

        /// <summary>
        /// 參考範本    
        /// </summary>
        public class SampleHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                var isHealthy = true;

                // ...

                if (isHealthy)
                {
                    return Task.FromResult(
                        HealthCheckResult.Healthy("基本範本"));
                }

                return Task.FromResult(
                    new HealthCheckResult(
                        context.Registration.FailureStatus, "API有問題。"));
            }
        }

        /// <summary>
        /// 有帶參數的範本
        /// </summary>
        public class SampleHealthCheckWithArgs : IHealthCheck
        {
            private readonly int _arg1;
            private readonly string _arg2;

            public SampleHealthCheckWithArgs(int arg1, string arg2)
                => (_arg1, _arg2) = (arg1, arg2);

            public Task<HealthCheckResult> CheckHealthAsync(
                HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                var isHealthy = true;

                // ...

                if (isHealthy)
                {
                    return Task.FromResult(
                        HealthCheckResult.Healthy("帶參數的範本"));
                }

                return Task.FromResult(
                   new HealthCheckResult(
                       context.Registration.FailureStatus, "API有問題。"));
            }
        }

        /// <summary>
        /// 檢查 MS SQL 是否可連線    
        /// </summary>
        // Sample SQL Connection Health Check
        public class SqlConnectionHealthCheck : IHealthCheck
        {
            private const string DefaultTestQuery = "Select 1";

            public string ConnectionString { get; }

            public string TestQuery { get; }

            public SqlConnectionHealthCheck(string connectionString) : this(connectionString, testQuery: DefaultTestQuery)
            {
            }

            public SqlConnectionHealthCheck(string connectionString, string testQuery)
            {
                ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
                TestQuery = testQuery;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        await connection.OpenAsync(cancellationToken);

                        if (TestQuery != null)
                        {
                            var command = connection.CreateCommand();
                            command.CommandText = TestQuery;

                            await command.ExecuteNonQueryAsync(cancellationToken);
                        }
                    }
                    catch (DbException ex)
                    {
                        return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
                    }
                }

                return HealthCheckResult.Healthy("資料庫連線成功");
                //return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
            }
        }

        //public class SampleHangfireHealthCheck : IHealthCheck
        //{
        //    private readonly IBackgroundJobClient _backgroundJobClient;

        //    public SampleHangfireHealthCheck(IBackgroundJobClient backgroundJobClient)
        //    {
        //        _backgroundJobClient = backgroundJobClient;
        //    }

        //    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        //    {
        //        try
        //        {
        //            // 嘗試將一個背景工作排進 Hangfire
        //            _backgroundJobClient.Enqueue(() => Console.WriteLine("Hangfire HealthCheck"));

        //            return Task.FromResult(HealthCheckResult.Healthy("Hangfire is working properly."));
        //        }
        //        catch (Exception ex)
        //        {
        //            return Task.FromResult(HealthCheckResult.Unhealthy("Hangfire is not responding.", ex));
        //        }
        //    }
        //}

    }
}
