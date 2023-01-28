namespace TED.Models.CueSheet
{
    public class SplitResult
    {
        private readonly Track _track;

        private readonly string _filePath;

        public SplitResult(Track track, string filePath)
        {
            if (track == null)
            {
                throw new ArgumentNullException("track");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException("filePath");
            }
            _track = track;
            _filePath = filePath;
        }

        public Track Track => _track;

        public string FilePath => _filePath;
    }
}
