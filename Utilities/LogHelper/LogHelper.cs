using System.Runtime.CompilerServices;

namespace Utilities.LogHelper
{
    public static class LogHelper
    {
        public static string Build<T>([CallerMemberName] string? methodName = null)
        {
            return $"{typeof(T).Name}.{methodName}";
        }
    }
}
