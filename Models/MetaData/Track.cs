using NodaTime;
using TED.Enums;
using TED.Utility;

namespace TED.Models.MetaData
{
    public class Track : MetaDataBase
    {
        public Track()
            : base(null, null)
        {
        }

        public Track(IRandomNumber randomNumber, IClock clock)
            : base(randomNumber, clock)
        {
        }

        public string? FileName { get; set; }

        public string? FileHash { get; set; }

        public double? Duration { get; set; }

        public string? DurationTime => Duration.HasValue ? new TimeInfo(SafeParser.ToNumber<decimal>(Duration.Value)).ToFullFormattedString() : "--:--";

        public string? DurationTimeShort => Duration.HasValue ? new TimeInfo(SafeParser.ToNumber<decimal>(Duration.Value)).ToShortFormattedString() : "--:--";

        public int? FileSize { get; set; }

        public IEnumerable<string>? PartTitlesList { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public Statuses? Status { get; set; }

        public string StatusVerbose => (Status ?? Statuses.Missing).ToString();

        public Image? Thumbnail { get; set; }

        public string? Title { get; set; }

        public DataToken? TrackData { get; set; }

        public Artist? TrackArtist { get; set; }

        public int? TrackNumber { get; set; }
    }
}