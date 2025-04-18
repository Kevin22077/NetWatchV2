using System;
using System.Globalization;

namespace NetWatchV2.Extensions
{
    public static class StringExtensions
    {
        public static string ToUpperFirstChar(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return input.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + input.Substring(1);
        }
    }
}