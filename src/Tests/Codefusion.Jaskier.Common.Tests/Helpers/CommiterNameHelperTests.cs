namespace Codefusion.Jaskier.Common.Tests.Helpers
{
    using Codefusion.Jaskier.Common.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class CommiterNameHelperTests
    {
        [Test]
        public void Test()
        {
            Test("", "");
            Test("", "   ");
            Test("A", "A");
            Test("AB", "Ab");
            Test("AB", "  Ab  ");
            Test("AB AC", "Ab Ac");
            Test("LUKE SKYWALKER", "Luke Skywalker");
            Test("BRZECZYSZCZYKIEWICZ GRZEGORZ", "Grzegorz BrzĘczyszczykiewicz");
            Test("QWERTYUIOPASDFGHJKLZXZCVBNM", "QWĘRTYUIÓPĄSDFGHJKLŻXŹCVBŃM");
            Test("OSTERREICH PENE UBERMUT VON", "Österreich Übermut von Pené");
            Test("OSTERREICH PENE UBERMUT VON", "  Pené Österreich von   Übermut ");
            Test("JON SNOW", "NORTH\\Jon Snow");
        }

        private static void Test(string expected, string parameter)
        {
            Assert.AreEqual(expected, CommiterNameHelper.Normalize(parameter));
        }
    }
}
