using System;
using TED.Models.CueSheet;
using TED.Models.CueSheet.Parsers;


namespace TED.Models.CueSheet.Parsers
{
    public class FileLineTrackParser : IParser<Track>
    {
        readonly FileLine _line;

        public FileLineTrackParser(FileLine line)
        {
            if (line == null) 
                throw new ArgumentNullException("line");

            _line = line;
        }

        public Track Parse()
        {
            return new Track
            {
                TrackNum = int.Parse(_line.RawParts[1]), 
                TrackType = _line.RawParts[2]
            };
        }
    }
}
