using NodaTime;
using TED.Utility;

namespace TED.Models.MetaData
{
    public sealed class ReleaseList : MetaDataBase
    {
        public ReleaseList()
            :base(null, null)
        {
        }

        public ReleaseList(IRandomNumber randomNumber, IClock clock)
            : base(randomNumber, clock)
        {
        }

        public DataToken? Artist { get; set; }

        public Image? ArtistThumbnail { get; set; }

        public decimal? Duration { get; set; }

        public string DurationTime
        {
            get
            {
                if (!Duration.HasValue)
                {
                    return "--:--";
                }

                return new TimeInfo(Duration.Value).ToFullFormattedString();
            }
        }

        public DataToken? Genre { get; set; }

        public bool IsValid
        {
            get
            {
                var artistName = Artist?.Text;
                var releaseName = Release?.Text;
                return Id != Guid.Empty &&
                       !string.IsNullOrEmpty(artistName) &&
                       !string.IsNullOrEmpty(releaseName);
            }
        }

        public DateTime? LastPlayed { get; set; }

        public IEnumerable<ReleaseMediaList>? Media { get; set; }

        public int? MediaCount { get; set; }

        public double? Rank { get; set; }

        public short? Rating { get; set; }

        public DataToken? Release { get; set; }

    }
}