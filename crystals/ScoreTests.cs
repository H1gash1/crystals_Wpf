using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace crystals.Tests
{
    [TestClass]
    public class ScoreTests
    {
        [TestMethod]
        public void ThreeCrystalsMatch_ShouldGive30Points()
        {
            int matchCount = 3;
            int expectedPoints = 30;

            int actualPoints = CalculatePoints(matchCount);

            Assert.AreEqual(expectedPoints, actualPoints, "За 3 кристалла должно давать 30 очков");
        }

        private int CalculatePoints(int matchCount)
        {
            switch (matchCount)
            {
                case 3: return 30;
                case 4: return 60;
                case 5: return 100;
                default: return matchCount * 20;
            }
        }
    }
}