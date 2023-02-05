using System.Collections.ObjectModel;

namespace TED.Models.CueSheet
{
    public class FileLine
    {
        private readonly string _command;

        private readonly string _line;

        private readonly ReadOnlyCollection<string> _rawParts;

        public FileLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentNullException("line");

            _line = line.Trim();
            _rawParts = Array.AsReadOnly(_line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            _command = _rawParts[0].ToUpperInvariant();
            if (string.Equals(_command, "REM"))
            {
                _command = $"{_rawParts[0].ToUpperInvariant()} {_rawParts[1].ToUpperInvariant()}";
            }
        }

        public string Command
        {
            get { return _command; }
        }

        public string Line
        {
            get { return _line; }
        }

        public ReadOnlyCollection<string> RawParts
        {
            get { return _rawParts; }
        }
    }
}
