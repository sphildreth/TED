﻿namespace TED.Models.CueSheet
{
    public class Track
    {
        private readonly List<Index> _indexes = new List<Index>();

        public string Flags { get; set; }

        public List<Index> Indexes
        {
            get { return _indexes; }
        }

        public string Isrc { get; set; }

        public string Performer { get; set; }

        public Index PostGap { get; set; }

        public Index PreGap { get; set; }

        public string SongWriter { get; set; }

        public string Title { get; set; }

        public int TrackNum { get; set; }

        public string TrackType { get; set; }

        public Index FindIndexByNumber(int indexNumber)
        {
            return (Indexes.Where(index => index.IndexNum == indexNumber)).FirstOrDefault();
        }
    }
}
