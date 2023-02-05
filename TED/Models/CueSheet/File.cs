namespace TED.Models.CueSheet
{
    public class File
    {
        private readonly List<Track> _tracks = new List<Track>();

        public string FileName { get; set; }

        public string FileType { get; set; }

        public List<Track> Tracks
        {
            get { return _tracks; }
        }
    }
}
