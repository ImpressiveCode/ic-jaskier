namespace Codefusion.Jaskier.Common.Tests.Helpers
{
    using Codefusion.Jaskier.Common.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class PathHelperTests
    {
        [Test]
        public void RelativePathTest()
        {
            Assert.AreEqual("A\\B\\C.txt", PathHelper.ReplaceStart("C:\\Dev\\Project\\A\\B\\C.txt", "C:\\Dev\\Project"));
            Assert.AreEqual("C:\\Dev\\Project\\A\\B\\C.txt", PathHelper.ReplaceStart("C:\\Dev\\Project\\A\\B\\C.txt", string.Empty));
            Assert.AreEqual("Dev\\Project\\A\\B\\C.txt", PathHelper.ReplaceStart("C:\\Dev\\Project\\A\\B\\C.txt", "C:\\"));
            Assert.AreEqual("C:\\Dev\\Project\\A\\B\\C.txt", PathHelper.ReplaceStart("C:\\Dev\\Project\\A\\B\\C.txt", "D:\\"));
        }
    }
}
