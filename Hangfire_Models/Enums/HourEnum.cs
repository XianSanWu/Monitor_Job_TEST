
using System.ComponentModel;

namespace Hangfire_Models.Enums
{
    /// <summary>
    /// 24小時
    /// </summary>
    public enum HourEnum
    {
        [Description("None")] None = 555,
        [Description("00時")] H_00 = 0,
        [Description("01時")] H_01 = 1,
        [Description("02時")] H_02 = 2,
        [Description("03時")] H_03 = 3,
        [Description("04時")] H_04 = 4,
        [Description("05時")] H_05 = 5,
        [Description("06時")] H_06 = 6,
        [Description("07時")] H_07 = 7,
        [Description("08時")] H_08 = 8,
        [Description("09時")] H_09 = 9,
        [Description("10時")] H_10 = 10,
        [Description("11時")] H_11 = 11,
        [Description("12時")] H_12 = 12,
        [Description("13時")] H_13 = 13,
        [Description("14時")] H_14 = 14,
        [Description("15時")] H_15 = 15,
        [Description("16時")] H_16 = 16,
        [Description("17時")] H_17 = 17,
        [Description("18時")] H_18 = 18,
        [Description("19時")] H_19 = 19,
        [Description("20時")] H_20 = 20,
        [Description("21時")] H_21 = 21,
        [Description("22時")] H_22 = 22,
        [Description("23時")] H_23 = 23,
    }

}
