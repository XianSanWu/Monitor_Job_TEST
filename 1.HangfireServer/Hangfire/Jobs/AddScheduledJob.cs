using Hangfire.Dashboard.Management.v2.Metadata;
using Hangfire.Server;
using System.ComponentModel;
using Hangfire.Dashboard.Management.v2.Support;
using Hangfire_Models.Enums;
using Hangfire_Models.Dto.Requests;

namespace Hangfire.Jobs
{
    [ManagementPage(MenuName = nameof(AddScheduledJob), Title = nameof(AddScheduledJob))]
    public class AddScheduledJob : IJob
    {
        [DisplayName("新增排程任務")]
        [Description("請輸入排程設定，包括 CRON 表達式、描述及開始時間。")]
        public void CreateSchedule(PerformContext context,

            [DisplayData(Label = "任務名稱", Placeholder = "請輸入任務名稱", Description = "此名稱將用於識別該排程任務。", IsRequired = true)]
            string jobName,

            [DisplayData(Label = "任務類型", Description = "請選擇此排程任務要執行的作業", DefaultValue = ScheduleTypeEnum.UpdateWorkflowStatusMailhunterJob, IsRequired = true)]
            ScheduleTypeEnum scheduleType,

            //[DisplayData(Label = "CRON 表達式", Placeholder = "例如 */5 * * * *", Description = "請輸入 CRON 格式的排程時間。")]
            //string cron
            [DisplayData(Label = "週期", Description = "請選擇排程週期", DefaultValue = ScheduleFrequencyEnum.Daily, IsRequired = true)]
            ScheduleFrequencyEnum frequency,

            [DisplayData(Label = "星期幾", Description = "*****僅當週期為每週時使用*****", DefaultValue = ChineseDayOfWeekEnum.None)]
            ChineseDayOfWeekEnum dayOfWeek,

            [DisplayData(Label = "幾號", Description = "*****僅當週期為每月時使用*****", DefaultValue = ChineseDayOfMonthEnum.None)]
            ChineseDayOfMonthEnum dayOfMonth,

            [DisplayData(Label = "小時", Description = "請選擇 0 到 23 小時", DefaultValue = HourEnum.None, IsRequired = true)]
            HourEnum hour,

            [DisplayData(Label = "分鐘", Description = "請選擇 0 到 59 分鐘", DefaultValue = MinuteEnum.None, IsRequired = true)]
            MinuteEnum minute
        )
        {
            var jobKey = $"{jobName}";//_{selectMethod}

            var selectMethod = scheduleType switch
            {
                ScheduleTypeEnum.UpdateWorkflowStatusMailhunterJob => nameof(ScheduleTypeEnum.UpdateWorkflowStatusMailhunterJob),
                ScheduleTypeEnum.UpdateWorkflowStatusTodayFinishJob => nameof(ScheduleTypeEnum.UpdateWorkflowStatusTodayFinishJob),
                _ => ""
            };

            var jobExecutionContext = new JobExecutionContext
            {
                JobKey = jobKey,
                JobId = context.BackgroundJob.Id,
                SelectMethod = selectMethod,
            };


            // 根據選擇的週期來產生 Cron 表達式
            string cron = frequency switch
            {
                // 每分鐘
                ScheduleFrequencyEnum.Minute =>
                    $"{(minute == MinuteEnum.None ? "*" : (int)minute)} * * * *",  // 每X分鐘執行修復

                // 每小時
                ScheduleFrequencyEnum.Hourly =>
                    $"{(minute == MinuteEnum.None ? "*" : (int)minute)} {(hour == HourEnum.None ? "*" : "*/" + (int)hour)} * * *",  // 每 N 小時

                // 每天
                ScheduleFrequencyEnum.Daily =>
                    $"{(minute == MinuteEnum.None ? "*" : (int)minute)} {(hour == HourEnum.None ? "*" : (int)hour)} * * *",

                // 每週
                ScheduleFrequencyEnum.Weekly when dayOfWeek != ChineseDayOfWeekEnum.None =>
                    $"{(minute == MinuteEnum.None ? "*" : (int)minute)} {(hour == HourEnum.None ? "*" : (int)hour)} * * {(int)dayOfWeek}",

                // 每月
                ScheduleFrequencyEnum.Monthly when dayOfMonth != ChineseDayOfMonthEnum.None && dayOfMonth != ChineseDayOfMonthEnum.EndOfMonth =>
                    $"{(minute == MinuteEnum.None ? "*" : (int)minute)} {(hour == HourEnum.None ? "*" : (int)hour)} {(int)dayOfMonth} * *",

                // 每月的最後一天
                ScheduleFrequencyEnum.Monthly when dayOfMonth == ChineseDayOfMonthEnum.EndOfMonth =>
                    $"{(minute == MinuteEnum.None ? "*" : (int)minute)} {(hour == HourEnum.None ? "*" : (int)hour)} L * *",  // "L" 代表每月最後一天

                _ => throw new ArgumentException("週期設定錯誤：請確認週期設定是否正確")
            };

            // 在定期工作中設定
            RecurringJob.AddOrUpdate<JobExecutor>(
                recurringJobId: jobKey,
                methodCall: executor => executor.Execute(jobExecutionContext),
                cronExpression: cron,
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                }
            );
        }
    }
}
