using NodaTime;
using TED.Enums;
using TED.Utility;

namespace TED.Models.MetaData
{
    [Serializable]
    public sealed class ArtistList : MetaDataBase
    {
        public ArtistList()
            : base(null, null)
        {
        }

        public ArtistList(IRandomNumber randomNumber, IClock clock)
            : base(randomNumber, clock)
        {
        }

        public DataToken? Artist { get; set; }

        public bool IsValid => Id != Guid.Empty;

        public DateTime? LastPlayed { get; set; }

        public IEnumerable<string>? MissingReleasesForCollection { get; set; }

        public int? PlayedCount { get; set; }

        public double? Rank { get; set; }

        public short? Rating { get; set; }

        public int? ReleaseCount { get; set; }

        public Image? Thumbnail { get; set; }

        public int? TrackCount { get; set; }

        public Statuses? Status { get; set; }

        public string StatusVerbose => (Status ?? Statuses.Missing).ToString();
    }
}
