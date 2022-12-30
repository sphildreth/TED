using MudBlazor;
using MudBlazor.Charts;
using System.Text.Json;
using System.Text.RegularExpressions;
using TED.Enums;
using TED.Extensions;
using TED.Models.MetaData;
using TED.Utility;

namespace TED.Processors
{
    public sealed class DirectoryProcessor
    {
        private static readonly Regex _hasFeatureFragmentsRegex = new(@"\((ft.|feat.|featuring|feature)+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _unwantedReleaseTitleTextRegex = new(@"(\\s*(-\\s)*((CD[_\-#\s]*[0-9]*)))|((\\(|\\[)+([0-9]|,|self|bonus|re(leas|master|(e|d)*)*|th|anniversary|cd|disc|deluxe|dig(ipack)*|vinyl|japan(ese)*|asian|remastered|limited|ltd|expanded|edition|\\s)+(]|\\)*))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _unwantedTrackTitleTextRegex = new(@"^([0-9]+)(\.|-|\s)*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DirectoryProcessor()
        {
        }

        public Release? Process(DateTime now, string dir, string[] filesInDirectory)
        {
            var allfileAtlsFound = new List<ATL.Track>();
            string? directorySFVFile = null;
            string? directoryM3UFile = null;
            var release = new Release();
            release.Directory = dir;
            foreach (var file in filesInDirectory)
            {
                var fileAtl = new ATL.Track(file);
                if (fileAtl != null)
                {
                    allfileAtlsFound.Add(fileAtl);
                }
                if(string.Equals(Path.GetExtension(file), ".sfv", StringComparison.OrdinalIgnoreCase))
                {
                    directorySFVFile = file;
                }
                if (string.Equals(Path.GetExtension(file), ".m3u", StringComparison.OrdinalIgnoreCase))
                {
                    directoryM3UFile = file;
                }
            }
            if (allfileAtlsFound.Any(x => x.AudioFormat.ID > -1))
            {
                var tagsFilesFound = allfileAtlsFound.Where(x => x.AudioFormat.ID > -1 && x.Duration > 0);
                var filesAtlsGroupedByRelease = tagsFilesFound.GroupBy(x => x.Album);
                foreach (var groupedByRelease in filesAtlsGroupedByRelease)
                {
                    var firstAtl = groupedByRelease.First();
                    DateTime? releaseDate = null;
                    var releaseYear = groupedByRelease.Where(x => x.Year > 0).FirstOrDefault()?.Year;
                    if (releaseYear.HasValue && releaseYear > 0)
                    {
                        releaseDate = new DateTime(releaseYear.Value, 1, 1);
                    }
                    var releaseData = new Release
                    {
                        Artist = new Models.DataToken
                        {
                            Value = SafeParser.ToToken(firstAtl.AlbumArtist.Nullify() ?? firstAtl.Artist),
                            Text = firstAtl.AlbumArtist.Nullify() ?? firstAtl.Artist
                        },
                        ReleaseData = new Models.DataToken
                        {
                            Value = SafeParser.ToToken(groupedByRelease.Key),
                            Text = groupedByRelease.Key
                        },
                        Genre = firstAtl.Genre.Nullify() == null ? null : new Models.DataToken
                        {
                            Value = SafeParser.ToToken(firstAtl.Genre),
                            Text = firstAtl.Genre
                        },
                        Directory = dir,
                        CreatedDate = now,
                        Id = Guid.NewGuid(),
                        MediaCount = tagsFilesFound.Select(x => x.DiscNumber ?? 0).Distinct().Count(),
                        ReleaseDateDateTime = releaseDate,
                        Year = releaseDate?.Year,
                        Status = Enums.Statuses.New,
                        TrackCount = groupedByRelease.First().TrackTotal
                    };
                    var firstAtlHasArtistImage = firstAtl.EmbeddedPictures?.FirstOrDefault(x => x.PicType == ATL.PictureInfo.PIC_TYPE.Artist ||
                                                                                                x.PicType == ATL.PictureInfo.PIC_TYPE.Band);
                    if (firstAtlHasArtistImage != null)
                    {
                        releaseData.ArtistThumbnail = new Models.Image
                        {
                            Bytes = firstAtlHasArtistImage.PictureData,
                            Caption = firstAtlHasArtistImage.Description
                        };
                    }
                    var firstAtlHasReleaseImage = firstAtl.EmbeddedPictures?.FirstOrDefault(x => x.PicType == ATL.PictureInfo.PIC_TYPE.Front ||
                                                                                                 x.PicType == ATL.PictureInfo.PIC_TYPE.Generic);
                    if (firstAtlHasReleaseImage != null)
                    {
                        releaseData.Thumbnail = new Models.Image
                        {
                            Bytes = firstAtlHasReleaseImage.PictureData,
                            Caption = firstAtlHasReleaseImage.Description
                        };
                    }
                    var medias = new List<ReleaseMedia>();
                    foreach (var mp3TagData in tagsFilesFound.GroupBy(x => x.DiscNumber))
                    {
                        var mediaTracks = tagsFilesFound.Where(x => x.DiscNumber == mp3TagData.Key);
                        var mediaNumber = SafeParser.ToNumber<short?>(mp3TagData.Key) ?? 0;
                        medias.Add(new ReleaseMedia
                        {
                            MediaNumber = mediaNumber < 1 ? SafeParser.ToNumber<short>(1) : mediaNumber,
                            TrackCount = mediaTracks.Count(),
                            SubTitle = mp3TagData.First().SeriesTitle,
                            Tracks = mediaTracks.OrderBy(x => x.TrackNumber).Select(x => new Track
                            {
                                CreatedDate = x.FileInfo().CreationTimeUtc,
                                LastUpdated = x.FileInfo().LastWriteTimeUtc,
                                Duration = x.DurationMs,
                                FileHash = HashHelper.GetHash(x.FileInfo().FullName).ToString(),
                                FileName = x.FileInfo().FullName,
                                FileSize = SafeParser.ToNumber<int?>(x.FileInfo().Length),
                                Id = Guid.NewGuid(),
                                Status = (x.FileInfo()?.Exists ?? false) ? Statuses.New : Statuses.Missing,
                                Title = x.Title,
                                TrackArtist = string.IsNullOrWhiteSpace(x.Artist) || string.Equals(releaseData.Artist.Value, x.Artist, StringComparison.OrdinalIgnoreCase) ? null : new Artist
                                {
                                    ArtistData = new Models.DataToken
                                    {
                                        Value = SafeParser.ToToken(x.Artist),
                                        Text = x.Artist
                                    }
                                },
                                TrackNumber = x.TrackNumber
                            }).ToArray()
                        });
                    }
                    releaseData.Media = medias;
                    releaseData.TrackCount = medias.Sum(x => x.TrackCount);
                    releaseData.Status = releaseData.Media.SelectMany(x => x.Tracks).Count() == releaseData.Media.Sum(x => x.TrackCount) ? Enums.Statuses.Ok : Enums.Statuses.Incomplete;
                    releaseData.Duration = medias.SelectMany(x => x.Tracks).Sum(x => x.Duration);
                    if(releaseData.Status == Enums.Statuses.Ok && directorySFVFile.Nullify() != null)
                    {
                        if(releaseData.TrackCount != GetMp3CountFromSFVFile(directorySFVFile)) 
                        {
                            releaseData.Status = Enums.Statuses.Incomplete;
                        }
                    }
                    if (releaseData.Status == Enums.Statuses.Ok && directoryM3UFile.Nullify() != null)
                    {
                        if (releaseData.TrackCount != GetMp3CountFromM3UFile(directoryM3UFile))
                        {
                            releaseData.Status = Enums.Statuses.Incomplete;
                        }
                    }
                    foreach(var media in releaseData.Media.OrderBy(x => x.MediaNumber).Select((v,i) => new { i, v}))
                    {
                        foreach(var track in media.v.Tracks.OrderBy(x => x.TrackNumber).Select((t, i) => new { i, t }))
                        {
                            track.t.Status = CheckTrackStatus(track.t);
                            if(track.t.TrackNumber != track.i + 1)
                            {
                                track.t.Status = Statuses.NeedsAttention;
                            }
                        }
                        if(media.v.MediaNumber != media.i + 1)
                        {
                            releaseData.Status = Statuses.NeedsAttention;
                        }
                    }
                    releaseData.Status = releaseData.Media.SelectMany(x => x.Tracks).Any(x => x.Status != Statuses.New) ? Statuses.NeedsAttention : releaseData.Status;
                    if(releaseData.Status == Statuses.Ok)
                    {
                        releaseData.Status = CheckReleaseStatus(releaseData);
                    }
                    var roadieDataFileName = Path.Combine(dir, $"ted.data.{releaseData.Artist.Text.ToFileNameFriendly()}.json");
                    System.IO.File.WriteAllText(roadieDataFileName, JsonSerializer.Serialize(releaseData));
                    return releaseData;
                }

            }
            return release;
        }

        private Statuses CheckReleaseStatus(Release release)
        {
            if (ReleaseHasUnwantedText(release?.ReleaseData?.Text ?? string.Empty))
            {
                return Statuses.NeedsAttention;
            }
            return release.Status ?? Statuses.NeedsAttention;
        }

        private Statuses CheckTrackStatus(Track track)
        {
            if(TrackHasFeaturingFragments(track?.Title ?? string.Empty) || TrackHasUnwantedText(track?.Title ?? string.Empty))
            {
                return Statuses.NeedsAttention;
            }
            return track.Status ?? Statuses.Missing;
        }

        private static bool TrackHasFeaturingFragments(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            return _hasFeatureFragmentsRegex.IsMatch(input);
        }

        private static bool ReleaseHasUnwantedText(string releaseTitle)
        {
            if (string.IsNullOrWhiteSpace(releaseTitle))
            {
                return false;
            }
            return _unwantedReleaseTitleTextRegex.IsMatch(releaseTitle);
        }

        private static bool TrackHasUnwantedText(string trackTitle)
        {
            if (string.IsNullOrWhiteSpace(trackTitle))
            {
                return false;
            }
            return _unwantedTrackTitleTextRegex.IsMatch(trackTitle);
        }


        private static int GetMp3CountFromM3UFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return 0;
            }
            var result = 0;
            foreach(var line in File.ReadAllLines(filePath)) 
            {
                if (IsLineForFileForTrack(line))
                {
                    result++;
                }
            }
            return result;
        }

        private static int GetMp3CountFromSFVFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return 0;
            }
            var result = 0;
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (IsLineForFileForTrack(line))
                {
                    result++;
                }
            }
            return result;
        }

        private static bool IsLineForFileForTrack(string lineFromFile)
        {
            if (string.IsNullOrWhiteSpace(lineFromFile))
            {
                return false;
            }
            if (lineFromFile.Contains(".mp3", StringComparison.OrdinalIgnoreCase) || 
                lineFromFile.Contains(".flac", StringComparison.OrdinalIgnoreCase) ||
                lineFromFile.Contains(".wav", StringComparison.OrdinalIgnoreCase) ||
                lineFromFile.Contains(".ac4", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}
