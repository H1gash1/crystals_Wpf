using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace crystals.Tests
{
    [TestClass]
    public class AchievementTests
    {
        [TestMethod]
        public void Score100_ShouldUnlock100PointsAchievement()
        {
            int score = 100;

            Assert.IsTrue(score >= 100, "При 100 очках достижение должно открыться");

            score = 99;
            Assert.IsFalse(score >= 100, "При 99 очках достижение не должно открыться");
        }
    }
}