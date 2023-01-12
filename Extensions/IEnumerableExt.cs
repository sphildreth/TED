namespace TED.Extensions
{
    public static class IEnumerableExt
    {
        public static string ToCsv<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return string.Join(",", source.Select(s => s.ToString()).ToArray());
        }
    }
}
