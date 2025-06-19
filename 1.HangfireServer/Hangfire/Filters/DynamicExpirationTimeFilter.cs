using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace Hangfire.Filters
{

    public class DynamicExpirationTimeFilter : JobFilterAttribute, IApplyStateFilter
    {
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context.NewState is SucceededState)
            {
                // 取得 Job 類型名稱
                var jobTypeName = context.BackgroundJob.Job?.Type?.Name ?? string.Empty;

                // 預設保留 30 天
                var expiration = TimeSpan.FromDays(30);

                // 根據不同 Job 決定 Expiration
                //if (jobTypeName.Contains("UpdateWorkflowStatusMailhunterJob"))
                //{
                //    expiration = TimeSpan.FromDays(7);
                //}
                //else if (jobTypeName.Contains("UpdateWorkflowStatusTodayFinishJob"))
                //{
                //    expiration = TimeSpan.FromDays(14);
                //}

                transaction.ExpireJob(context.BackgroundJob.Id, expiration);
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            // 無需處理
        }
    }

}
