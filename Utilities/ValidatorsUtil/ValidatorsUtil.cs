namespace Utilities.ValidatorsUtil
{
    public class ValidatorsUtil
    {
        /// <summary>
        /// 判斷輸入值是否存在Enum中
        /// </summary>
        /// <typeparam name="TEnum">任一Enum</typeparam>
        /// <param name="value">Enum值</param>
        /// <returns></returns>
        public static bool CheckEnumExists<TEnum>(string value) where TEnum : struct, Enum
        {
            return Enum.TryParse(value, out TEnum result) && Enum.IsDefined(typeof(TEnum), result);
        }


        /// <summary>
        /// 要數字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NumericOnly(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return double.TryParse(value, out _);
        }

        /// <summary>
        /// 不允許數字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NonNumeric(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true; // 如果是空白，視為通過驗證
            }

            return !double.TryParse(value, out _);
        }
    }
}
