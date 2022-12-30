using MudBlazor;
using System.Text.Json;
using TED.Extensions;
using TED.Models.MetaData;
using TED.Utility;

namespace TED.Processors
{
    public sealed class DirectoryProcessor
    {
        public DirectoryProcessor()
        {
        }

        public Release? Process(DateTime now, string dir, string[] filesInDirectory)
        {
            var allfileAtlsFound = new List<ATL.Track>();
            var release = new Release();
            release.Directory = dir;
            foreach (var file in filesInDirectory)
            {
                var fileAtl = new ATL.Track(file);
                if (fileAtl != null)
                {
                    allfileAtlsFound.Add(fileAtl);
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
                                Status = Enums.Statuses.New,
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
                    releaseData.Status = releaseData.Media.SelectMany(x => x.Tracks).Count() == releaseData.Media.Sum(x => x.TrackCount) ? Enums.Statuses.Ok : Enums.Statuses.Incomplete;
                    releaseData.Duration = medias.SelectMany(x => x.Tracks).Sum(x => x.Duration);
                    var roadieDataFilenameByRelease = $"{releaseData.Artist.Text.ToFileNameFriendly()}.";
                    if (filesAtlsGroupedByRelease.Count() == 1)
                    {
                        roadieDataFilenameByRelease = null;
                    }
                    var roadieDataFileName = Path.Combine(dir, $"ted.data.{roadieDataFilenameByRelease}json");
                    System.IO.File.WriteAllText(roadieDataFileName, JsonSerializer.Serialize(releaseData));
                    return releaseData;
                }

            }
            return release;
        }
    }
}
