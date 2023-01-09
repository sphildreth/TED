using TED.Models.MetaData;

namespace TED.Processors
{
    public sealed class ReleaseSaveProcessor
    {
        public ReleaseSaveProcessor()
        {
        }

        public async Task<(bool, IEnumerable<string>?)> ProcessAsync(DateTime now, Release release)
        {
            var errorMessages = new List<string>();
            var releaseDirectory = release.Directory ?? throw new Exception("Invalid directory on Release");

            try
            {
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
                await Parallel.ForEachAsync(filesInDirectory, async (file, cancellationTokenfile) =>
                {
                    var fullPathToFile = Path.Combine(releaseDirectory, file);
                    var fileAtl = new ATL.Track(file);
                    if (fileAtl != null)
                    {
                        var trackForFile = release.Media.SelectMany(x => x.Tracks).FirstOrDefault(x => string.Equals(x.FileName, fullPathToFile, StringComparison.OrdinalIgnoreCase));
                        fileAtl.AlbumArtist = releaseArtist;
                        fileAtl.Album = release.ReleaseData?.Text;
                        fileAtl.TrackNumber = trackForFile.TrackNumber;
                        fileAtl.TrackTotal = release.Media.FirstOrDefault(x => x.TrackById(trackForFile.Id) != null)?.TrackCount;
                        fileAtl.Title = trackForFile.Title;
                        fileAtl.Year = release.ReleaseDateDateTime?.Year ?? throw new Exception("Invalid Release year");
                        var trackArtist = trackForFile.TrackArtist?.ArtistData?.Text;
                        if (trackArtist != null && !string.Equals(trackArtist, releaseArtist, StringComparison.OrdinalIgnoreCase))
                        {
                            fileAtl.Artist = trackArtist;
                        }
                        if (!fileAtl.Save())
                        {
                            errorMessages.Add($"Unable to update [{trackForFile}]");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message);
            }

            var roadieDataFileName = Path.Combine(releaseDirectory, $"ted.data.json");
            if (File.Exists(roadieDataFileName))
            {
                File.Delete(roadieDataFileName);
            }
            return (!errorMessages.Any(), errorMessages);
        }
    }
}
