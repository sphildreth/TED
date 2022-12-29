using NodaTime;
using TED.Enums;
using TED.Utility;

namespace TED.Models.MetaData
{
    public class TrackList : MetaDataBase
    {
        public TrackList()
            : base(null, null)
        {
        }

        public TrackList(IRandomNumber randomNumber, IClock clock)
            : base(randomNumber, clock)
        {
        }

        public string FileName { get; set; }

        public string FileHash { get; set; }

        public int? Duration { get; set; }

        public string DurationTime => Duration.HasValue ? new TimeInfo(Duration.Value).ToFullFormattedString() : "--:--";

        public string DurationTimeShort => Duration.HasValue ? new TimeInfo(Duration.Value).ToShortFormattedString() : "--:--";

        public int? FavoriteCount { get; set; }

        public int? FileSize { get; set; }

        public DateTime? LastPlayed { get; set; }

        public int? MediaNumber { get; set; }

        public IEnumerable<string>? PartTitlesList { get; set; }

        public int? PlayedCount { get; set; }

        public short? Rating { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public Statuses? Status { get; set; }

        public string StatusVerbose => (Status ?? Statuses.Missing).ToString();

        public Image? Thumbnail { get; set; }

        public string Title { get; set; }

        public DataToken? Track { get; set; }

        public ArtistList? TrackArtist { get; set; }

        public int? TrackNumber { get; set; }

        public string TrackPlayUrl { get; set; }

        public int? Year
        {
            get
            {
                if (ReleaseDate.HasValue)
                {
                    return ReleaseDate.Value.Year;
                }

                return null;
            }
        }
    }
}