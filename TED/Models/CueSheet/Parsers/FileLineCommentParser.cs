namespace TED.Models.CueSheet.Parsers
{
    public class FileLineCommentParser : IParser<Tuple<string, string>>
    {
        private readonly FileLine _line;

        public FileLineCommentParser(FileLine line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            _line = line;
        }

        public Tuple<string, string> Parse()
        {
            string name = _line.RawParts[1];

            int valStart = (_line.Command + " " + name + " ").Length;
            string value = _line.Line.Substring(valStart).Trim(' ', '"');

            return Tuple.Create(name, value);
        }
    }
}
