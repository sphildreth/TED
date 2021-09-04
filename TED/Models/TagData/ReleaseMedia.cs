using System;
using System.Collections.Generic;
using System.Linq;

namespace TED.Models.TagData
{
    public class ReleaseMedia
    {
        public int ReleaseMediaNumber { get; set; }

        public string Name { get; set; }

        public int TotalTrackNumber => Media.Count();

        public bool IsComplete => Media.Count() == Media.Max(x => x.TrackNumber);

        public bool IsValid => IsComplete && IsTrackDurationValid;

        public double TrackDuration => Media.Sum(x => x.Length);

        public bool IsTrackDurationValid => TrackDuration > 0;

        public string TrackDurationFormatted => TrackDuration == 0 ? null : TimeSpan.FromSeconds(TrackDuration).ToString(TagData.TimeSpanFormat);

        public long FileSize => Media.Sum(x => x.FileSize);

        public string FileSizeFormatted => FileDirectory.SizeSuffix(FileSize);

        public IEnumerable<Media> Media { get; set; }
    }
}