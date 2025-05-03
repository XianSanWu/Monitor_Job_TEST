using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Day1 = 1,

        [Description("2號")]
        Day2 = 2,

        [Description("3號")]
        Day3 = 3,

        [Description("4號")]
        Day4 = 4,

        [Description("5號")]
        Day5 = 5,

        [Description("6號")]
        Day6 = 6,

        [Description("7號")]
        Day7 = 7,

        [Description("8號")]
        Day8 = 8,

        [Description("9號")]
        Day9 = 9,

        [Description("10號")]
        Day10 = 10,

        [Description("11號")]
        Day11 = 11,

        [Description("12號")]
        Day12 = 12,

        [Description("13號")]
        Day13 = 13,

        [Description("14號")]
        Day14 = 14,

        [Description("15號")]
        Day15 = 15,

        [Description("16號")]
        Day16 = 16,

        [Description("17號")]
        Day17 = 17,

        [Description("18號")]
        Day18 = 18,

        [Description("19號")]
        Day19 = 19,

        [Description("20號")]
        Day20 = 20,

        [Description("21號")]
        Day21 = 21,

        [Description("22號")]
        Day22 = 22,

        [Description("23號")]
        Day23 = 23,

        [Description("24號")]
        Day24 = 24,

        [Description("25號")]
        Day25 = 25,

        [Description("26號")]
        Day26 = 26,

        [Description("27號")]
        Day27 = 27,

        [Description("28號")]
        Day28 = 28,

        [Description("月底")]
        EndOfMonth = 999
    }
}
