using NodaTime;
using Roadie.Utility;

namespace Roadie.Models.MetaData
{
    [Serializable]
    public sealed class ReleaseMediaList<T> : MetaDataBase
    {
        public ReleaseMediaList(IRandomNumber randomNumber, IClock clock, short? mediaNumber, string? subTitle, int? trackCount, IEnumerable<T> tracks)
            : base(randomNumber, clock)
        {
            MediaNumber = mediaNumber;
            SubTitle = subTitle;
            TrackCount = trackCount;
            Tracks = tracks;
        }

        public short? MediaNumber { get; }
        public string? SubTitle { get; }
        public int? TrackCount { get; }
        public IEnumerable<T> Tracks { get; } = Enumerable.Empty<T>();
    }
}