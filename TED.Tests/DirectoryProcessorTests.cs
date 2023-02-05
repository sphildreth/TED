using TED.Processors;

namespace TED.Tests
{
    [TestClass]
    public sealed class DirectoryProcessorTests
    {
        [TestMethod]
        [DataRow("Release Title", "Something", 1, false)]
        [DataRow("Release Title", "11:11", 5, false)]
        [DataRow("Release Title", "11:11", 11, false)]
        [DataRow("Release Title", "The Track Title", 5, false)]
        [DataRow(null, null, 1, true)]
        [DataRow(null, "", 1, true)]
        [DataRow(null, " ", 1, true)]
        [DataRow(null, "   ", 1, true)]
        [DataRow("Release Title", "15", 15, true)]
        [DataRow("Release Title", "15 ", 15, true)]
        [DataRow("Release Title", "0005 Track Title", 5, true)]
        [DataRow("Release Title", "Track   Title", 5, true)]
        [DataRow("Release Title", "11 - Track Title", 11, true)]
        [DataRow("Release Title", "Track Title - Part II", 11, false)]
        [DataRow("Release Title", "Release Title", 1, false)]
        [DataRow("Release Title", "Release Title - 01 Track Title", 1, true)]
        [DataRow("Release Title", "I Can't Even Walk Without You Holding My Hand", 6, false)]
        [DataRow("Release Title", "'81 Camaro", 1, false)]
        [DataRow("Release Title", "'81 Camaro", 8, false)]
        [DataRow("Release Title", "'81 Camaro", 81, false)]
        [DataRow("Release Title", "Release Title (prod DJ Stinky)", 5, true)]
        [DataRow("Release Title", "Production Blues", 5, false)]
        [DataRow("Release Title", "The Production Blues", 5, false)]
        [DataRow("Release Title", "Deep Delightful (DJ Andy De Gage Remix)", 5, false)]
        [DataRow("Release Title", "Left and Right (Feat. Jung Kook of BTS)", 5, true)]
        [DataRow("Release Title", "Left and Right ft. Jung Kook)", 5, true)]
        [DataRow("Release Title", "Karakondžula", 5, false)]
        [DataRow("Release Title", "Track■Title", 5, false)]
        [DataRow("Release Title", "Track💣Title", 5, false)]
        [DataRow("Release Title best of 48 years (Compiled and Mixed by DJ Stinky", "Track Title (Compiled and Mixed by DJ Stinky)", 5, false)]
        [DataRow("Megamix Chart Hits Best Of 12 Years (Compiled and Mixed by DJ Fl", "Megamix Chart Hits Best Of 12 Years (Compiled and Mixed by DJ Flimflam)", 5, false)]
        public void TrackHasUnwantedText(string releaseTitle, string trackName, int? trackNumber, bool shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.TrackHasUnwantedText(releaseTitle, trackName, trackNumber));
        }

        [TestMethod]
        [DataRow("A Stone's Throw", false)]
        [DataRow("Broken Arrow", false)]
        [DataRow("American Music Vol. 1", false)]
        [DataRow(null, true)]
        [DataRow("", true)]
        [DataRow(" ", true)]
        [DataRow("   ", true)]
        [DataRow("Release Title Digipak", true)]
        [DataRow("Release Title digipak", true)]
        [DataRow("Release Title diGIpaK", true)]
        [DataRow("Monarch Deluxe Edition", true)]
        [DataRow("Monarch Re-Master", true)]
        [DataRow("Monarch Target Edition", true)]
        [DataRow("Monarch Remastered", true)]
        [DataRow("Monarch Re-mastered", true)]
        [DataRow("Monarch Release", true)]
        [DataRow("Monarch Remaster", true)]
        [DataRow("Monarch Expanded", true)]
        [DataRow("Monarch (Expanded)", true)]
        [DataRow("Monarch (Expanded", true)]
        [DataRow("Monarch WEB", true)]
        [DataRow("Monarch REMASTERED", true)]
        [DataRow("Monarch (REMASTERED)", true)]
        [DataRow("Monarch [REMASTERED]", true)]
        [DataRow("Michael Bublé - Higher (Deluxe)", true)]
        [DataRow("Necro Sapiens (320)", true)]
        [DataRow("Retro", false)]
        [DataRow("Eternally Gifted", false)]
        [DataRow("Electric Deluge, Vol. 2", false)]
        [DataRow("Experience Yourself Ep", false)]
        [DataRow("Release■Title", false)]
        [DataRow("Release💣Title", false)]
        public void ReleaseTitleHasUnwantedText(string releaseTitle, bool shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.ReleaseTitleHasUnwantedText(releaseTitle));
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(" ", null)]
        [DataRow("01-cabasa--uso_sketch-1757063c.mp3 1757063c", "01-cabasa--uso_sketch-1757063c.mp3")]
        [DataRow("The Price is Right.mp3 1757063c", "The Price is Right.mp3")]
        public void SFVFileNameFromSFVLine(string? line, string? shouldBe)
        {
            Assert.AreEqual(DirectoryProcessor.Mp3FileNameFromSFVLine(line), shouldBe);
        }

        [TestMethod]
        [DataRow(null, false)]
        [DataRow("Something", false)]
        [DataRow("Something With Bob", false)]
        [DataRow("Something Ft Bob", true)]
        [DataRow("Something ft Bob", true)]
        [DataRow("Something Ft. Bob", true)]
        [DataRow("Something (Ft. Bob)", true)]
        [DataRow("Something Feat. Bob", true)]
        [DataRow("Something Featuring Bob", true)]
        [DataRow("Eternally Gifted", false)]
        [DataRow("Shift Scene", false)]
        public void StringHasFeaturingFragments(string input, bool shouldBe)
        {
            Assert.AreEqual(DirectoryProcessor.StringHasFeaturingFragments(input), shouldBe);
        }

        [TestMethod]
        [DataRow("CD1", true)]
        [DataRow("CD01", true)]
        [DataRow("CD1 ", true)]
        [DataRow("CD 1", true)]
        [DataRow("CD 2", true)]
        [DataRow("CD 01", true)]
        [DataRow("[CD1]", true)]
        [DataRow("CD-1", true)]
        [DataRow("CD-01", true)]
        [DataRow("[CD-1]", true)]
        [DataRow("[CD-01]", true)]
        [DataRow("CD-23", true)]
        [DataRow("Release Title CD1", true)]
        [DataRow("Release Title [CD1]", true)]
        [DataRow("Release Title", false)]
        [DataRow("Release Title CD", false)]
        [DataRow("2001 - Preflyte Sessions (2-CD)", false)]
        [DataRow("America-Original Album Series (5CD Box)\\1971 America", false)]
        public void IsDirectoryMediaDirectory(string directory, bool shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.IsDirectoryMediaDirectory(null, directory));
        }

        [TestMethod]
        [DataRow("cover", true)]
        [DataRow("covers", true)]
        [DataRow("scans", true)]
        [DataRow("Release Title", false)]
        [DataRow("Release Title CD", false)]
        public void IsCoverImagesDirectory(string directory, bool shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.IsCoverImagesDirectory(directory));
        }

        [TestMethod]
        [DataRow("CD1", 1)]
        [DataRow("CD01", 1)]
        [DataRow("CD1 ", 1)]
        [DataRow("CD051 ", 51)]
        [DataRow("[CD1]", 1)]
        [DataRow("Release Title CD1", 1)]
        [DataRow("Release Title [CD1]", 1)]
        [DataRow("Release Title", 1)]
        [DataRow("Release Title CD", 1)]
        public void DetermineMediaNumberFromDirectory(string directory, int shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.DetermineMediaNumberFromDirectory(directory));
        }

        [TestMethod]
        [DataRow("Whitesnake & Tesla", "Whitesnake/Tesla")]
        [DataRow("Whitesnake & Tesla ft Banana", "Whitesnake/Tesla/Banana")]
        [DataRow("Whitesnake ft Banana", "Whitesnake/Banana")]
        [DataRow("Whitesnake", "Whitesnake")]
        [DataRow("Duke/Jones X Justin Theroux", "Duke/Jones/Justin Theroux")]
        [DataRow("Post Malone Ft. Doja Cat", "Post Malone/Doja Cat")]
        [DataRow("Post Malone;Doja Cat", "Post Malone/Doja Cat")]
        [DataRow("Post Malone ; Doja Cat", "Post Malone/Doja Cat")]
        [DataRow("Post Malone; Doja Cat", "Post Malone/Doja Cat")]
        [DataRow("Post Malone with Doja Cat", "Post Malone/Doja Cat")]
        public void ReplaceTrackArtistSeperators(string input, string shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.ReplaceTrackArtistSeperators(input));
        }

        [TestMethod]
        [DataRow("Track Title", "Track Title")]
        [DataRow("Track Title Ft. Alisha", "Track Title")]
        [DataRow("Track Title feat Alisha", "Track Title")]
        [DataRow("Track Title (Ft. Alisha)", "Track Title")]
        [DataRow("Track Title Feat. Alisha", "Track Title")]
        [DataRow("The Hardest Day Of My Life (feat. Tom Newton)", "The Hardest Day Of My Life")]
        [DataRow("Dada (feat. Tefo Foxx)", "Dada")]
        [DataRow("Sol (Feat. Gustavo \"Chizzo\" Napoli (La Renga) & Alex Lora)", "Sol")]
        public void RemoveFeaturingArtistFromTrackTitleCompareTrackTitle(string text, string shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.RemoveFeaturingArtistFromTrackTitle(text).Item1);
        }

        [TestMethod]
        [DataRow("Track Title", null)]
        [DataRow("Track Title Ft. Alisha", "Alisha")]
        [DataRow("Track Title (Ft. Alisha)", "Alisha")]
        [DataRow("Track Title Feat. Alisha", "Alisha")]
        [DataRow("Bob Ft. Jim", "Jim")]
        [DataRow("The Hardest Day Of My Life (feat. Tom Newton)", "Tom Newton")]
        [DataRow("Dada (feat. Tefo Foxx)", "Tefo Foxx")]
        [DataRow("Dada (feat. Tefo Foxx & Foxxy Brown)", "Tefo Foxx/Foxxy Brown")]
        [DataRow("Sol (Feat. Gustavo \"Chizzo\" Napoli (La Renga) & Alex Lora)", "Gustavo 'Chizzo' Napoli (La Renga)/Alex Lora")]
        public void RemoveFeaturingArtistFromTrackTitleCompareFeaturingArtist(string text, string? shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.RemoveFeaturingArtistFromTrackTitle(text).Item2);
        }

        [TestMethod]
        [DataRow("Track Title", false)]
        [DataRow("cover-PROOF.jpg", true)]
        [DataRow("cover proof.jpg", true)]
        [DataRow("00-master_blaster-we_love_italo_disco-cd-flac-2003-proof.jpg", true)]
        [DataRow("cover.jpg", false)]
        public void IsImageProofType(string text, bool shouldBe)
        {
            Assert.AreEqual(shouldBe, DirectoryProcessor.IsImageAProofType(new FileInfo(text)));
        }

    }
}
