namespace TED.Models.CueSheet
{
    public class CueSheet
    {
        private readonly List<Tuple<string, string>> _comments = new List<Tuple<string, string>>();

        private readonly List<File> _files = new List<File>();

        public string Catalog { get; set; }

        public string CdTextFile { get; set; }

        public List<Tuple<string, string>> Comments
        {
            get { return _comments; }
        }

        public List<File> Files
        {
            get { return _files; }
        }

        public string Performer { get; set; }

        public string SongWriter { get; set; }

        public string Title { get; set; }

        public bool IsStandard { get; set; }

        public bool IsNoncompliant { get; set; }

        public string Genre { get; internal set; }

        public string Date { get; internal set; }

        public string DiscId { get; internal set; }

        public int? DiscNumber { get; internal set; }

        public int? DiscTotal { get; internal set; }

        public bool TryGetCommentValue(string name, out string value)
        {
            value = null;

            foreach (var comment in _comments.Where(comment => comment.Item1 == name))
            {
                value = comment.Item2;
                return true;
            }

            return false;
        }

        public static Track Next(IList<Track> tracks, Track currentTrack)
        {
            int indexOf = tracks.IndexOf(currentTrack);

            return (indexOf + 1 < tracks.Count) ? tracks[indexOf + 1] : null;
        }
    }
}
