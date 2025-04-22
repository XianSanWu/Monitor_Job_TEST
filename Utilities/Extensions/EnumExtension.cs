using System.ComponentModel;
using System.Reflection;

namespace Utilities.Extensions
{
    /// <summary>
    /// 可列舉型別擴充模組
    /// </summary>
    public static class EnumExtension
    {
        #region Methods
        /// <summary>
        /// 取得Enum的敘述
        /// </summary>
        /// <param name="pEnum">Enum物件</param>
        /// <returns>Enum的敘述或名稱</returns>
        public static string GetEnumDesc(this Enum pEnum)
        {
            string result = string.Empty;
            FieldInfo? EnumInfo = pEnum.GetType().GetField(pEnum.ToString());
            if (EnumInfo != null)
            {
                DescriptionAttribute[]? EnumAttributes = (DescriptionAttribute[])EnumInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((EnumAttributes.Length > 0))
                {
                    result = EnumAttributes[0].Description;
                }
                else
                {
                    result = pEnum.ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// 比對字串是否等於Enum Description
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="targetEnum"></param>
        /// <returns></returns>
        public static bool EqualDescription(string sourceString, Enum targetEnum)
        {
            #region 流程
            try
            {
                return targetEnum.GetEnumDesc().Equals(sourceString);
            }
            catch (Exception)
            {
                return false;
            }

            #endregion
        }

        #endregion
    }

}