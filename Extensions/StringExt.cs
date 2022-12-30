using System.Text.RegularExpressions;

namespace TED.Extensions
{
    public static class StringExt
    {
        public static string? Nullify(this string? input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return input;
        }

    }
}
