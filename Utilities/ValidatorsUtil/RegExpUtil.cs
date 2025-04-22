using System.Text.RegularExpressions;

namespace Utilities.ValidatorsUtil
{
    public static class RegExpUtil
    {
        /// <summary> 客戶 ID 搜尋 (最多 2 個大寫字母 + 最多 9 個數字) </summary>
        public static readonly Regex CustIdSearch = new(@"^[A-Z]{0,2}[0-9]{0,9}$", RegexOptions.Compiled);

        /// <summary> 整數 (正數) </summary>
        public static readonly Regex Int = new(@"^[0-9]*$", RegexOptions.Compiled);

        /// <summary> 正負整數 </summary>
        public static readonly Regex IsNumeric = new(@"^-?\d+$", RegexOptions.Compiled);

        /// <summary> yyyy-MM-dd 格式日期 </summary>
        public static readonly Regex DateDashAD = new(@"^\d{4}-\d{2}-\d{2}$", RegexOptions.Compiled);

        /// <summary> 四位數年份 yyyy </summary>
        public static readonly Regex YYYY = new(@"^\d{4}$", RegexOptions.Compiled);

        /// <summary> yyyy-MM 年份 + 月份 </summary>
        public static readonly Regex YYYYMM = new(@"^\d{4}-\d{2}$", RegexOptions.Compiled);

        /// <summary> yyyy-MM-dd 或 yyyy/MM/dd 或 yyyy.MM.dd </summary>
        public static readonly Regex DateFmt1 = new(@"^\d{4}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$", RegexOptions.Compiled);

        /// <summary> yyyyMMdd 純數字日期 </summary>
        public static readonly Regex DateFmt2 = new(@"^\d{4}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$", RegexOptions.Compiled);

        /// <summary> MM/dd/yyyy </summary>
        public static readonly Regex DateFmt3 = new(@"^(0[1-9]|1[0-2])\/(0[1-9]|[12][0-9]|3[01])\/(19|20)\d\d$", RegexOptions.Compiled);

        /// <summary> 中文字符檢查 </summary>
        public static readonly Regex Chinese = new(@"[\u4e00-\u9fa5]", RegexOptions.Compiled);

        /// <summary> 空白與換行符號 </summary>
        public static readonly Regex WhiteSpaceAndNewLines = new(@"\s", RegexOptions.Compiled);

        /// <summary> 中文、英文、數字、底線、連字符 </summary>
        public static readonly Regex RegexChineseEnglishNumbers = new(@"^[\u4e00-\u9fa5a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        /// <summary> 英文、數字、底線、連字符 </summary>
        public static readonly Regex RegexEnglishNumbers = new(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        /// <summary> 英文、數字、特殊符號 </summary>
        public static readonly Regex RegexSymbolsEnglishNumbers = new(@"^[a-zA-Z0-9_!\-@#&*()+=<>?^,.:;]+$", RegexOptions.Compiled);

        /// <summary>> 驗證輸入是否符合指定的正則表達式 </summary>
        public static bool IsMatch(Regex regex, string? input = "")
        {
            return !string.IsNullOrEmpty(input) && regex.IsMatch(input);
        }
    }
}
