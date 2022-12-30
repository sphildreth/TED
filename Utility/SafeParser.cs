using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TED.Utility
{
    public static class SafeParser
    {
        private const string Salt = "9A0786D9-3DF7-4BE3-AB51-6E1CB91B028A";

        /// <summary>
        ///     Safely return a Boolean for a given Input.
        ///     <remarks>Has Additional String Operations</remarks>
        /// </summary>
        public static bool ToBoolean(object input)
        {
            if(input == null)
            {
                return false;
            }
            switch (input)
            {
                case null:
                    return false;

                case bool t:
                    return t;

                default:
                    switch (input.ToString()?.ToLower())
                    {
                        case "true":
                        case "1":
                        case "y":
                        case "yes":
                            return true;

                        default:
                            return false;
                    }
            }
        }

        public static DateTime? ToDateTime(object input)
        {
            if (input == null) return default(DateTime?);
            try
            {
                var dt = DateTime.MinValue;
                var i = input as string ?? input.ToString();
                if (!string.IsNullOrEmpty(i))
                {
                    if (Regex.IsMatch(i, @"([0-9]{4}).+([0-9]{4})")) i = i.Substring(0, 4);
                    i = Regex.Replace(i, @"(\\)", "/");
                    i = Regex.Replace(i, @"(;)", "/");
                    i = Regex.Replace(i, @"(\/+)", "/");
                    i = Regex.Replace(i, @"(-+)", "/");
                    var parts = i.Contains("/") ? i.Split('/').ToList() : new List<string> { i };
                    if (parts.Count == 2)
                        if (parts[0] != null && parts[1] != null && parts[0] == parts[1])
                            parts = new List<string> { parts[0], "01" };
                    while (parts.Count < 3) parts.Insert(0, "01");
                    var tsRaw = string.Empty;
                    foreach (var part in parts)
                    {
                        if (tsRaw.Length > 0) tsRaw += "/";
                        tsRaw += part;
                    }

                    DateTime.TryParse(tsRaw, out dt);
                }

                try
                {
                    return ChangeType<DateTime?>(input);
                }
                catch
                {
                    // ignored
                }

                return dt != DateTime.MinValue ? (DateTime?)dt : null;
            }
            catch
            {
                return default(DateTime?);
            }
        }

        public static T ToEnum<T>(object input) where T : struct, IConvertible
        {
            if (input == null) return default(T);
            Enum.TryParse(input.ToString(), true, out T r);
            return r;
        }

        public static Guid? ToGuid(object input)
        {
            if (input == null) return null;
            var i = input.ToString();
            if (!string.IsNullOrEmpty(i) && i.Length > 0 && i[1] == ':') i = i.Substring(2, i.Length - 2);
            if (!Guid.TryParse(i, out var result)) return null;
            return result;
        }

        /// <summary>
        ///     Safely Return a Number For Given Input
        /// </summary>
        public static T? ToNumber<T>(object? input)
        {
            if (input == null) return default(T);
            try
            {
                return ChangeType<T>(input);
            }
            catch
            {
                return default(T);
            }
        }

        public static string? ToString(object input, string? defaultValue = null)
        {
            defaultValue = defaultValue ?? string.Empty;
            switch (input)
            {
                case null:
                    return defaultValue;

                case string r:
                    return r.Trim();

                default:
                    return defaultValue;
            }
        }

        public static int? ToYear(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            int parsed;
            if (input.Length == 4)
            {
                if (int.TryParse(input, out parsed)) return parsed > 0 ? (int?)parsed : null;
            }
            else if (input.Length > 4)
            {
                if (int.TryParse(input.Substring(0, 4), out parsed)) return parsed > 0 ? (int?)parsed : null;
            }

            return null;
        }

        private static T? ChangeType<T>(object value)
        {
            Type? t = typeof(T);            
            if (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return (T)Convert.ChangeType(value, t);
            }
            if (value == null)
            {
                return default(T);
            } 
            t = Nullable.GetUnderlyingType(t);
            return t == null ? default(T) : (T)Convert.ChangeType(value, t);
        }

        public static string ToToken(string input)
        {
            var hashids = new Hashids(Salt);
            var numbers = 0;
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);
            var looper = bytes.Length / 4;
            for (var i = 0; i < looper; i++)
            {
                numbers += BitConverter.ToInt32(bytes, i * 4);
            }
            if (numbers < 0)
            {
                numbers *= -1;
            }

            return hashids.Encode(numbers);
        }

        public static string? ToFileNameFriendly(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return Regex.Replace(PathSanitizer.SanitizeFilename(input, ' ') ?? string.Empty, @"\s+", " ").Trim();
        }
    }
}