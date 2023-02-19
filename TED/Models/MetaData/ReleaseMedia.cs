using System.Text.Json.Serialization;
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

        [JsonIgnore]
        public short MediaNumberValue => MediaNumber ?? 0;

        public string SubTitle { get; set; }

        public int? TrackCount { get; set; }

        public IEnumerable<Track> Tracks { get; set; } = Enumerable.Empty<Track>();

        public IEnumerable<int> MissingTrackNumbers 
        {
            get
            {
                if(!Tracks.Any())
                {
                    return Enumerable.Empty<int>();
                }
                return Enumerable.Range(Tracks.Min(x => x.TrackNumber ?? 0), Tracks.Count()).Except(Tracks.Select(x => x.TrackNumber ?? 0));
            }
        }

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
