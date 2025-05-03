
using System.ComponentModel;

namespace Hangfire_Models.Enums
{
    /// <summary>
    /// 24小時
    /// </summary>
    public enum HourEnum
    {
        [Description("None")] None = 555,
        [Description("00時")] H00 = 0,
        [Description("01時")] H01 = 1,
        [Description("02時")] H02 = 2,
        [Description("03時")] H03 = 3,
        [Description("04時")] H04 = 4,
        [Description("05時")] H05 = 5,
        [Description("06時")] H06 = 6,
        [Description("07時")] H07 = 7,
        [Description("08時")] H08 = 8,
        [Description("09時")] H09 = 9,
        [Description("10時")] H10 = 10,
        [Description("11時")] H11 = 11,
        [Description("12時")] H12 = 12,
        [Description("13時")] H13 = 13,
        [Description("14時")] H14 = 14,
        [Description("15時")] H15 = 15,
        [Description("16時")] H16 = 16,
        [Description("17時")] H17 = 17,
        [Description("18時")] H18 = 18,
        [Description("19時")] H19 = 19,
        [Description("20時")] H20 = 20,
        [Description("21時")] H21 = 21,
        [Description("22時")] H22 = 22,
        [Description("23時")] H23 = 23,
    }

}
