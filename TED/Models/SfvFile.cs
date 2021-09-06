using System.Collections.Generic;
using System.Linq;

namespace TED.Models
{
    public sealed class SfvFile
    {
        public bool IsValid => !string.IsNullOrEmpty(Name) && Entries.Any();

        public string Name { get; set; }

        public IEnumerable<SfvFileEntry> Entries { get; set; } = Enumerable.Empty<SfvFileEntry>();
    }
}