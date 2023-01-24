using System.Text.RegularExpressions;
using TED.Extensions;
using TED.Models.MetaData;

namespace TED.Processors
{
    public class ReleaseEditProcessor
    {
        public const int MinimumYearValue = 1900;

        private readonly ILogger _logger;

        public ReleaseEditProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<(bool, IEnumerable<string>)> ProcessAsync(DateTime now, Release release)
        {
            throw new NotImplementedException();
        }

        public async Task<Release> DoMagic(Release release)
        {
            release.ReleaseDateDateTime = DateTime.Parse(release.ReleaseDate ?? DateTime.MinValue.ToString());
            RenumberTracks(release);
            RemoveFeaturingArtistFromTracksArtist(release);
            RemoveFeaturingArtistFromTrackTitle(release);
            ReplaceTracksArtistSeperators(release);
            if (!IsValidReleaseYear(release.Year))
            {
                SetYearToCurrent(release);
            }
            return release;
        }

        public static string? RemoveArtistFromTrackArtist(string? artist, string? trackArtist)
        {
            if (artist.Nullify() == null)
            {
                return trackArtist;
            }
            if (trackArtist.Nullify() == null)
            {
                return null;
            }
            return Regex.Replace(trackArtist, $"\\s*({artist})\\s*(&|and|with)*", string.Empty).Trim();
        }

        public void RenumberTracks(Release release)
        {
            var looper = 1;
            foreach (var media in release.Media)
            {
                looper = 1;
                foreach (var track in media.Tracks)
                {
                    track.TrackNumber = looper;
                    looper++;
                }
            }
            release.TrackCount = looper;
        }

        public void TrimTrackTitles(int stringLengthToTrim, Release release)
        {
            foreach (var media in release.Media)
            {
                foreach (var track in media.Tracks.Where(x => x.Title.Nullify() != null))
                {
                    track.Title = track.Title.Substring(stringLengthToTrim, track.Title.Length - stringLengthToTrim);
                }
            }
        }

        public void RemoveFeaturingArtistFromTrackTitle(Release release)
        {
            foreach (var media in release.Media)
            {
                foreach (var track in media.Tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    var trackFeatureArtist = DirectoryProcessor.RemoveFeaturingArtistFromTrackTitle(track.Title);
                    track.Title = trackFeatureArtist.Item1 ?? track.Title;
                    if (trackFeatureArtist.Item2 != null && !StringExt.DoStringsMatch(release?.Artist?.Text, trackFeatureArtist.Item2))
                    {
                        if (track.TrackArtist.ArtistData.Text.Nullify() != null)
                        {
                            track.TrackArtist.ArtistData.Text = $"{track.TrackArtist.ArtistData.Text}/{trackFeatureArtist.Item2}";
                        }
                        else
                        {
                            track.TrackArtist.ArtistData.Text = trackFeatureArtist.Item2;
                        }
                    }
                }
            }
        }

        public async Task PromoteTrackArtist(Release release)
        {
            release.Artist = release.Media.First().Tracks.First().TrackArtist.ArtistData;
            foreach (var media in release.Media)
            {
                foreach (var track in media.Tracks)
                {
                    track.TrackArtist = new Artist()
                    {
                        ArtistData = new Models.DataToken()
                    };
                }
            }
        }

        public void SetYearToCurrent(Release release)
        {
            release.ReleaseDateDateTime = DateTime.UtcNow;
        }

        public static void RemoveArtistFromTrackArtists(Release release)
        {
            foreach (var media in release.Media)
            {
                foreach (var track in media.Tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    track.TrackArtist.ArtistData.Text = RemoveArtistFromTrackArtist(release.Artist?.Text, track.TrackArtist.ArtistData.Text);
                }
            }
        }

        public void ReplaceTracksArtistSeperators(Release release)
        {
            foreach (var media in release.Media)
            {
                foreach (var track in media.Tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    track.TrackArtist.ArtistData.Text = DirectoryProcessor.ReplaceTrackArtistSeperators(track.TrackArtist.ArtistData.Text);
                }
            }
        }

        public void RemoveFeaturingArtistFromTracksArtist(Release release)
        {
            foreach (var media in release.Media)
            {
                foreach (var track in media.Tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    var trackArtistFeatureArtist = DirectoryProcessor.RemoveFeaturingArtistFromTrackTitle(track.TrackArtist?.ArtistData.Text);
                    if (trackArtistFeatureArtist.Item1 != null && trackArtistFeatureArtist.Item2 == null &&
                        !StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item1))
                    {
                        track.TrackArtist.ArtistData.Text = trackArtistFeatureArtist.Item1;
                    }
                    else if (trackArtistFeatureArtist.Item1 != null && trackArtistFeatureArtist.Item2 != null)
                    {
                        if (!StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item1) &&
                            !StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item2))
                        {
                            track.TrackArtist.ArtistData.Text = $"{trackArtistFeatureArtist.Item1}/{trackArtistFeatureArtist.Item2}";
                        }
                        else if (StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item1) &&
                                  !StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item2))
                        {
                            track.TrackArtist.ArtistData.Text = trackArtistFeatureArtist.Item2;
                        }
                    }
                    else
                    {
                        track.TrackArtist = new Artist
                        {
                            ArtistData = new Models.DataToken()
                        };
                    }
                }
            }
        }

        public static bool IsValidReleaseYear(int? year)
        {
            if (year == null) 
            { 
                return false; 
            }
            return year.Value >= MinimumYearValue && year.Value <= DateTime.UtcNow.Year + 1;
        }
    }
}
