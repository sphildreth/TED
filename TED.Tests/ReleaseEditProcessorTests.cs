using Microsoft.Extensions.Logging;
using TED.Models.MetaData;
using TED.Processors;

namespace TED.Tests
{
    [TestClass]
    public class ReleaseEditProcessorTests
    {
        private ILogger<ReleaseEditProcessor>? _releaseEditProcessorLogger;

        private ILogger<ReleaseEditProcessor> ReleaseEditProcessorLogger
        {
            get
            {
                if (_releaseEditProcessorLogger != null)
                {
                    return _releaseEditProcessorLogger;
                }
                using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                _releaseEditProcessorLogger = loggerFactory.CreateLogger<ReleaseEditProcessor>();
                return _releaseEditProcessorLogger;
            }
        }

        private Release NewTestRelease => new Release
        {
            Artist = new Models.DataToken(),
            ReleaseData = new Models.DataToken(),
            Media = new List<ReleaseMedia>
                    {
                        new ReleaseMedia
                        {
                            Tracks = new List<Track>
                            {
                                new Track
                                {
                                    TrackArtist = new Artist()
                                    {
                                        ArtistData = new Models.DataToken()
                                    }
                                }
                            }
                        }
                    }
        };

        [TestMethod]
        [DataRow("Something & Bob", null, "Something & Bob")]
        [DataRow("Something & Bob", "", "Something & Bob")]
        [DataRow("Something & Bob", "Something", "Bob")]
        [DataRow("Something and Bob", "Something", "Bob")]
        [DataRow("Something with Bob", "Something", "Bob")]
        [DataRow("Something Bob", "Something", "Bob")]
        public void RemoveStringFromString(string input, string stringToRemove, string shouldBe)
        {
            Assert.AreEqual(shouldBe, ReleaseEditProcessor.RemoveArtistFromTrackArtist(stringToRemove, input));
        }

        [TestMethod]
        [DataRow("Bobs Greatest Hits CD 01", "Bobs Greatest Hits")]
        [DataRow("Bobs Greatest Hits CD1", "Bobs Greatest Hits")]
        [DataRow("Bobs Greatest Hits cD1", "Bobs Greatest Hits")]
        [DataRow("Bobs Greatest Hits", "Bobs Greatest Hits")]
        [DataRow("Superman III", "Superman III")]
        public void RemoveUnwantedTextFromReleaseTitle(string input, string shouldBe)
        {
            Assert.AreEqual(shouldBe, ReleaseEditProcessor.RemoveUnwantedTextFromReleaseTitle(input));
        }

        [TestMethod]
        public async Task InvalidYearGetsReplacedWithCurrent()
        {
            var release = NewTestRelease;
            var releaseEditProcessor = new ReleaseEditProcessor(ReleaseEditProcessorLogger);
            await releaseEditProcessor.DoMagic(release);
            Assert.AreEqual(DateTime.UtcNow.Year, release.Year);
        }

        [TestMethod]
        public async Task TrackArtistFeatureHandling()
        {
            var release = NewTestRelease;
            release.Artist = new Models.DataToken
            {
                Text = "Bob"
            };
            var newTracks = new List<Track>()
            {
                new Track
                {
                    TrackNumber = 1,
                    TrackArtist = new Artist
                    {
                        ArtistData = new Models.DataToken
                        {
                            Text = "Bob"
                        }
                    }
                },
                new Track
                {
                    TrackNumber = 2,
                    TrackArtist = new Artist
                    {
                        ArtistData = new Models.DataToken
                        {
                            Text = "Bob Ft. Jim"
                        }
                    }
                },
                new Track
                {
                    TrackNumber = 3,
                    TrackArtist = new Artist
                    {
                        ArtistData = new Models.DataToken
                        {
                            Text = "Jim"
                        }
                    }
                },
                new Track
                {
                    TrackNumber = 4,
                    TrackArtist = new Artist
                    {
                        ArtistData = new Models.DataToken
                        {
                            Text = "Jim/Lulu/Angie"
                        }
                    }
                },
                new Track
                {
                    TrackNumber = 5,
                    TrackArtist = new Artist
                    {
                        ArtistData = new Models.DataToken
                        {
                            Text = "Jim Ft. Lulu"
                        }
                    }
                },
                new Track
                {
                    TrackNumber = 6,
                    TrackArtist = new Artist
                    {
                        ArtistData = new Models.DataToken
                        {
                            Text = "Bob feat Lulu"
                        }
                    }
                }
            };
            release.Media.First().Tracks = newTracks.ToList();
            var releaseEditProcessor = new ReleaseEditProcessor(ReleaseEditProcessorLogger);
            var rr = await releaseEditProcessor.DoMagic(release);

            Assert.AreEqual(null, rr.Media.First().Tracks.First(x => x.TrackNumber == 1).TrackArtist?.ArtistData?.Text);
            Assert.AreEqual("Jim", rr.Media.First().Tracks.First(x => x.TrackNumber == 2).TrackArtist?.ArtistData?.Text);
            Assert.AreEqual("Jim", rr.Media.First().Tracks.First(x => x.TrackNumber == 3).TrackArtist?.ArtistData?.Text);
            Assert.AreEqual("Jim/Lulu/Angie", rr.Media.First().Tracks.First(x => x.TrackNumber == 4).TrackArtist?.ArtistData?.Text);
            Assert.AreEqual("Jim/Lulu", rr.Media.First().Tracks.First(x => x.TrackNumber == 5).TrackArtist?.ArtistData?.Text);
            Assert.AreEqual("Lulu", rr.Media.First().Tracks.First(x => x.TrackNumber == 6).TrackArtist?.ArtistData?.Text);
        }

        [TestMethod]
        [DataRow(null, false)]
        [DataRow(0, false)]
        [DataRow(1492, false)]
        [DataRow(2072, false)]
        [DataRow(ReleaseEditProcessor.MinimumYearValue, true)]
        [DataRow(1980, true)]
        [DataRow(2022, true)]
        public void IsValidYear(int? year, bool shouldBe)
        {
            Assert.AreEqual(shouldBe, ReleaseEditProcessor.IsValidReleaseYear(year));
        }
    }
}
