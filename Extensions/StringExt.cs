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

        public static string? CleanString(this string? input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return input.Replace("’", "'");
        }

        public static bool ContainsUnicodeCharacter(this string input)
        {
            const int MaxAnsiCode = 255;
            return input.Any(c => c > MaxAnsiCode);
        }
    }
}
