using System.ComponentModel.DataAnnotations;

namespace TED.Models 
{
    [Serializable]
    public sealed class Image 
    {
        public byte[]? Bytes { get; set; } 

        [MaxLength(100)]
        public string? Caption { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? Url { get; set; }

        public Image()
        {
        }
    }
}