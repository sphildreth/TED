﻿using TED.Utility;

namespace TED.Models.CueSheet.Parsers
{
    public class FileLineTrackParser : IParser<Track>
    {
        private readonly FileLine _line;

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
                TrackNum = SafeParser.ToNumber<int>(_line.RawParts[1]),
                TrackType = _line.RawParts[2]
            };
        }
    }
}
