using System.Diagnostics;

namespace TED.Models
{
    /// <summary>
    /// Generic Data "Token" (or List Item) for associations and child lists on objects. Example "Genres" for a Release.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Text [{ Text }]")]
    public sealed class DataToken
    {
        /// <summary>
        /// This is the Text to show to the user (ie name of genre or artist or label)
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// This is the value to submit or the Key (Guid) of the item
        /// </summary>
        public string? Value { get; set; }

        public int RandomSortId { get; set; }

        public DataToken()
        {
        }
    }
}
