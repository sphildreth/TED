using NodaTime;
using TED.Utility;

namespace TED.Models.MetaData
{
    [Serializable]
    public sealed class ReleaseMediaList : MetaDataBase
    {
        public ReleaseMediaList()
            : base(null, null)
        {
        }

        public ReleaseMediaList(IRandomNumber randomNumber, IClock clock)
            : base(randomNumber, clock)
        {
        }

        public short? MediaNumber { get; set; }

        public string? SubTitle { get; set; }

        public int? TrackCount { get; set; }

        public IEnumerable<TrackList> Tracks { get; set; } = Enumerable.Empty<TrackList>();
    }
}
