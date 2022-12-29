using System.ComponentModel.DataAnnotations;
using NodaTime;
using TED.Utility;

namespace TED.Models.MetaData
{
    [Serializable]
    public abstract class MetaDataBase
    {
        public MetaDataBase(IRandomNumber? randomNumber, IClock? clock)
        {
            RandomSortId = randomNumber?.Next() ?? 0;
            CreatedDate = clock?.GetCurrentInstant().ToDateTimeUtc() ?? DateTime.MinValue;
            Id = Guid.NewGuid();
        }

        public DateTime? CreatedDate { get; set; }

        [Required]
        public Guid Id { get; set; }

        public DateTime? LastUpdated { get; set; }

        public int RandomSortId { get; set; }

        [MaxLength(250)]
        public virtual string? SortName { get; set; }


    }
}