namespace TED.Extensions
{
    public static class AtlTrackExt
    {
        public static FileInfo FileInfo(this ATL.Track track)
        {
            return new FileInfo(track.Path);
        }
    }
}
