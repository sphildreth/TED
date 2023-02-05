using TED.Extensions;

namespace TED.Tests
{
    [TestClass]
    public class StringExtTests
    {
        [TestMethod]
        [DataRow("Bob", "Bob", true)]
        [DataRow("Bob", "Bob ", true)]
        [DataRow("Bob", "bOb", true)]
        [DataRow("Bob", "BOB ", true)]
        [DataRow("Bob", null, false)]
        [DataRow(null, "Bob", false)]
        [DataRow("Bob", "Steve", false)]
        public void DoStringMatch(string string1, string string2, bool shouldBe)
        {
            Assert.AreEqual(shouldBe, StringExt.DoStringsMatch(string1, string2));
        }

        [TestMethod]
        [DataRow("Bob", "Bob")]
        [DataRow("Bob    ", "Bob")]
        [DataRow("Bob    ", "Bob")]
        [DataRow("   Bob   ", "Bob")]
        [DataRow("Bob And Nancy", "Bob And Nancy")]
        [DataRow("Bob And Nancy!", "Bob And Nancy!")]
        [DataRow("Bob And Nancy, wITH sTEVE", "Bob And Nancy, wITH sTEVE")]
        [DataRow(" Bob    And    Nancy", "Bob And Nancy")]
        [DataRow(" Bob    And    Nancy   ", "Bob And Nancy")]
        public void CleanString(string input, string shouldBe)
        {
            Assert.AreEqual(shouldBe, input.CleanString());
        }
    }
}
