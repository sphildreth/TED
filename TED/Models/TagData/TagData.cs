using System;
using System.Collections.Generic;
using System.Linq;

namespace TED.Models.TagData
{
    public class TagData
    {
        public static string TimeSpanFormat = @"hh\:mm\:ss\.fff";

        public string ArtistName
        {
            get
            {
                var artistsForReleases = Releases.Select(x => x.Artist).Distinct();
                if(artistsForReleases.Count() == 1)
                {
                    return artistsForReleases.First();
                }
                return null;
            }
        }

        public long GenerationDuration { get; set; }

        public bool AllReleasesComplete => Releases.All(x => x.IsComplete);

        public bool IsValid => AllReleasesComplete;

        public int ReleaseCount => Releases.Count();

        public int MediaCount => Releases.Sum(x => x.TotalTrackNumber);

        public long TotalFileSize => Releases.Sum(x => x.ReleaseFileSize);

        public string TotalFileSizeFormatted => FileDirectory.SizeSuffix(TotalFileSize);

        public double TotalTrackDuration => Releases.Sum(x => x.TrackDuration);

        public string TotalTrackDurationFormnatted => TotalTrackDuration == 0 ? null : TimeSpan.FromSeconds(TotalTrackDuration).ToString(TagData.TimeSpanFormat);

        public bool AllReleasesHaveImages => Releases.All(x => x.HasTagImage || x.ImageFiles.Any());

        public IEnumerable<string> ArtistImageFiles { get; set; }

        public IEnumerable<Release> Releases { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}