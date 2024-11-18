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
            int index = source.IndexOf(fromStr);
            if (index == -1)
            {
                throw new ArgumentException("Search string does not exist in source string", nameof(fromStr));
            }

            string result = source[(index + fromStr.Length)..];
            return result;
        }

        public static string To(this string source, string toStr)
        {
            int index = source.IndexOf(toStr);

            if (index < 0)
            {
                throw new ArgumentException("Search string does not exist in source string", nameof(toStr));
            }

            string result = source[..index];
            return result;
        }
    }
}