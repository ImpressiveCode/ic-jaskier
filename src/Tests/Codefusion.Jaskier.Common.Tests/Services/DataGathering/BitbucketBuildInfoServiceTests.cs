namespace Codefusion.Jaskier.Common.Tests.Services.DataGathering
{
    using System.Linq;
    using System.Threading.Tasks;
    using Codefusion.Jaskier.API;
    using Codefusion.Jaskier.Common.Services;
    using NUnit.Framework;

    [TestFixture]
    public class BitbucketBuildInfoServiceTests
    {
        [Test]
        [Ignore("Requires database with existing build info.")]
        public async Task TestQueries()
        {
            // Arrange
            var service = new BitbucketBuildInfoService(new TestsConfiguration());
            string startFrom;
            string endAt;

            // Test getting all.
            var allBuildInfos = (await service.GetBuildsInfo(null)).ToList();

            int count = allBuildInfos.Count;

            Assert.That(count > 0);

            // Test range from. Start from 5th element.            
            startFrom = allBuildInfos[4].CommitHash;

            var startFromInfos = (await service.GetBuildsInfo(new CommitsRange(startFrom, null))).ToList();

            Assert.AreEqual(5, startFromInfos.Count);
            Assert.AreEqual(startFrom, startFromInfos.Last().CommitHash);

            // Test range to. End at 5th element.
            endAt = allBuildInfos[4].CommitHash;

            var endAtInfos = (await service.GetBuildsInfo(new CommitsRange(null, endAt))).ToList();

            Assert.AreEqual(count - 4, endAtInfos.Count);
            Assert.AreEqual(endAt, endAtInfos[0].CommitHash);

            // Test range from-to. Start from 10th and end at 5th.
            startFrom = allBuildInfos[9].CommitHash;
            endAt = allBuildInfos[4].CommitHash;

            var startEndInfos = (await service.GetBuildsInfo(new CommitsRange(startFrom, endAt))).ToList();

            Assert.AreEqual(6, startEndInfos.Count);
            Assert.AreEqual(startFrom, startEndInfos.Last().CommitHash);
            Assert.AreEqual(endAt, startEndInfos.First().CommitHash);
        }
    }
}
