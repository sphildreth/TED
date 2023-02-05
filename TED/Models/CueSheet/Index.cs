namespace TED.Models.CueSheet
{
    public class Index
    {
        private byte _indexNum;

        public byte IndexNum
        {
            get { return _indexNum; }
            set
            {
                if (!(value <= 99))
                    throw new Exception("Index number must be between 0 and 99.");

                _indexNum = value;
            }
        }

        public IndexTime IndexTime { get; set; }
    }
}
