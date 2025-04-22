using System.Text;

namespace Utilities.Utilities
{
    public class Base64Util
    {
        /// <summary>
        /// 將字串轉為 Base64 編碼
        /// </summary>
        public static string Encode(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 將 Base64 字串解碼回原本字串
        /// </summary>
        public static string Decode(string? base64Encoded)
        {
            if (string.IsNullOrEmpty(base64Encoded))
                return string.Empty;

            byte[] bytes = Convert.FromBase64String(base64Encoded);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
