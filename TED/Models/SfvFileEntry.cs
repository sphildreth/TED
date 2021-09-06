using System;
using System.Linq;

namespace TED.Models
{
    public sealed class SfvFileEntry
    {
        public string FileName { get; set; }

        public string Crc32 { get; set; }

        public int TrackNumber
        {
            get
            {
                if (FileName.Contains(".flac", StringComparison.OrdinalIgnoreCase) || FileName.Contains(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = FileName.Split(' ');
                    var i = int.TryParse(parts[0], out int trackNumber);
                    return i ? trackNumber : 0;
                }
                return 0;
            }
        }

        public string TrackName
        {
            get
            {
                if (FileName.Contains(".flac", StringComparison.OrdinalIgnoreCase) || FileName.Contains(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = FileName.Split(' ');
                    return string.Join(' ', parts.Skip(1).Take(parts.Length - 1));
                }
                return null;
            }
        }
    }
}