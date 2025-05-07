using Hangfire.Common;
using Hangfire.Server;

namespace Hangfire.Filters
{
    public static class JobExecutionContextAccessor
    {
        private static readonly AsyncLocal<string?> _currentJobId = new();

        public static string? CurrentJobId
        {
            get => _currentJobId.Value;
            set => _currentJobId.Value = value;
        }
    }

    public class JobTrackingFilter : JobFilterAttribute, IServerFilter
    {
        public void OnPerforming(PerformingContext context)
        {
            var jobId = context.BackgroundJob?.Id;
            JobExecutionContextAccessor.CurrentJobId = jobId;
        }

        public void OnPerformed(PerformedContext context)
        {
            var jobId = JobExecutionContextAccessor.CurrentJobId;

            // 清除，避免 thread static 汙染
            JobExecutionContextAccessor.CurrentJobId = null;
        }
    }

}
