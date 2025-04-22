using System.Reflection;

namespace Utilities.Utilities
{
    public class Reflection
    {
        /// <summary>
        /// 使用反射來獲取模型的有效欄位名稱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static HashSet<string> GetValidColumns<T>()
        {
            var validColumns = new HashSet<string>();

            // 獲取類型的所有屬性
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // 這裡簡單處理，將所有公有屬性加入 validColumns，具體邏輯可以根據需求調整
                validColumns.Add(property.Name);
            }

            return validColumns;
        }

        /// <summary>
        /// 使用反射來獲取模型的有效欄位名稱及其對應的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, object?> GetValidColumnsWithValues<T>(T model)
        {
            var validColumns = new Dictionary<string, object?>();

            // 獲取類型的所有屬性
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // 將屬性名稱和其對應的值（可能為 null）加入字典中
                validColumns.Add(property.Name, property.GetValue(model));
            }

            return validColumns;
        }

        /// <summary>
        /// 使用反射來獲取模型的有效欄位名稱及其對應的資料 型態 及 值 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, (Type PropertyType, object? Value)> GetModelPropertiesWithValues<T>(T model)
        {
            var validColumns = new Dictionary<string, (Type, object?)>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                validColumns.Add(property.Name, (property.PropertyType, property.GetValue(model)));
            }

            return validColumns;
        }

    }
}
