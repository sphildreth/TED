﻿using TED.Extensions;

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

        [TestMethod]
        [DataRow("Bob", "Bob")]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow(" ", null)]
        [DataRow("Bob ", "Bob ")]
        public void Nullify(string? input, string? shouldBe)
        {
            Assert.AreEqual(shouldBe, input.Nullify());
        }        
        
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow("Bob", null)]
        [DataRow("09/Bob Rocks", null)]        
        [DataRow("Bob Rocks", null)]        
        [DataRow("/Discography 2001-2010/2009/Bob Rocks", 2009)]
        [DataRow("2009/Bob Rocks", 2009)]
        [DataRow("2009 Bob Rocks", 2009)]
        [DataRow("2009", 2009)]
        public void TryToGetYearFromString(string input, int? shouldBe)
        {
            Assert.AreEqual(shouldBe, input.TryToGetYearFromString());
        }
        
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow("Bob", null)]
        [DataRow("01 Steve Winwood.mp3", 1)]
        [DataRow(" 01 Steve Winwood.mp3", 1)]
        [DataRow(" 01  - Steve Winwood.mp3", 1)]
        [DataRow("01-Steve Winwood.mp3", 1)]
        [DataRow("01 - Steve Winwood.mp3", 1)]
        [DataRow("001 - Steve Winwood.mp3", 1)]
        [DataRow("14 - Steve Winwood.mp3", 14)]
        [DataRow(" 01 - Steve Winwood.mp3", 1)]
        public void TryToGetTrackNumberFromString(string input, int? shouldBe)
        {
            Assert.AreEqual(shouldBe, input.TryToGetTrackNumberFromString());
        }    
        
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow("Bob", null)]
        [DataRow("01 Steve Winwood.mp3", "Steve Winwood.mp3")]
        [DataRow(" 01 Steve Winwood.mp3", "Steve Winwood.mp3")]
        [DataRow(" 01  - Steve Winwood.mp3", "Steve Winwood.mp3")]
        [DataRow("01-Steve Winwood.mp3", "Steve Winwood.mp3")]
        [DataRow("01 - Steve Winwood.mp3", "Steve Winwood.mp3")]
        [DataRow("001 - Steve Winwood.mp3", "Steve Winwood.mp3")]
        [DataRow("14 - Steve Winwood.mp3", "Steve Winwood.mp3")]
        [DataRow(" 01 - Steve Winwood.mp3", "Steve Winwood.mp3")]
        public void RemoveTrackNumberFromString(string input, string? shouldBe)
        {
            Assert.AreEqual(shouldBe, input.RemoveTrackNumberFromString());
        }         
        
    }
}
