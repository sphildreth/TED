using NodaTime;
using TED.Enums;
using TED.Utility;

namespace TED.Models.MetaData
{
    [Serializable]
    public sealed class Artist : MetaDataBase
    {
        public Artist()
            : base(null, null)
        {
        }

        public Artist(IRandomNumber randomNumber, IClock clock)
            : base(randomNumber, clock)
        {
        }

        public DataToken? ArtistData { get; set; }

        public bool IsValid => Id != Guid.Empty && 
                               !string.IsNullOrEmpty(ArtistData?.Text);

        public IEnumerable<string>? MissingReleasesForCollection { get; set; }

        public int? ReleaseCount { get; set; }

        public Image? Thumbnail { get; set; }

        public int? TrackCount { get; set; }

        public Statuses? Status { get; set; }

        public string StatusVerbose => (Status ?? Statuses.Missing).ToString();
    }
}
