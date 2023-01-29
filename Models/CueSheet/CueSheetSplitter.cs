using System.Collections.Concurrent;

namespace TED.Models.CueSheet
{
    public class CueSheetSplitter
    {
        private readonly CueSheet _cueSheet;

        private readonly string _cueFilePath;

        private readonly Func<string, string, Index, Index, Task<bool>> _buildArgsFunc;

        public CueSheetSplitter(CueSheet cueSheet, string cueFilePath, Func<string, string, Index, Index, Task<bool>> buildArgsFunc)
        {
            if (cueSheet == null)
                throw new ArgumentNullException("cueSheet");

            if (string.IsNullOrWhiteSpace(cueFilePath))
                throw new ArgumentNullException("cueFilePath");

            if (buildArgsFunc == null)
                throw new ArgumentNullException("buildArgsFunc");

            _cueSheet = cueSheet;
            _cueFilePath = cueFilePath;
            _buildArgsFunc = buildArgsFunc;
        }

        public async Task<IEnumerable<SplitResult>> Split()
        {
            var results = new ConcurrentBag<SplitResult>();

            List<Track> tracks = _cueSheet.IsStandard
                ? _cueSheet.Files[0].Tracks
                : _cueSheet.Files.Select(file => file.Tracks[0]).ToList();

            await Parallel.ForEachAsync(tracks.OrderBy(x => x.TrackNum), async (track, cancelationToken) =>
            {
                string tempWavPath = Path.Combine(
                    Path.GetDirectoryName(_cueFilePath),
                    string.Format("{0}-{1}.mp3", track.TrackNum, Guid.NewGuid().ToString("N")));

                Index skip = track.FindIndexByNumber(1);

                if (skip == null)
                {
                    throw new Exception(string.Format("Index 1 not found in cue sheet for track '{0}'.", track.Title));
                }
                Index until = null;

                if (_cueSheet.IsStandard)
                {
                    Track nextTrack = CueSheet.Next(tracks, track);

                    if (nextTrack != null)
                    {
                        until = nextTrack.FindIndexByNumber(1);

                        if (until == null)
                        {
                            throw new Exception(string.Format("Index 1 not found in cue sheet for track '{0}'.", nextTrack.Title));
                        }
                    }
                }

                string fileName = _cueSheet.IsStandard ? _cueSheet.Files[0].FileName : _cueSheet.Files[tracks.FindIndex(x => x.TrackNum == track.TrackNum)].FileName;
                string filePath;
                if (Path.IsPathRooted(fileName))
                {
                    filePath = fileName;
                }
                else
                {
                    string cueDirName = Path.GetDirectoryName(_cueFilePath);

                    if (cueDirName == null)
                    {
                        throw new Exception("Cue file directory is null.");
                    }
                    filePath = Path.Combine(cueDirName, fileName);
                }

                await _buildArgsFunc(filePath, tempWavPath, skip, until);
                results.Add(new SplitResult(track, tempWavPath));
            });
            return results;
        }
    }
}
