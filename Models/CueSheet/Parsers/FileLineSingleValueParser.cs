using System;
using TED.Models.CueSheet;
using TED.Models.CueSheet.Parsers;


namespace TED.Models.CueSheet.Parsers
{
    public class FileLineSingleValueParser : IParser<string>
    {
        readonly FileLine _line;

        public FileLineSingleValueParser(FileLine line)
        {
            if (line == null) 
                throw new ArgumentNullException("line");

            _line = line;
        }

        public string Parse()
        {
            int valStart = (_line.Command + " ").Length;

            return _line.Line.Substring(valStart).Trim(' ', '"');
        }
    }
}
