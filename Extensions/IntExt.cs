using System.Runtime.CompilerServices;

namespace TED.Extensions
{
    public static class IntExt
    {
        public static string ToStringPadLeft(this int? input, short padLeft, char padWith = '0')
        {
            if(input == null)
            {
                return null;
            }
            return input.ToString().PadLeft(padLeft, padWith);
        }
    }
}
