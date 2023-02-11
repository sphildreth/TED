using NodaTime;
using TED.Utility;

namespace TED.Models.MetaData
{
    [Serializable]
    public sealed class ReleaseMedia : MetaDataBase
    {
        public ReleaseMedia()
            : base(null, null)
        {
        }

        public ReleaseMedia(IRandomNumber randomNumber, IClock clock)
            : base(randomNumber, clock)
        {
        }

        public bool IsValid => Id != Guid.Empty &&
                       MediaNumber > 0 &&
                       TrackCount > 0 &&
                       (Tracks?.Any() ?? false);

        public short? MediaNumber { get; set; }

        public string SubTitle { get; set; }

        public int? TrackCount { get; set; }

        public IEnumerable<Track> Tracks { get; set; } = Enumerable.Empty<Track>();

        public Track TrackById(Guid id)
        {
            return Tracks?.FirstOrDefault(x => x.Id == id);
        }

        public override string ToString()
        {
            return $"MediaNumber [{MediaNumber}] TrackCount [{TrackCount}]";
        }
    }
}
