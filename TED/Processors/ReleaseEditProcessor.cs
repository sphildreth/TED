using System.Text.RegularExpressions;
using TED.Extensions;
using TED.Models.MetaData;
using TED.Utility;

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

        public Task<Release> DoMagic(Release release)
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
            var modifiedTitle = RemoveUnwantedTextFromReleaseTitle(release.ReleaseData?.Text);
            release.ReleaseData = new Models.DataToken
            {
                Text = modifiedTitle,
                Value = SafeParser.ToToken(modifiedTitle)
            };
            return Task.FromResult(release);
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

        public static string? RemoveUnwantedTextFromReleaseTitle(string? title)
        {
            if (title.Nullify() == null)
            {
                return null;
            }
            return DirectoryProcessor.UnwantedReleaseTitleTextRegex.Replace(title, string.Empty).Trim();
        }

        public void RenumberTracks(Release release)
        {
            var looper = 1;
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                looper = 1;
                foreach (var track in tracks)
                {
                    track.TrackNumber = looper;
                    looper++;
                }
                media.Tracks = tracks;
            }
            release.TrackCount = looper;
            release.Media = medias;
        }

        public void TrimTrackTitles(int stringLengthToTrim, Release release)
        {
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks.Where(x => x.Title.Nullify() != null))
                {
                    track.Title = track.Title.Substring(stringLengthToTrim, track.Title.Length - stringLengthToTrim);
                }
                media.Tracks = tracks;
            }
            release.Media = medias;
        }

        public void RemoveFeaturingArtistFromTrackTitle(Release release)
        {
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks)
                {
                    var trackFeatureArtist = DirectoryProcessor.RemoveFeaturingArtistFromTrackTitle(track.Title);
                    track.Title = trackFeatureArtist.Item1 ?? track.Title;
                    if (trackFeatureArtist.Item2 != null && !StringExt.DoStringsMatch(release?.Artist?.Text, trackFeatureArtist.Item2))
                    {
                        if (track.TrackArtist.ArtistData.Text.Nullify() != null)
                        {
                            var tt = $"{track.TrackArtist.ArtistData.Text}/{trackFeatureArtist.Item2}";
                            track.TrackArtist.ArtistData = new Models.DataToken
                            {
                                Text = tt,
                                Value = SafeParser.ToToken(tt)
                            };
                        }
                        else
                        {
                            var tt = trackFeatureArtist.Item2;
                            track.TrackArtist.ArtistData = new Models.DataToken
                            {
                                Text = tt,
                                Value = SafeParser.ToToken(tt)
                            };
                        }
                    }
                }
                media.Tracks = tracks;
            }
            release.Media = medias;
        }

        public async Task PromoteTrackArtist(Release release)
        {
            release.Artist = release.Media.First().Tracks.First().TrackArtist.ArtistData;
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    track.TrackArtist = new Artist()
                    {
                        ArtistData = new Models.DataToken()
                    };
                }
                media.Tracks = tracks;
            }
            release.Media = medias;
        }

        public void SetYearToCurrent(Release release)
        {
            release.ReleaseDateDateTime = DateTime.UtcNow;
        }

        public static void RemoveTrackArtistFromTracks(Release release)
        {
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    track.TrackArtist = new Artist()
                    {
                        ArtistData = new Models.DataToken()
                    };
                }
                media.Tracks = tracks;
            }
            release.Media = medias;
        }

        public static void RemoveArtistFromTrackArtists(Release release)
        {
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    var tt = RemoveArtistFromTrackArtist(release.Artist?.Text, track.TrackArtist.ArtistData.Text);
                    track.TrackArtist.ArtistData = new Models.DataToken
                    {
                        Text = tt,
                        Value = SafeParser.ToToken(tt)
                    };
                }
                media.Tracks = tracks;
            }
            release.Media = medias;
        }

        public void ReplaceTracksArtistSeperators(Release release)
        {
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    var tt = DirectoryProcessor.ReplaceTrackArtistSeperators(track.TrackArtist.ArtistData.Text);
                    track.TrackArtist.ArtistData = new Models.DataToken
                    {
                        Text = tt,
                        Value = SafeParser.ToToken(tt)
                    };
                }
                media.Tracks = tracks;
            }
            release.Media = medias;
        }

        public void ReplaceTextFromTracks(Release release, string textToReplace, string textToReplaceWith)
        {
            if (textToReplace.Nullify() == null)
            {
                return;
            }
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks)
                {
                    if (track.Title.Nullify() != null)
                    {
                        track.Title = track.Title.Replace(textToReplace, textToReplaceWith).Trim();
                    }
                }
                media.Tracks = tracks;
            }
            release.Media = medias;
        }

        public void RemoveFeaturingArtistFromTracksArtist(Release release)
        {
            var medias = (release?.Media ?? Enumerable.Empty<ReleaseMedia>()).OrderBy(x => x.MediaNumber).ToList();
            foreach (var media in medias)
            {
                var tracks = (media.Tracks ?? Enumerable.Empty<Track>()).OrderBy(x => x.TrackNumber).ToList();
                foreach (var track in tracks.Where(x => x.TrackArtist?.ArtistData != null))
                {
                    var trackArtistFeatureArtist = DirectoryProcessor.RemoveFeaturingArtistFromTrackTitle(track.TrackArtist?.ArtistData.Text);
                    if (trackArtistFeatureArtist.Item1 != null && trackArtistFeatureArtist.Item2 == null &&
                        !StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item1))
                    {
                        track.TrackArtist.ArtistData = new Models.DataToken
                        {
                            Text = trackArtistFeatureArtist.Item1,
                            Value = SafeParser.ToToken(trackArtistFeatureArtist.Item1)
                        };
                    }
                    else if (trackArtistFeatureArtist.Item1 != null && trackArtistFeatureArtist.Item2 != null)
                    {
                        if (!StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item1) &&
                            !StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item2))
                        {
                            var tt = $"{trackArtistFeatureArtist.Item1}/{trackArtistFeatureArtist.Item2}";
                            track.TrackArtist.ArtistData = new Models.DataToken
                            {
                                Text = tt,
                                Value = SafeParser.ToToken(tt)
                            };
                        }
                        else if (StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item1) &&
                                 !StringExt.DoStringsMatch(release?.Artist?.Text, trackArtistFeatureArtist.Item2))
                        {
                            track.TrackArtist.ArtistData = new Models.DataToken
                            {
                                Text = trackArtistFeatureArtist.Item2,
                                Value = SafeParser.ToToken(trackArtistFeatureArtist.Item2)
                            };
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
                media.Tracks = tracks;
            }
            release.Media = medias;
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
