using Microsoft.VisualBasic;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.RegularExpressions;
using TED.Enums;
using TED.Extensions;
using TED.Models;
using TED.Models.MetaData;
using TED.Utility;

namespace TED.Processors
{
    public sealed class DirectoryProcessor
    {

        private static readonly Regex _hasFeatureFragmentsRegex = new(@"\((ft.|feat.|featuring|feature)+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _unwantedReleaseTitleTextRegex = new(@"(\\s*(-\\s)*((CD[_\-#\s]*[0-9]*)))|((\\(|\\[)+([0-9]|,|self|bonus|re(leas|master|(e|d)*)*|th|anniversary|cd|disc|deluxe|dig(ipack)*|vinyl|japan(ese)*|asian|remastered|limited|ltd|expanded|edition|\\s)+(]|\\)*))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _unwantedTrackTitleTextRegex = new(@"^([0-9]+)(\.|-|\s)*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static string ImageNotFound = @"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";

        public DirectoryProcessor()
        {
        }

        public async Task<Release?> ProcessAsync(DateTime now, string dir, string[] filesInDirectory)
        {
            var allfileAtlsFound = new List<ATL.Track>();
            string? directorySFVFile = null;
            string? directoryM3UFile = null;
            var release = new Release();
            release.Directory = dir;

            try
            {
                foreach (var file in filesInDirectory)
                {
                    var fileAtl = new ATL.Track(file);
                    if (fileAtl != null)
                    {
                        allfileAtlsFound.Add(fileAtl);
                    }
                    if (string.Equals(Path.GetExtension(file), ".sfv", StringComparison.OrdinalIgnoreCase))
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
                            releaseData.ProcessingMessages.Add(ProcessMessage.MakeInfoMessage("Set ArtistThumbnail to ID3 found picture."));
                        }
                        else
                        {
                            var artistThumbnailData = await FirstArtistImageInDirectory(dir, filesInDirectory);
                            releaseData.ArtistThumbnail = artistThumbnailData.Item1;
                            releaseData.ProcessingMessages.Add(new ProcessMessage($"Found [{ artistThumbnailData.Item2 }] number of Artist images.", artistThumbnailData.Item2 > 0, artistThumbnailData.Item2 > 0 ? ProcessMessage.OkCheckMark : ProcessMessage.Warning));
                        }
                        var firstAtlHasReleaseImage = firstAtl.EmbeddedPictures?.FirstOrDefault(x => x.PicType == ATL.PictureInfo.PIC_TYPE.Front ||
                                                                                                     x.PicType == ATL.PictureInfo.PIC_TYPE.Generic);
                        if (firstAtlHasReleaseImage != null)
                        {
                            releaseData.CoverImage = new Models.Image
                            {
                                Bytes = firstAtlHasReleaseImage.PictureData,
                                Caption = firstAtlHasReleaseImage.Description
                            };
                            releaseData.ProcessingMessages.Add(ProcessMessage.MakeInfoMessage("Set CoverImage to ID3 found picture."));
                        }
                        else
                        {
                            var releaseCoverImageData = await FirstReleaseImageInDirectory(dir, filesInDirectory);
                            releaseData.CoverImage = releaseCoverImageData.Item1;
                            releaseData.ProcessingMessages.Add(new ProcessMessage($"Found [{releaseCoverImageData.Item2}] number of Release images.", releaseCoverImageData.Item2 > 0, releaseCoverImageData.Item2 > 0 ? ProcessMessage.OkCheckMark : ProcessMessage.Warning));
                        }
                        if (releaseData.CoverImage == null)
                        {
                            releaseData.CoverImage = new Models.Image
                            {
                                Bytes = Convert.FromBase64String(ImageNotFound)
                            };
                            releaseData.Status = Statuses.Incomplete;
                            releaseData.ProcessingMessages.Add(new ProcessMessage("CoverImage not found.", false, ProcessMessage.BadCheckMark));
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
                        if (releaseData.Status == Enums.Statuses.Ok && directorySFVFile.Nullify() != null)
                        {
                            var doesTrackCountMatchSFVCount = releaseData.TrackCount == await GetMp3CountFromSFVFile(directorySFVFile);
                            if (!doesTrackCountMatchSFVCount)
                            {
                                releaseData.Status = Enums.Statuses.Incomplete;
                            }
                            releaseData.ProcessingMessages.Add(new ProcessMessage("Checked TrackCount with SFV", doesTrackCountMatchSFVCount, doesTrackCountMatchSFVCount ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark));
                        }
                        if (releaseData.Status == Enums.Statuses.Ok && directoryM3UFile.Nullify() != null)
                        {
                            var doesTrackCountMatchM3UCount = releaseData.TrackCount == await GetMp3CountFromM3UFile(directoryM3UFile);
                            if (!doesTrackCountMatchM3UCount)
                            {
                                releaseData.Status = Enums.Statuses.Incomplete;
                            }
                            releaseData.ProcessingMessages.Add(new ProcessMessage("Checked TrackCount with M3U", doesTrackCountMatchM3UCount, doesTrackCountMatchM3UCount ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark));
                        }
                        foreach (var media in releaseData.Media.OrderBy(x => x.MediaNumber).Select((v, i) => new { i, v }))
                        {
                            foreach (var track in media.v.Tracks.OrderBy(x => x.TrackNumber).Select((t, i) => new { i, t }))
                            {
                                var trackStatusCheckData = CheckTrackStatus(track.t);
                                track.t.Status = trackStatusCheckData.Item1;
                                if (track.t.TrackNumber != track.i + 1)
                                {
                                    track.t.Status = Statuses.NeedsAttention;
                                    releaseData.ProcessingMessages.Add(new ProcessMessage ($"Track [{ track.t.ToString() }] TrackNumber expected [{ track.i + 1 }] found [{ track.t.TrackNumber }]", false, ProcessMessage.BadCheckMark));
                                }
                                if (trackStatusCheckData.Item2 != null)
                                {
                                    releaseData.ProcessingMessages.AddRange(trackStatusCheckData.Item2);
                                }
                            }
                            if (media.v.MediaNumber != media.i + 1)
                            {
                                releaseData.Status = Statuses.NeedsAttention;
                                releaseData.ProcessingMessages.Add(new ProcessMessage($"Media [{media.v.ToString()}] MediaNumber expected [{media.i + 1}] found [{media.v.MediaNumber}]", false, ProcessMessage.BadCheckMark));
                            }
                        }
                        releaseData.Status = releaseData.Media.SelectMany(x => x.Tracks).Any(x => x.Status != Statuses.New) ? Statuses.NeedsAttention : releaseData.Status;
                        if (releaseData.Status == Statuses.Ok)
                        {
                            var releaseStatusCheckData = CheckReleaseStatus(releaseData);
                            releaseData.Status = releaseStatusCheckData.Item1;
                            if(releaseStatusCheckData.Item2 != null)
                            {
                                releaseData.ProcessingMessages.AddRange(releaseStatusCheckData.Item2);
                            }
                        }
                        var roadieDataFileName = Path.Combine(dir, $"ted.data.json");
                        System.IO.File.WriteAllText(roadieDataFileName, JsonSerializer.Serialize(releaseData));

                        releaseData.ProcessingMessages.Add(ProcessMessage.MakeInfoMessage($"Creation Date [{releaseData.CreatedDate}]"));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                "Duration is acceptable.",
                                releaseData.Duration > 0,
                                releaseData.Duration > 0 ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                "Media count is acceptable",
                                releaseData.MediaCount > 0,
                                releaseData.MediaCount > 0 ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                $"Release is { (releaseData.IsValid ? "valid" : "invalid") }",
                                releaseData.IsValid,
                                releaseData.IsValid ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                $"Release {(releaseData.Status != Statuses.NeedsAttention || releaseData.Status == Statuses.Ok ? "does not " : "does")} need editing",
                                releaseData.Status != Statuses.NeedsAttention || releaseData.Status == Statuses.Ok,
                                releaseData.Status != Statuses.NeedsAttention || releaseData.Status == Statuses.Ok ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                $"Status is {(releaseData.Status == Statuses.Ok ? "acceptable" : "not acceptable")}",
                                releaseData.Status == Statuses.Ok,
                                releaseData.Status == Statuses.Ok ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                "Track count is acceptable",
                                releaseData.TrackCount > 0,
                                releaseData.TrackCount > 0 ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                "Year is acceptable",
                                releaseData.Year > 0,
                                releaseData.Year > 0 ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));

                        return releaseData;
                    }
                }

            }
            catch (Exception ex)
            {
                release.ProcessingMessages.Add(new ProcessMessage(ex));
            }
            return release;
        }

        private static async Task<(Image?, int, int)> FirstArtistImageInDirectory(string dir, string[] filesInDirectory)
        {
            var artistImagesInFolder = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.Artist, SearchOption.TopDirectoryOnly);
            if (artistImagesInFolder?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(artistImagesInFolder.First().FullName)
                }, artistImagesInFolder.Count(), 0);
            }
            var secondaryArtistImagesInFolder = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.ArtistSecondary, SearchOption.TopDirectoryOnly);
            if (secondaryArtistImagesInFolder?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(secondaryArtistImagesInFolder.First().FullName)
                }, 0, secondaryArtistImagesInFolder.Count());
            }
            return (null, 0, 0);
        }

        private static async Task<(Image?, int, int)> FirstReleaseImageInDirectory(string dir, string[] filesInDirectory)
        {
            var releasetImagesInFolder = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.Release, SearchOption.TopDirectoryOnly);
            if (releasetImagesInFolder?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(releasetImagesInFolder.First().FullName)
                }, releasetImagesInFolder.Count(), 0);
            }
            var secondaryReleaseImagesInFolder = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.ReleaseSecondary, SearchOption.TopDirectoryOnly);
            if (secondaryReleaseImagesInFolder?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(secondaryReleaseImagesInFolder.First().FullName)
                }, 0, secondaryReleaseImagesInFolder.Count());
            }
            return (null, 0, 0);
        }

        private static (Statuses, IEnumerable<ProcessMessage>?) CheckReleaseStatus(Release release)
        {
            if (ReleaseHasUnwantedText(release?.ReleaseData?.Text ?? string.Empty))
            {
                return (Statuses.NeedsAttention, new List<ProcessMessage> { ProcessMessage.MakeBadMessage($"Release [{ release }] Title has unwanted text.") });
            }
            return (release?.Status ?? Statuses.NeedsAttention, null);
        }

        private static (Statuses, IEnumerable<ProcessMessage>?) CheckTrackStatus(Track track)
        {
            if (TrackHasFeaturingFragments(track?.Title ?? string.Empty) || TrackHasUnwantedText(track?.Title ?? string.Empty))
            {
                return (Statuses.NeedsAttention, new List<ProcessMessage> { ProcessMessage.MakeBadMessage($"Track [{ track }] Title has unwanted text.") });
            }
            return (track?.Status ?? Statuses.Missing, null);
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

        private static async Task<int> GetMp3CountFromM3UFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return 0;
            }
            var result = 0;
            foreach (var line in await File.ReadAllLinesAsync(filePath))
            {
                if (IsLineForFileForTrack(line))
                {
                    result++;
                }
            }
            return result;
        }

        private static async Task<int> GetMp3CountFromSFVFile(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return 0;
            }
            var result = 0;
            foreach (var line in await File.ReadAllLinesAsync(filePath))
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
