
using System.ComponentModel;

namespace Hangfire_Models.Enums
{
    /// <summary>
    /// 週期
    /// </summary>
    public enum ScheduleFrequencyEnum
    {
        [Description("每分鐘")]
        Minute,

        [Description("每小時")]
        Hourly,

        [Description("每日")]
        Daily,

        [Description("每週")]
        Weekly,

        [Description("每月")]
        Monthly
    }
}
