using System;
using System.Collections.Generic;
using System.Linq;

namespace TED.Models.TagData
{
    public class Release
    {
        public string Artist { get; set; }

        public string Directory { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; set; }

        public bool IsNameValid => !string.IsNullOrEmpty(Name);

        public int ExpectedTrackNumber { get; set; }

        public int TotalTrackNumber => ReleaseMedia.Sum(x => x.TotalTrackNumber);

        /// <summary>
        /// When true at least one of the tracks has an image via Tag data (not file base images)
        /// </summary>
        public bool HasTagImage { get; set; }

        public bool IsComplete => (ExpectedTrackNumber == 0 || (ExpectedTrackNumber > 0 && ExpectedTrackNumber == TotalTrackNumber)) && (ReleaseMedia?.All(x => x.IsComplete) ?? false);

        public bool IsValid => HasValidYear && IsNameValid && IsComplete && IsTrackDurationValid;

        public double TrackDuration => ReleaseMedia.Sum(x => x.TrackDuration);

        public bool IsTrackDurationValid => TrackDuration > 0;

        public string TrackDurationFormatted => TrackDuration == 0 ? null : TimeSpan.FromSeconds(TrackDuration).ToString(TagData.TimeSpanFormat);

        public long ReleaseFileSize => ReleaseMedia.Sum(x => x.FileSize);

        public string ReleaseFileSizeFormatted => FileDirectory.SizeSuffix(ReleaseFileSize);

        public int TotalDiscCount => ReleaseMedia.Count();

        public IEnumerable<string> ImageFiles { get; set; } = Enumerable.Empty<string>();

        public int Year { get; set; }

        public bool HasValidYear => Year < 2200 && Year > 1900;

        public IEnumerable<ReleaseMedia> ReleaseMedia { get; set; }
    }
}