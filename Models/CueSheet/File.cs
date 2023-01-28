using System.Collections.Generic;
using TED.Models.CueSheet;


namespace TED.Models.CueSheet
{
    public class File
    {
        readonly List<Track> _tracks = new List<Track>();

        public string FileName { get; set; }

        public string FileType { get; set; }

        public List<Track> Tracks
        {
            get { return _tracks; }
        }
    }
}
