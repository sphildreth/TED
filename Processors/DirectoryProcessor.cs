using FFMpegCore;
using System.Collections.Concurrent;
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
        private static readonly Regex _hasFeatureFragmentsRegex = new(@"(ft.|feat.|featuring)+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _unwantedReleaseTitleTextRegex = new(@"(\s*(-\s)*((CD[_\-#\s]*[0-9]*)))|((,|self|bonus|release|remaster|remastered|anniversary|cd|disc|deluxe|digipak|digipack|vinyl|japan(ese)*|asian|remastered|limited|ltd|expanded|edition|web)+(]|\)*))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _unwantedTrackTitleTextRegex = new(@"(\s{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static string ImageNotFound = @"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";

        public DirectoryProcessor()
        {
        }

        public async Task<Release?> ProcessAsync(DateTime now, string? dir, string[] filesInDirectory)
        {
            await CheckIfDirectoryHasMultipleReleases(now, dir);

            var allfileAtlsFound = new List<ATL.Track>();
            string? directorySFVFile = null;
            string? directoryM3UFile = null;
            var release = new Release();
            release.Directory = dir;
            if (string.IsNullOrEmpty(dir))
            {
                release.ProcessingMessages.Add(ProcessMessage.MakeBadMessage("Invalid dir"));
                release.Status = Statuses.NoMediaFiles;
                return release;
            }
            if (!filesInDirectory.Any())
            {
                release.ProcessingMessages.Add(ProcessMessage.MakeBadMessage("No files found in dir"));
                release.Status = Statuses.NoMediaFiles;
                return release;
            }
            try
            {
                foreach (var subDir in Directory.GetDirectories(dir, "*.*", SearchOption.AllDirectories))
                {
                    ProcessSubDirectory(dir, new DirectoryInfo(subDir));
                }

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
                        var isFirstM3uFileInDirectory = string.IsNullOrEmpty(directoryM3UFile);
                        if (isFirstM3uFileInDirectory)
                        {
                            directoryM3UFile = file;
                        }
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
                                Text = (firstAtl.AlbumArtist.Nullify() ?? firstAtl.Artist).CleanString()
                            },
                            ReleaseData = new Models.DataToken
                            {
                                Value = SafeParser.ToToken(groupedByRelease.Key),
                                Text = groupedByRelease.Key.CleanString()
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
                            releaseData.ProcessingMessages.Add(new ProcessMessage($"Found [{artistThumbnailData.Item2}] number of Artist images.", artistThumbnailData.Item2 > 0, artistThumbnailData.Item2 > 0 ? ProcessMessage.OkCheckMark : ProcessMessage.Warning));
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
                            releaseData.Status = releaseData.Status == Statuses.Reviewed ? Statuses.Reviewed : Statuses.Incomplete;
                            releaseData.ProcessingMessages.Add(new ProcessMessage("CoverImage not found.", false, ProcessMessage.BadCheckMark));
                        }
                        var doesReleaseHaveNonMp3Tracks = tagsFilesFound.Any(x => !string.Equals(x.AudioFormat.ShortName, "mpeg", StringComparison.OrdinalIgnoreCase));
                        if (doesReleaseHaveNonMp3Tracks)
                        {
                            tagsFilesFound = await ConvertToMp3(dir, tagsFilesFound);
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
                                    Title = x.Title.CleanString(),
                                    TrackArtist = new Artist
                                    {
                                        ArtistData = new Models.DataToken
                                        {
                                            Value = SafeParser.ToToken(StringExt.DoStringsMatch(releaseData.Artist.Text, x.Artist) ? string.Empty : x.Artist),
                                            Text = StringExt.DoStringsMatch(releaseData.Artist.Text, x.Artist) ? string.Empty : x.Artist
                                        }
                                    },
                                    TrackNumber = x.TrackNumber
                                }).ToArray()
                            });
                        }
                        releaseData.Media = medias;
                        releaseData.TrackCount = medias.Sum(x => x.TrackCount);
                        releaseData.Status = releaseData.Status == Statuses.Reviewed ? Statuses.Reviewed : releaseData.Media.SelectMany(x => x.Tracks).Count() == releaseData.Media.Sum(x => x.TrackCount) ? Enums.Statuses.Ok : Enums.Statuses.Incomplete;
                        releaseData.Duration = medias.SelectMany(x => x.Tracks).Sum(x => x.Duration);
                        if (releaseData.Status == Enums.Statuses.Ok && directorySFVFile.Nullify() != null)
                        {
                            var mp3CountFromSFVFile = await GetMp3CountFromSFVFile(directorySFVFile);
                            var doesTrackCountMatchSFVCount = releaseData.TrackCount == mp3CountFromSFVFile;
                            if (!doesTrackCountMatchSFVCount)
                            {
                                releaseData.Status = Enums.Statuses.Incomplete;
                            }
                            releaseData.ProcessingMessages.Add(new ProcessMessage($"Checked TrackCount with SFV expected [{mp3CountFromSFVFile}] found [{releaseData.TrackCount}]", doesTrackCountMatchSFVCount, doesTrackCountMatchSFVCount ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark));
                        }
                        if (releaseData.Status == Enums.Statuses.Ok && directoryM3UFile.Nullify() != null)
                        {
                            var mp3CountFromM3UFile = await GetMp3CountFromM3UFile(directoryM3UFile);
                            var doesTrackCountMatchM3UCount = releaseData.TrackCount == mp3CountFromM3UFile;
                            if (!doesTrackCountMatchM3UCount)
                            {
                                releaseData.Status = Enums.Statuses.Incomplete;
                            }
                            releaseData.ProcessingMessages.Add(new ProcessMessage($"Checked TrackCount with M3U expected [{mp3CountFromM3UFile}] found [{releaseData.TrackCount}]", doesTrackCountMatchM3UCount, doesTrackCountMatchM3UCount ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark));
                        }
                        if (releaseData.Status == Statuses.Ok)
                        {
                            var doAllTracksHaveSameAlbumArtist = AllTracksForHaveSameArtist(releaseData.Artist.Text, tagsFilesFound);
                            if (!doAllTracksHaveSameAlbumArtist)
                            {
                                releaseData.Status = Enums.Statuses.NeedsAttention;
                                releaseData.ProcessingMessages.Add(ProcessMessage.MakeBadMessage($"Tracks have different Album Artists [{tagsFilesFound.GroupBy(x => x.Artist).Select(x => x.Key).ToCsv()}]"));
                            }
                        }
                        if (releaseData.Status == Statuses.Ok)
                        {
                            if (StringHasFeaturingFragments(releaseData.Artist.Text))
                            {
                                releaseData.Status = Enums.Statuses.NeedsAttention;
                                releaseData.ProcessingMessages.Add(ProcessMessage.MakeBadMessage($"Release Artist has featuring fragments"));
                            }
                        }
                        foreach (var media in releaseData.Media.OrderBy(x => x.MediaNumber).Select((v, i) => new { i, v }))
                        {
                            foreach (var track in media.v.Tracks.OrderBy(x => x.TrackNumber).Select((t, i) => new { i, t }))
                            {
                                var trackStatusCheckData = CheckTrackStatus(releaseData, track.t);
                                track.t.Status = trackStatusCheckData.Item1;
                                if (track.t.TrackNumber != track.i + 1)
                                {
                                    track.t.Status = Statuses.NeedsAttention;
                                    releaseData.ProcessingMessages.Add(new ProcessMessage($"Track [{track.t.ToString()}] TrackNumber expected [{track.i + 1}] found [{track.t.TrackNumber}]", false, ProcessMessage.BadCheckMark));
                                }
                                if (trackStatusCheckData.Item2 != null)
                                {
                                    releaseData.ProcessingMessages.AddRange(trackStatusCheckData.Item2);
                                }
                                if (track?.t?.TrackArtist?.ArtistData?.Text != null && StringHasFeaturingFragments(track?.t?.TrackArtist?.ArtistData?.Text))
                                {
                                    track.t.Status = Statuses.NeedsAttention;
                                    releaseData.ProcessingMessages.Add(new ProcessMessage($"Track [{track.t.ToString()}] TrackArtist has featuring fragments", false, ProcessMessage.BadCheckMark));
                                }
                            }
                            if (releaseData.Status != Statuses.Reviewed && media.v.MediaNumber != media.i + 1)
                            {
                                releaseData.Status = Statuses.NeedsAttention;
                                releaseData.ProcessingMessages.Add(new ProcessMessage($"Media [{media.v.ToString()}] MediaNumber expected [{media.i + 1}] found [{media.v.MediaNumber}]", false, ProcessMessage.BadCheckMark));
                            }
                        }
                        releaseData.Status = releaseData.Status == Statuses.Reviewed ? Statuses.Reviewed : releaseData.Media.SelectMany(x => x.Tracks).Any(x => x.Status != Statuses.New) ? Statuses.NeedsAttention : releaseData.Status;
                        if (releaseData.Status == Statuses.Ok)
                        {
                            var releaseStatusCheckData = CheckReleaseStatus(releaseData);
                            releaseData.Status = releaseStatusCheckData.Item1;
                            if (releaseStatusCheckData.Item2 != null)
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
                                $"Release is {(releaseData.IsValid ? "valid" : "invalid")}",
                                releaseData.IsValid,
                                releaseData.IsValid ? ProcessMessage.OkCheckMark : ProcessMessage.BadCheckMark
                            ));
                        releaseData.ProcessingMessages.Add(new ProcessMessage
                            (
                                $"Release {(releaseData.Status != Statuses.NeedsAttention || releaseData.Status == Statuses.Ok ? "does not require" : "requires")} editing",
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
                        if (releaseData.ArtistThumbnail != null)
                        {
                            await File.WriteAllBytesAsync(Path.Combine(releaseData.Directory, "artist.jpg"), releaseData.ArtistThumbnail.Bytes);
                            releaseData.ProcessingMessages.Add(ProcessMessage.MakeInfoMessage("Updated Artist Thumbnail image file."));
                        }
                        if (releaseData.CoverImage != null)
                        {
                            await File.WriteAllBytesAsync(Path.Combine(releaseData.Directory, "cover.jpg"), releaseData.CoverImage.Bytes);
                            releaseData.ProcessingMessages.Add(ProcessMessage.MakeInfoMessage("Updated Release Cover image file."));
                        }
                        return releaseData;
                    }
                }
                else
                {
                    release.ProcessingMessages.Add(ProcessMessage.MakeBadMessage("No Media files found in dir"));
                    release.Status = Statuses.NoMediaFiles;
                    return release;
                }
            }
            catch (Exception ex)
            {
                release.ProcessingMessages.Add(new ProcessMessage(ex));
            }
            return release;
        }

        public async Task CheckIfDirectoryHasMultipleReleases(DateTime now, string? dir)
        {
            var allSFVFilesInReleaseDirectory = Directory.GetFiles(dir, "*.sfv");
            if (allSFVFilesInReleaseDirectory.Count() > 1)
            {
                foreach (var it in allSFVFilesInReleaseDirectory.Skip(1).Select((x, i) => new { Value = x, Index = i }))
                {
                    var sfvFilename = Path.GetFileName(it.Value);
                    var parentDirectory = Directory.GetParent(dir).FullName;
                    var newReleaseDirectory = Path.Combine(parentDirectory, $"{(new DirectoryInfo(dir)).Name} ({it.Index})");
                    if (!Directory.Exists(newReleaseDirectory))
                    {
                        Directory.CreateDirectory(newReleaseDirectory);
                    }
                    var newSfvFilename = Path.Combine(newReleaseDirectory, sfvFilename);
                    File.Move(it.Value, newSfvFilename, true);
                    foreach (var line in await File.ReadAllLinesAsync(newSfvFilename))
                    {
                        if (IsLineForFileForTrack(line))
                        {
                            var fileNameFromLine = Mp3FileNameFromSFVLine(line);
                            var fileNameToMove = Path.Combine(dir, fileNameFromLine);
                            if (File.Exists(fileNameToMove))
                            {
                                File.Move(fileNameToMove, Path.Combine(newReleaseDirectory, fileNameFromLine), true);
                            }
                        }
                    }
                    foreach (var fileNamedLikeSFV in Directory.GetFiles(dir, $"{Path.GetFileNameWithoutExtension(sfvFilename)}*.*"))
                    {
                        File.Move(fileNamedLikeSFV, Path.Combine(newReleaseDirectory, Path.GetFileName(fileNamedLikeSFV)), true);
                    }
                }
            }
        }

        private void ProcessSubDirectory(string releaseDirectory, DirectoryInfo subDirectory)
        {
            var coverImages = (ImageHelper.FindImageTypeInDirectory(subDirectory, ImageType.Release) ?? Enumerable.Empty<FileInfo>()).ToList();
            coverImages.AddRange((ImageHelper.FindImageTypeInDirectory(subDirectory, ImageType.ReleaseSecondary) ?? Enumerable.Empty<FileInfo>()));
            if (coverImages?.Any() ?? false)
            {
                Parallel.ForEach(coverImages, coverImage =>
                {
                    File.Move(coverImage.FullName, Path.Combine(releaseDirectory, coverImage.Name), true);
                });
            }
        }

        private static bool AllTracksForHaveSameArtist(string albumArtist, IEnumerable<ATL.Track> atlTracksForRelease)
        {
            if (atlTracksForRelease.Any())
            {
                return true;
            }
            var tracksGroupedByArtist = atlTracksForRelease.GroupBy(x => x.AlbumArtist);
            return string.Equals(tracksGroupedByArtist.First().Key, albumArtist) && tracksGroupedByArtist.Count() == 1;
        }

        private static async Task<(Image?, int, int)> FirstArtistImageInDirectory(string dir, string[] filesInDirectory)
        {
            var artistImagesInDirectory = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.Artist, SearchOption.TopDirectoryOnly);
            if (artistImagesInDirectory?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(artistImagesInDirectory.First().FullName)
                }, artistImagesInDirectory.Count(), 0);
            }
            var secondaryArtistImagesInDirectory = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.ArtistSecondary, SearchOption.TopDirectoryOnly);
            if (secondaryArtistImagesInDirectory?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(secondaryArtistImagesInDirectory.First().FullName)
                }, 0, secondaryArtistImagesInDirectory.Count());
            }
            return (null, 0, 0);
        }

        private static async Task<(Image?, int, int)> FirstReleaseImageInDirectory(string dir, string[] filesInDirectory)
        {
            var releasetImagesInDirectory = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.Release, SearchOption.TopDirectoryOnly);
            if (releasetImagesInDirectory?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(releasetImagesInDirectory.First().FullName)
                }, releasetImagesInDirectory.Count(), 0);
            }
            var secondaryReleaseImagesInDirectory = ImageHelper.FindImageTypeInDirectory(new DirectoryInfo(dir), ImageType.ReleaseSecondary, SearchOption.TopDirectoryOnly);
            if (secondaryReleaseImagesInDirectory?.Any() ?? false)
            {
                return (new Image
                {
                    Bytes = await File.ReadAllBytesAsync(secondaryReleaseImagesInDirectory.First().FullName)
                }, 0, secondaryReleaseImagesInDirectory.Count());
            }
            return (null, 0, 0);
        }

        private async Task<IEnumerable<ATL.Track>> ConvertToMp3(string dir, IEnumerable<ATL.Track> atlTracks)
        {
            var result = new ConcurrentBag<ATL.Track>();
            await Parallel.ForEachAsync(atlTracks.Where(x => !string.Equals(x.AudioFormat.ShortName, "mpeg", StringComparison.OrdinalIgnoreCase)), async (atlTrack, cancellationTokenfile) =>
            {
                var trackFileInfo = atlTrack.FileInfo();
                var trackDirectory = trackFileInfo?.Directory?.FullName ?? throw new Exception("Invalid FileInfo For Track");
                var newFileName = Path.Combine(trackDirectory, $"{Path.GetFileNameWithoutExtension(trackFileInfo.Name)}.mp3");

                await FFMpegArguments.FromFileInput(trackFileInfo)
                                     .OutputToFile(newFileName, true, options =>
                                     {
                                         options.WithAudioBitrate(FFMpegCore.Enums.AudioQuality.Ultra);
                                         options.WithAudioCodec("mp3").ForceFormat("mp3");
                                     }).ProcessAsynchronously(true);
                var newAtl = new ATL.Track(newFileName);
                if(string.Equals(newAtl.AudioFormat.ShortName, "mpeg", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(newAtl);
                    trackFileInfo.Delete();
                }
                else
                {
                    throw new Exception($"Unable to convert [{trackFileInfo.FullName}] to MP3");
                }
            });
            return result;
        }

        private static (Statuses, IEnumerable<ProcessMessage>?) CheckReleaseStatus(Release release)
        {
            if (ReleaseHasUnwantedText(release?.ReleaseData?.Text ?? string.Empty))
            {
                return (Statuses.NeedsAttention, new List<ProcessMessage> { ProcessMessage.MakeBadMessage($"Release [{release}] Title has unwanted text.") });
            }
            return (release?.Status ?? Statuses.NeedsAttention, null);
        }

        private static (Statuses, IEnumerable<ProcessMessage>?) CheckTrackStatus(Release release, Track track)
        {
            if (release.Status == Statuses.Reviewed)
            {
                return (release.Status, null);
            }
            if (TrackArtistHasReleaseArtist(release, track) || StringHasFeaturingFragments(track?.Title ?? string.Empty) || TrackHasUnwantedText(release?.ReleaseData?.Text ?? string.Empty, track?.Title ?? string.Empty, track.TrackNumber))
            {
                return (Statuses.NeedsAttention, new List<ProcessMessage> { ProcessMessage.MakeBadMessage($"Track [{track}] Title has unwanted text.") });
            }
            return (track?.Status ?? Statuses.Missing, null);
        }

        /// <summary>
        /// When the TrackArtist name contains the Release Artist name (e.g. Release Artist "Bob Dylan" and the Track Artist is "Bob Dylan/Tracey Morgan")
        /// </summary>
        private static bool TrackArtistHasReleaseArtist(Release release, Track track)
        {
            return false;
        }

        public static bool StringHasFeaturingFragments(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            return _hasFeatureFragmentsRegex.IsMatch(input);
        }

        public static bool ReleaseHasUnwantedText(string releaseTitle)
        {
            if (string.IsNullOrWhiteSpace(releaseTitle))
            {
                return true;
            }
            if (_unwantedReleaseTitleTextRegex.IsMatch(releaseTitle))
            {
                return true;
            }
            if (releaseTitle.ContainsUnicodeCharacter())
            {
                return true;
            }
            return false;
        }

        public static bool TrackHasUnwantedText(string releaseTitle, string trackTitle, int? trackNumber)
        {
            if (string.IsNullOrWhiteSpace(trackTitle))
            {
                return true;
            }
            if (_unwantedTrackTitleTextRegex.IsMatch(trackTitle))
            {
                return true;
            }
            if (trackTitle.ContainsUnicodeCharacter())
            {
                return true;
            }
            if (trackTitle.Any(char.IsDigit))
            {
                if (string.Equals(trackTitle.Trim(), (trackNumber ?? 0).ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return Regex.IsMatch(trackTitle, $"^({releaseTitle}\\s*.*\\s*)?([0-9]*{trackNumber}\\s)");
            }
            return false;
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

        public static string? Mp3FileNameFromSFVLine(string? lineFromFile)
        {
            if (string.IsNullOrWhiteSpace(lineFromFile))
            {
                return null;
            }
            var parts = lineFromFile.Split(' ');
            return parts.Take(parts.Length - 1).ToCsv(" ");
        }

        public static bool IsLineForFileForTrack(string lineFromFile)
        {
            if (string.IsNullOrWhiteSpace(lineFromFile))
            {
                return false;
            }
            if (lineFromFile.StartsWith("#"))
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

        public static bool IsDirectoryEmpty(string path) => !Directory.EnumerateFileSystemEntries(path).Any();

        public static void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }
                Directory.Delete(target_dir, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Deleting [{target_dir}] [{ex.Message}]");
            }
        }

        public static void MoveFolder(string sourceDirectory, string destinationDirectory)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            string[] files = Directory.GetFiles(sourceDirectory);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destinationDirectory, name);
                if (string.Equals(name, "ted.data.json", StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(file);
                }
                else
                {
                    File.Move(file, dest, true);
                }
            }
            string[] folders = Directory.GetDirectories(sourceDirectory);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destinationDirectory, name);
                MoveFolder(folder, dest);
            }
            if (IsDirectoryEmpty(sourceDirectory))
            {
                Directory.Delete(sourceDirectory);
            }
        }
    }
}
