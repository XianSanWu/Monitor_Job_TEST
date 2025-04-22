using System.Text.RegularExpressions;

namespace Utilities.Utilities
{
    public class SortUtil
    {
        /// <summary>
        /// 字串數字排序
        /// </summary>
        public class NaturalSort : IComparer<string>
        {
            public int Compare(string? x, string? y)
            {
                if (x == null) return -1;
                if (y == null) return 1;

                var regex = new Regex(@"\d+|\D+");

                var xParts = regex.Matches(x);
                var yParts = regex.Matches(y);

                int minCount = Math.Min(xParts.Count, yParts.Count);

                for (int i = 0; i < minCount; i++)
                {
                    var xPart = xParts[i].Value;
                    var yPart = yParts[i].Value;

                    if (int.TryParse(xPart, out int xNum) && int.TryParse(yPart, out int yNum))
                    {
                        int numCompare = xNum.CompareTo(yNum);
                        if (numCompare != 0)
                            return numCompare;
                    }
                    else
                    {
                        int strCompare = string.Compare(xPart, yPart, StringComparison.OrdinalIgnoreCase);
                        if (strCompare != 0)
                            return strCompare;
                    }
                }

                return xParts.Count.CompareTo(yParts.Count);
            }
        }

    }
}
