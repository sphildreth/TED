using NodaTime;
using Roadie.Enums;
using Roadie.Utility;

namespace Roadie.Models.MetaData
{
    public class TrackList : MetaDataBase
    {

        public TrackList(IRandomNumber randomNumber, IClock clock, string fileName, string fileHash, int? duration, int? favoriteCount, int? fileSize, DateTime? lastPlayed, int? mediaNumber, IEnumerable<string> partTitlesList, int? playedCount, short? rating, DateTime? releaseDate, Statuses? status, Image thumbnail, string title, DataToken track, ArtistList trackArtist, int? trackNumber, string trackPlayUrl)
            : base(randomNumber, clock)
        {
            FileName = fileName;
            FileHash = fileHash;
            Duration = duration;
            FavoriteCount = favoriteCount;
            FileSize = fileSize;
            LastPlayed = lastPlayed;
            MediaNumber = mediaNumber;
            PartTitlesList = partTitlesList;
            PlayedCount = playedCount;
            Rating = rating;
            ReleaseDate = releaseDate;
            Status = status;
            Thumbnail = thumbnail;
            Title = title;
            Track = track;
            TrackArtist = trackArtist;
            TrackNumber = trackNumber;
            TrackPlayUrl = trackPlayUrl;
        }

        public string FileName { get;  }

        public string FileHash { get; }

        public int? Duration { get; }

        public string DurationTime => Duration.HasValue ? new TimeInfo(Duration.Value).ToFullFormattedString() : "--:--";

        public string DurationTimeShort => Duration.HasValue ? new TimeInfo(Duration.Value).ToShortFormattedString() : "--:--";

        public int? FavoriteCount { get; }

        public int? FileSize { get; }

        public DateTime? LastPlayed { get; }

        public int? MediaNumber { get; }

        public IEnumerable<string> PartTitlesList { get; }

        public int? PlayedCount { get; }

        public short? Rating { get; }

        public DateTime? ReleaseDate { get; }

        public Statuses? Status { get; }

        public string StatusVerbose => (Status ?? Statuses.Missing).ToString();

        public Image Thumbnail { get; }

        public string Title { get; }

        public DataToken Track { get; }

        public ArtistList TrackArtist { get; }

        public int? TrackNumber { get; }

        public string TrackPlayUrl { get; }

        public int? Year
        {
            get
            {
                if (ReleaseDate.HasValue)
                {
                    return ReleaseDate.Value.Year;
                }

                return null;
            }
        }
    }
}