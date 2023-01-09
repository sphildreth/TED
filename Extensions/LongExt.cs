﻿namespace TED.Extensions
{
    public static class LongExt
    {
        public static string FormatFileSize(this long size)
        {
            if (size < 900)
            {
                return $"{size} B";
            }
            var ds = size / 1024.0;
            if (ds < 900)
            {
                return $"{ds:F2} KB";
            }
            ds /= 1024.0;
            if (ds < 900)
            {
                return $"{ds:F2} MB";
            }
            ds /= 1024.0;
            if (ds < 900)
            {
                return $"{ds:F3} GB";
            }
            ds /= 1024.0;
            if (ds < 900)
            {
                return $"{ds:F3} TB";
            }
            ds /= 1024.0;
            return $"{ds:F4} PB";
        }
    }
}
