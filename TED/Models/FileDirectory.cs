using System;

namespace TED.Models
{
    public sealed class FileDirectory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public long NumberOfMediaFound { get; set; }
        public bool HasTagData { get; set; }
        public bool IsSelected { get; set; }
    }
}