using TED.Processors;
using TED.Utility;

namespace TED.Tests
{
    [TestClass]
    public sealed class ImageHelperTests
    {
        [TestMethod]
        [DataRow("info.txt", false)]
        [DataRow("band info.txt", false)]
        [DataRow("band.txt", false)]
        [DataRow("Something With Bob.jpg", false)]
        [DataRow("logo.jpg", false)]
        [DataRow("Band.jpg", true)]
        [DataRow("Band_1.jpg", true)]
        [DataRow("Band01.jpg", true)]
        [DataRow("Band 01.jpg", true)]
        [DataRow("Band 01.jpg", true)]
        [DataRow("Band 14.jpg", true)]
        public void FileIsBandImage(string fileName, bool shouldBe)
        {
            Assert.AreEqual(ImageHelper.IsArtistImage(new FileInfo(fileName)), shouldBe);
        }
    }
}
