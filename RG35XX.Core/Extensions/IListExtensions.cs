namespace RG35XX.Core.Extensions
{
    public static class IListExtensions
    {
        public static T Pop<T>(this IList<T> list)
        {
            T item = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return item;
        }

        public static void Push<T>(this IList<T> list, T item)
        {
            list.Add(item);
        }

        public static T Peek<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        public static T Peek<T>(this IList<T> list, int index)
        {
            return list[list.Count - 1 - index];
        }
    }
}
