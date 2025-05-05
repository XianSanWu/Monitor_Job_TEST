
using Hangfire_Models.Dto.Requests;
using Hangfire_Servies.Interfaces;

namespace Hangfire.Jobs
{
    public class JobExecutor(ICommandDispatcher commandDispatcher, ILogger<JobExecutor> logger)
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly ILogger<JobExecutor> _logger = logger;

        [DisableConcurrentExecution(timeoutInSeconds: 3600)]
        [AutomaticRetry(Attempts = 0)]
        public async Task Execute(JobExecutionContext jobExecutionContext)
        {
            _logger.LogInformation("開始執行 Job：{JobKey}，JobId：{JobId}", jobExecutionContext.JobKey, jobExecutionContext.JobId);

            try
            {
                await _commandDispatcher.DispatchAsync(jobExecutionContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "執行 Job 發生錯誤，JobKey：{JobKey}", jobExecutionContext.JobKey);
                throw new Exception($"執行 Job 發生錯誤，JobKey：{jobExecutionContext.JobKey}，JobId：{jobExecutionContext.JobId}，EX：{ex}，EX_MSG：{ex.Message}");
            }
        }
    }

}
