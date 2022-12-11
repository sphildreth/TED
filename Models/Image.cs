using System.ComponentModel.DataAnnotations;

namespace Roadie.Models 
{
    [Serializable]
    public sealed class Image 
    {
        public byte[]? Bytes { get; } 

        [MaxLength(100)]
        public string? Caption { get; }

        public string? ThumbnailUrl { get; }

        public string? Url { get; }

        public Image()
        {
        }

        /// <summary>
        ///     Set image Url to given value and nullify other entity values, intended to be used in List collection (like images for an artist)
        /// </summary>
        public Image(string url)
            : this(url, null, null)
        {
        }

        public Image(byte[] bytes)
            : this(null, null, null, bytes)
        {
        }

        public Image(string? url, string? caption, string? thumbnailUrl, byte[]? bytes = null)
        {
            Bytes = bytes;
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            Caption = caption;
        }
    }
}