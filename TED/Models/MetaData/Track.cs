using System.Text.Json.Serialization;
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

        public bool IsValid => Id != Guid.Empty &&
                       TrackNumber > 0 &&
                       Duration > 0 &&
                       !string.IsNullOrEmpty(Title) &&
                       !string.IsNullOrEmpty(FileName) &&
                       FileSize > 0 &&
                       (Status != Statuses.Missing && Status != Statuses.NeedsAttention);

        public string FileName { get; set; }

        public string FileHash { get; set; }

        public double? Duration { get; set; }

        public string DurationTime => Duration.HasValue ? new TimeInfo(SafeParser.ToNumber<decimal>(Duration.Value)).ToFullFormattedString() : "--:--";

        public string DurationTimeShort => Duration.HasValue ? new TimeInfo(SafeParser.ToNumber<decimal>(Duration.Value)).ToShortFormattedString() : "--:--";

        public int? FileSize { get; set; }

        public IEnumerable<string> PartTitlesList { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public Statuses Status { get; set; } = Statuses.Missing;

        public string StatusVerbose => Status.ToString();

        public Image Thumbnail { get; set; }

        public string Title { get; set; }

        public Artist TrackArtist { get; set; }

        public int? TrackNumber { get; set; }

        [JsonIgnore]
        public int TrackNumberValue => TrackNumber ?? 0;

        public override string ToString()
        {
            return $"TrackNumber [{TrackNumber}] Duration [{Duration}] FileSize [{FileSize}] Title [{Title}]";
        }
    }
}
