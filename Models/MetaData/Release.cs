using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using TED.Enums;
using TED.Extensions;
using TED.Utility;

namespace TED.Models.MetaData
{
    public class Release : MetaDataBase
    {
        public Release()
            :this(null, null)
        {
        }

        public Release(IRandomNumber? randomNumber, IClock? clock)
            : base(randomNumber, clock)
        {
        }

        public override string ToString()
        {
            return $"Status [{Status}] Directory [{ Directory }]";
        }

        public DataToken? Artist { get; set; }

        public Image? ArtistThumbnail { get; set; }

        public Image? CoverImage { get; set; }

        /// <summary>
        /// Total Duration of the Release in Milliseconds
        /// </summary>
        public double? Duration { get; set; }

        /// <summary>
        /// Total Duration of the Release in Minutes
        /// </summary>
        public double? DurationMinutes => Duration == null ? null : TimeSpan.FromMilliseconds(Duration.Value).TotalMinutes;

        /// <summary>
        /// This is the file directory for the Release
        /// </summary>
        [Required]
        public string Directory { get; set; }

        public string DurationTime
        {
            get
            {
                if (!Duration.HasValue)
                {
                    return "--:--";
                }

                return new TimeInfo(SafeParser.ToNumber<decimal>(Duration.Value)).ToFullFormattedString();
            }
        }

        public string FormattedMediaSize
        {
            get
            {
                return SafeParser.ToNumber<long>(Media?.SelectMany(x => x.Tracks).Sum(x => x.FileSize) ?? 0).FormatFileSize();
            }
        }

        public DataToken? Genre { get; set; }

        public bool IsValid => Id != Guid.Empty &&
                       !string.IsNullOrEmpty(Artist?.Text) &&
                       !string.IsNullOrEmpty(ReleaseData?.Text) &&
                       CoverImage?.Bytes?.Length > 0 &&
                       (Status != Statuses.Incomplete && Status != Statuses.NeedsAttention && Status != Statuses.Incomplete);

        public IEnumerable<ReleaseMedia>? Media { get; set; }

        public int? MediaCount { get; set; }

        public DataToken? ReleaseData { get; set; }

        string? _releaseDate;
        public string? ReleaseDate
        {
            get
            {
                if (!string.IsNullOrEmpty(_releaseDate))
                {
                    return _releaseDate;
                }
                return ReleaseDateDateTime.HasValue
                        ? ReleaseDateDateTime.Value.ToUniversalTime().ToString("yyyy-MM-dd")
                        : null;
            }
            set => _releaseDate = value;
        }

        public int? Year { get; set; }

        private DateTime? _releaseDateDateTime = DateTime.MinValue;

        [JsonIgnore]
        public DateTime? ReleaseDateDateTime
        {
            get => _releaseDateDateTime;
            set
            {
                if (value.HasValue)
                {
                    Year = value.Value.Year;
                }
                _releaseDateDateTime = value;
            }
        }

        public int? TrackCount { get; set; }

        public Statuses Status { get; set; } = Statuses.Incomplete;

        [JsonIgnore]
        public List<ProcessMessage> ProcessingMessages { get; set; } = new List<ProcessMessage>();
    }
}