
namespace Utilities.Extensions
{
    public class PathHelper
    {
        /// <summary>
        /// 可列舉型別擴充模組
        /// </summary>
        public static class PathExtension
        {
            #region Methods

            #region Path 

            /// <summary>
            /// 取得某目錄的上幾層的目錄路徑
            /// </summary>
            /// <param name="folderPath"></param>
            /// <param name="levels"></param>
            /// <returns></returns>
            public static string GetParentDirectoryPath(string folderPath, int levels)
            {
                string result = folderPath;
                for (int i = 0; i < levels; i++)
                {
                    var parent = Directory.GetParent(result);
                    if (parent != null)
                    {
                        result = parent.FullName;
                    }
                    else
                    {
                        return result;
                    }
                }

                return result;
            }

            /// <summary>
            /// 取得某目錄的上層的目錄路徑
            /// </summary>
            /// <param name="folderPath"></param>
            /// <returns></returns>
            public static string GetParentDirectoryPath(string folderPath)
            {
                return GetParentDirectoryPath(folderPath, 1);
            }

            /// <summary>
            /// 取得路徑的目錄路徑
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public static string GetDirectoryPath(string filePath)
            {
                return Path.GetDirectoryName(filePath) ?? string.Empty;
            }

            #endregion

            #endregion

        }
    }
}