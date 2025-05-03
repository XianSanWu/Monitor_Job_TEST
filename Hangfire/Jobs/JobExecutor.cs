
using Hangfire.Server;
using Hangfire_Models.Dto.Requests;
using Hangfire_Servies.Interfaces;

namespace Hangfire.Jobs
{
    public class JobExecutor(ICommandDispatcher commandDispatcher, ILogger<JobExecutor> logger)
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly ILogger<JobExecutor> _logger = logger;

        public async Task Execute(JobExecutionContext jobExecutionContext)
        {
            _logger.LogInformation("開始執行 Job：{JobKey}，JobId：{JobId}", jobExecutionContext.JobKey, jobExecutionContext.JobId);
          
            await _commandDispatcher.DispatchAsync(jobExecutionContext);
        }
    }


}
