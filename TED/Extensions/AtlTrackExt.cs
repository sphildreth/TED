using TED.Utility;

namespace TED.Extensions
{
    public static class AtlTrackExt
    {
        public static FileInfo FileInfo(this ATL.Track track)
        {
            return new FileInfo(track.Path);
        }

        public static short DisNumberValue(this ATL.Track track)
        {
            var r = SafeParser.ToNumber<short?>(track.DiscNumber) ?? 1;
            return r < Processors.DirectoryProcessor.MinimumDiscNumber ? Processors.DirectoryProcessor.MinimumDiscNumber : r;
        }
    }
}
