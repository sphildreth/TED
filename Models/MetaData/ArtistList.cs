using NodaTime;
using Roadie.Enums;
using Roadie.Utility;

namespace Roadie.Models.MetaData
{
    [Serializable]
    public sealed class ArtistList : MetaDataBase
    {
        public ArtistList(IRandomNumber randomNumber, IClock clock, DataToken artist, DateTime? lastPlayed, IEnumerable<string> missingReleasesForCollection, int? playedCount, double? rank, short? rating, int? releaseCount, Image thumbnail, int? trackCount, Statuses? status)
            :base(randomNumber, clock)
        {
            Artist = artist;
            LastPlayed = lastPlayed;
            MissingReleasesForCollection = missingReleasesForCollection;
            PlayedCount = playedCount;
            Rank = rank;
            Rating = rating;
            ReleaseCount = releaseCount;
            Thumbnail = thumbnail;
            TrackCount = trackCount;
            Status = status;
        }

        public DataToken Artist { get; }
        public bool IsValid => Id != Guid.Empty;
        public DateTime? LastPlayed { get; }
        public IEnumerable<string> MissingReleasesForCollection { get; }
        public int? PlayedCount { get;  }
        public double? Rank { get;  }
        public short? Rating { get;  }
        public int? ReleaseCount { get;  }
        public Image Thumbnail { get;  }
        public int? TrackCount { get;  }
        public Statuses? Status { get;  }
        public string StatusVerbose => (Status ?? Statuses.Missing).ToString();

    }
}