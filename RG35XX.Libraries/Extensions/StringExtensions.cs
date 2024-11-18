using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG35XX.Libraries.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<int> AllIndexesOf(this string source, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("the string to find may not be empty", nameof(value));
            }

            for (int index = 0; ; index += value.Length)
            {
                index = source.IndexOf(value, index);
                if (index == -1)
                {
                    break;
                }

                yield return index;
            }
        }

        public static string From(this string source, string fromStr)
        {
            string result = source[(source.IndexOf(fromStr) + fromStr.Length)..];
            return result;
        }

        public static string To(this string source, string toStr)
        {
            string result = source[..source.IndexOf(toStr)];
            return result;
        }
    }
}