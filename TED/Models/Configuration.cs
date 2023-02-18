namespace TED.Models
{
    /// <summary>
    /// Configuration class representing configuration options.
    /// </summary>
    [Serializable]
    public sealed class Configuration
    {    

        public string StagingDirectory { get; set; }

        public string InboundDirectory { get; set; }

        public int StagingDirectoryScanLimit { get; set; } = 250;

        public IEnumerable<string> StagingDirectoryExtensionsToIgnore { get; set; } = Enumerable.Empty<string>();

        public Configuration()
        {
        }
    }
}
