using System;

namespace TED.Models.TagData
{
    public class Media
    {
        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string Title { get; set; }

        public bool IsTitleValid => !string.IsNullOrEmpty(Title);

        public short TrackNumber { get; set; }

        public double Length { get; set; }

        public string LengthFormatted => Length == 0 ? null : TimeSpan.FromSeconds(Length).ToString(TagData.TimeSpanFormat);

        public bool IsValid => TrackNumber > 0 && FileSize > 0 && IsTitleValid;
    }
}