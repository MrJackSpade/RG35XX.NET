namespace RG35XX.Core.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> list, T value)
        {
            int i = 0;

            foreach (T? item in list)
            {
                if (object.Equals(item, value))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }
    }
}