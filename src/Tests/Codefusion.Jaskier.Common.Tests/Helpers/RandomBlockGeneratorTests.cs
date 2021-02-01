namespace Codefusion.Jaskier.Common.Tests.Helpers
{
    using System.Collections.Generic;

    using Codefusion.Jaskier.Common.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class RandomBlockGeneratorTests
    {
        [Test]
        public void Test1()
        {
            var generator = RandomBlockGenerator.Create(new List<char> { 'A', 'B' }, (min, max) => 0);
            generator.Next();
            generator.Next();
            generator.Next();
            generator.Next();
            Test("ABAB", generator);
        }

        [Test]
        public void Test2()
        {
            var generator = RandomBlockGenerator.Create(new List<char> { 'A', 'B' }, (min, max) => 1);
            generator.Next();
            generator.Next();
            generator.Next();
            generator.Next();
            generator.Next();
            Test("BABAB", generator);
        }

        [Test]
        public void Test3()
        {
            var generator = RandomBlockGenerator.Create(new List<char> { 'A', 'B' });
            generator.SetBlock('A');
            generator.Next();
            generator.Next();
            generator.Next();
            generator.Next();
            Test("ABABA", generator);

            generator.SetBlock('B');
            generator.Next();
            generator.Next();
            generator.Next();
            generator.Next();
            Test("BABAB", generator);
        }

        [Test]
        public void Test4()
        {
            var generator = RandomBlockGenerator.Create(new List<char> { 'A', 'B' }, (min, max) => 0);
            generator.SetBlock('A', 'B');
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            Test("ABAAAB", generator);
        }

        [Test]
        public void Test5()
        {
            int[] randomIndexes = { 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1 };
            int i = 0;
            var generator = RandomBlockGenerator.Create(new List<char> { 'A', 'B' }, (min, max) => randomIndexes[i++]);

            generator.SetBlock('A', 'A', 'A');
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);
            generator.Next(3);

            Test("AAABAABAAABBBA", generator);
        }

        private static void Test(string expected, RandomBlockGenerator generator)
        {
            Test(expected, generator.GetBlock().ToArray());
        }

        private static void Test(string expected, char[] block)
        {
            var actual = new string(block);
            Assert.AreEqual(expected, actual);
        }
    }
}
