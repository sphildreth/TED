namespace TED.Models.CueSheet.Parsers
{
    public interface IParser<T>
    {
        T Parse();
    }
}
