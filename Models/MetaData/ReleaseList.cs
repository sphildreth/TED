using NodaTime;
using Roadie.Utility;

namespace Roadie.Models.MetaData
{
    public sealed class ReleaseList<T> : MetaDataBase
    {
        public ReleaseList(IRandomNumber randomNumber, IClock clock, DataToken artist, Image artistThumbnail, decimal? duration, DataToken genre, DateTime? lastPlayed, IEnumerable<ReleaseMediaList<T>> media, int? mediaCount, double? rank, short? rating, DataToken release)
            : base(randomNumber, clock)
        {
            Artist = artist;
            ArtistThumbnail = artistThumbnail;
            Duration = duration;
            Genre = genre;
            LastPlayed = lastPlayed;
            Media = media;
            MediaCount = mediaCount;
            Rank = rank;
            Rating = rating;
            Release = release;
        }

        public DataToken Artist { get;  }

        public Image ArtistThumbnail { get;  }

        public decimal? Duration { get;  }

        public string DurationTime
        {
            get
            {
                if (!Duration.HasValue)
                {
                    return "--:--";
                }

                return new TimeInfo(Duration.Value).ToFullFormattedString();
            }
        }

        public DataToken Genre { get;  }

        public bool IsValid
        {
            get
            {
                var artistName = Artist?.Text;
                var releaseName = Release?.Text;
                return Id != Guid.Empty &&
                       !string.IsNullOrEmpty(artistName) &&
                       !string.IsNullOrEmpty(releaseName);
            }
        }

        public DateTime? LastPlayed { get;  }

        public IEnumerable<ReleaseMediaList<T>> Media { get;  }

        public int? MediaCount { get;  }

        public double? Rank { get;  }

        public short? Rating { get;  }

        public DataToken Release { get;  }

    }
}