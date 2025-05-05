using System.ComponentModel;

namespace Hangfire_Models.Enums
{
    /// <summary>
    ///  每月幾號（1-28號或月底）
    /// </summary>
    public enum ChineseDayOfMonthEnum
    {
        [Description("空")]
        None = 555,

        [Description("1號")]
        Day_1 = 1,

        [Description("2號")]
        Day_2 = 2,

        [Description("3號")]
        Day_3 = 3,

        [Description("4號")]
        Day_4 = 4,

        [Description("5號")]
        Day_5 = 5,

        [Description("6號")]
        Day_6 = 6,

        [Description("7號")]
        Day_7 = 7,

        [Description("8號")]
        Day_8 = 8,

        [Description("9號")]
        Day_9 = 9,

        [Description("10號")]
        Day_10 = 10,

        [Description("11號")]
        Day_11 = 11,

        [Description("12號")]
        Day_12 = 12,

        [Description("13號")]
        Day_13 = 13,

        [Description("14號")]
        Day_14 = 14,

        [Description("15號")]
        Day_15 = 15,

        [Description("16號")]
        Day_16 = 16,

        [Description("17號")]
        Day_17 = 17,

        [Description("18號")]
        Day_18 = 18,

        [Description("19號")]
        Day_19 = 19,

        [Description("20號")]
        Day_20 = 20,

        [Description("21號")]
        Day_21 = 21,

        [Description("22號")]
        Day_22 = 22,

        [Description("23號")]
        Day_23 = 23,

        [Description("24號")]
        Day_24 = 24,

        [Description("25號")]
        Day_25 = 25,

        [Description("26號")]
        Day_26 = 26,

        [Description("27號")]
        Day_27 = 27,

        [Description("28號")]
        Day_28 = 28,

        [Description("月底")]
        EndOfMonth = 999
    }
}
