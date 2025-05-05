
using Hangfire.Filters;
using Hangfire_Models.Dto.Requests;
using Hangfire_Servies.Interfaces;
using Serilog.Context;

namespace Hangfire.Jobs
{
    public class JobExecutor(ICommandDispatcher commandDispatcher, ILogger<JobExecutor> logger)
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly ILogger<JobExecutor> _logger = logger;

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("{0}")]
        [JobTrackingFilter]
        public async Task Execute(JobExecutionContext jobExecutionContext)
        {
            var currentExecutionId = JobExecutionContextAccessor.CurrentJobId;
            try
            {
                _logger.LogInformation("開始執行 Job：{JobKey}，JobId：{JobId}，CurrentExecutionId：{CurrentExecutionId}", jobExecutionContext.JobKey, jobExecutionContext.JobId, currentExecutionId);
                jobExecutionContext.CurrentExecutionId = currentExecutionId;

                await _commandDispatcher.DispatchAsync(jobExecutionContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "執行 Job 發生錯誤，JobKey：{JobKey}", jobExecutionContext.JobKey);
                throw new Exception($"執行 Job 發生錯誤，JobKey：{jobExecutionContext.JobKey}，JobId：{jobExecutionContext.JobId}，CurrentExecutionId：{currentExecutionId}，EX：{ex}，EX_MSG：{ex.Message}");
            }
        }
    }

}
