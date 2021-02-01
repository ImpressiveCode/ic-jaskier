namespace Codefusion.Jaskier.Common.Tests.Helpers
{
    using System.Collections.Generic;

    using Codefusion.Jaskier.Common.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class ImposedBlockGeneratorTests
    {
        [Test]
        public void NullTest()
        {
            var generator = new ImposedBlockGenerator();
            Assert.IsNull(generator.Next());
            generator.SetBlock(null);
            Assert.IsNull(generator.Next());
        }

        [Test]
        public void Test()
        {
            var generator = new ImposedBlockGenerator();
            generator.SetBlock(new List<char> { 'A', 'B', 'C', 'D' });
            generator.SetPosition(-5);

            Assert.AreEqual('A', generator.Next());
            Assert.AreEqual('B', generator.Next());
            Assert.AreEqual('C', generator.Next());
            Assert.AreEqual('D', generator.Next());
            Assert.AreEqual('A', generator.Next());
            Assert.AreEqual('B', generator.Next());
            Assert.AreEqual('C', generator.Next());
            Assert.AreEqual('D', generator.Next());
            Assert.AreEqual('A', generator.Next());
            Assert.AreEqual('B', generator.Next());
            Assert.AreEqual('C', generator.Next());
            Assert.AreEqual('D', generator.Next());
        }
    }
}
