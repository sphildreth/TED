using TED.Utility;

namespace TED.Models.CueSheet.Parsers
{
    public class FileLineIndexParser : IParser<Index>
    {
        private readonly FileLine _line;

        public FileLineIndexParser(FileLine line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            _line = line;
        }

        public Index Parse()
        {
            return new Index
            {
                IndexNum = SafeParser.ToNumber<byte>(_line.RawParts[1]),
                IndexTime = new IndexTime(_line.RawParts.Last())
            };
        }
    }
}
