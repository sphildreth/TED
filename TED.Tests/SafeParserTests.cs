using TED.Utility;

namespace TED.Tests
{
    [TestClass]
    public sealed class SafeParserTests
    {
        [TestMethod]
        [DataRow("02/22/1988")]
        [DataRow("02/22/88")]
        [DataRow("02-22-1988")]
        [DataRow("1988")]
        [DataRow("\"1988\"")]
        [DataRow("1988-06-15T07:00:00Z")]
        [DataRow("1988/05/02")]
        public void DateFromString(string input)
        {
            Assert.IsNotNull(SafeParser.ToDateTime(input));
        }
    }
}
