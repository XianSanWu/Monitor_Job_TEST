using System.ComponentModel;

namespace Hangfire_Models.Enums
{
    /// <summary>
    ///  星期幾
    /// </summary>
    public enum ChineseDayOfWeekEnum
    {
        [Description("空")]
        None = 555,

        [Description("星期日")]
        Sunday = 0,

        [Description("星期一")]
        Monday = 1,

        [Description("星期二")]
        Tuesday = 2,

        [Description("星期三")]
        Wednesday = 3,

        [Description("星期四")]
        Thursday = 4,

        [Description("星期五")]
        Friday = 5,

        [Description("星期六")]
        Saturday = 6,

    }
}
