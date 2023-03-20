using System.Diagnostics;
using System.Text.Json;
using TED.Extensions;
using TED.Models.MetaData;

namespace TED.Processors
{
    public sealed class ReleaseSaveProcessor
    {
        private readonly ILogger _logger;

        public ReleaseSaveProcessor(ILogger<ReleaseSaveProcessor> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Do processing for Processing and return success status and processing messages.
        /// </summary>
        /// <param name="now">DateTime to use for date marking</param>
        /// <param name="release">Release to Save</param>
        /// <returns>Tuple of processing success and any processing messages</returns>
        /// <exception cref="Exception">Processing exception</exception>
        public async Task<(bool, IEnumerable<string>)> ProcessAsync(DateTime now, Release release)
        {
            var errorMessages = new List<string>();
            var releaseDirectory = release.Directory ?? throw new Exception("Invalid directory on Release");

            try
            {
                var sw = Stopwatch.StartNew();
                if (release.ArtistThumbnail != null)
                {
                    await File.WriteAllBytesAsync(Path.Combine(releaseDirectory, "artist.jpg"), release.ArtistThumbnail.Bytes);
                }
                if (release.CoverImage != null)
                {
                    await File.WriteAllBytesAsync(Path.Combine(releaseDirectory, "cover.jpg"), release.CoverImage.Bytes);
                }
                var filesInDirectory = Directory.GetFiles(releaseDirectory, "*.mp3");
                var releaseArtist = release.Artist?.Text ?? throw new Exception("Invalid Release artist");
                Parallel.ForEach(filesInDirectory, file =>
                {
                    var fullPathToFile = Path.Combine(releaseDirectory, file);
                    var fileAtl = new ATL.Track(file);
                    if (fileAtl != null)
                    {
                        var trackForFile = release.Media.SelectMany(x => x.Tracks).FirstOrDefault(x => string.Equals(x.FileName, fullPathToFile, StringComparison.OrdinalIgnoreCase));
                        if (trackForFile == null)
                        {
                            throw new Exception($"Unable to find Track for Filename [{fullPathToFile}]");
                        }
                        var mediaForFile = release.Media.Where(x => x.Tracks.Any(x => x.Id == trackForFile.Id)).FirstOrDefault();
                        fileAtl.Album = release.ReleaseData?.Text ?? throw new Exception("Invalid Release Title");
                        fileAtl.AlbumArtist = releaseArtist;
                        fileAtl.Comment = string.Empty;
                        fileAtl.DiscNumber = mediaForFile.MediaNumber;
                        fileAtl.DiscTotal = release.Media.Max(x => x.MediaNumber);
                        fileAtl.Title = trackForFile.Title;
                        fileAtl.TrackNumber = trackForFile.TrackNumber;
                        fileAtl.TrackTotal = release.Media.FirstOrDefault(x => x.TrackById(trackForFile.Id) != null)?.TrackCount;
                        fileAtl.Year = release.ReleaseDateDateTime?.Year ?? throw new Exception("Invalid Release year");
                        var trackArtist = trackForFile.TrackArtist?.ArtistData?.Text.Nullify();
                        if (trackArtist != null && !StringExt.DoStringsMatch(releaseArtist, trackArtist))
                        {
                            fileAtl.Artist = trackArtist;
                        }
                        else
                        {
                            fileAtl.Artist = string.Empty;
                        }
                        if (!fileAtl.Save())
                        {
                            errorMessages.Add($"Unable to update [{trackForFile}]");
                        }
                    }
                });
                var roadieDataFileName = Path.Combine(releaseDirectory, $"ted.data.json");
                File.WriteAllText(roadieDataFileName, JsonSerializer.Serialize(release));
                sw.Stop();
                _logger.LogInformation("Saved Release [{ release }] Elapsed Time [{ elapsedTime }]", release.ToString(), sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Saving [{ release }] [{ error}]", release.ToString(), ex.Message);
                errorMessages.Add(ex.Message);
            }
            return (!errorMessages.Any(), errorMessages);
        }
    }
}
