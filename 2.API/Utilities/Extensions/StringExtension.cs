using System.ComponentModel;
using System.Reflection;

namespace Utilities.Extensions
{
    public static class StringExtension
    {
        ///// <summary>
        ///// String to Enum
        ///// </summary>
        ///// <typeparam name="TTarget"></typeparam>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //public static TTarget ConvertToEnum<TTarget>(this string str)
        //{
        //    #region 參數宣告

        //    #endregion

        //    #region 流程 
        //    return (TTarget)Enum.Parse(typeof(TTarget), str);
        //    #endregion
        //}

        /// <summary>
        /// String to Enum
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TTarget ConvertStringToEnum<TTarget>(this string str)
        {
            #region 參數宣告

            #endregion

            #region 流程 
            return (TTarget)Enum.Parse(typeof(TTarget), str);
            #endregion
        }

        /// <summary>
        /// Int to Enum
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TTarget ConvertIntToEnum<TTarget>(this int str)
        {
            #region 參數宣告

            #endregion

            #region 流程 
            return (TTarget)Enum.ToObject(typeof(TTarget), str);
            #endregion
        }

        ///  <summary> 
        ///  擴充方法，取得枚舉的Description
        ///  </summary> 
        ///  <param name="value">枚舉值</param> 
        ///  <param name=" nameInstead">當枚舉值沒有定義DescriptionAttribute，是否使用枚舉名代替，預設是使用</param> 
        ///  <returns>枚舉的Description </returns> 
        public static string? GetDescription(this Enum value, bool nameInstead = true)
        {
            if (value == null)
            {
                return null;
            }

            Type type = value.GetType();
            string? name = Enum.GetName(type, value);

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            FieldInfo? field = type.GetField(name);
            if (field == null)
            {
                return nameInstead ? name : null;
            }

            DescriptionAttribute? attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? (nameInstead ? name : null);
        }

    }

    public static class IEnumerableExtension
    {
        /// <summary>
        /// 判斷 List<String>是否為null
        /// 也可用  if (!searchInfo.EmployeeNoList?.Any() ?? false) 判斷
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> IsNull<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                yield break;
            }

            foreach (var item in source)
            {
                yield return item;
            }
        }
    }


}