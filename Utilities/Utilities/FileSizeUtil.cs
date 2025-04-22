
namespace Utilities.Utilities
{
    public class FileSizeUtil
    {
        public static int ConvertToBytes(int size, FileSize unit)
        {
            return unit switch
            {
                FileSize.B => size,
                FileSize.KB => size * 1024,
                FileSize.MB => size * 1024 * 1024,
                FileSize.GB => size * 1024 * 1024 * 1024,
                FileSize.TB => size * 1024 * 1024 * 1024 * 1024,
                FileSize.PB => size * 1024 * 1024 * 1024 * 1024 * 1024,
                _ => throw new ArgumentOutOfRangeException(nameof(unit), "Unsupported file size unit.")
            };
        }

        public enum FileSize
        {
            B,     // Byte
            KB,    // Kilobyte
            MB,    // Megabyte
            GB,    // Gigabyte
            TB,    // Terabyte
            PB     // Petabyte
        }
    }
}
