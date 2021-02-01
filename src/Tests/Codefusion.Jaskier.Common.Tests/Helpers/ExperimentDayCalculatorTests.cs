namespace Codefusion.Jaskier.Common.Tests.Helpers
{
    using System;

    using Codefusion.Jaskier.Common.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class ExperimentDayCalculatorTests
    {
        [Test]
        public void Test()
        {
            var calc = new ExperimentDayCalculator();
            calc.Days = 10;

            var now = DateTime.Now;

            // 10
            Assert.AreEqual(10, calc.Days);

            // 10
            Assert.AreEqual(10, calc.Calculate(now));
            Assert.AreEqual(10, calc.Calculate(now));
            Assert.AreEqual(10, calc.Calculate(now.AddHours(23)));
            Assert.AreEqual(10, calc.Calculate(now.AddDays(-1)));

            // 11
            Assert.AreEqual(11, calc.Calculate(now.AddDays(1)));
            Assert.AreEqual(11, calc.Calculate(now.AddDays(1)));

            // 12
            Assert.AreEqual(12, calc.Calculate(now.AddDays(2)));

            // 13
            Assert.AreEqual(13, calc.Calculate(now.AddDays(3)));

            // 14            
            Assert.AreEqual(14, calc.Calculate(now.AddDays(4)));

            // 15            
            Assert.AreEqual(15, calc.Calculate(now.AddDays(7)));

            // 16
            Assert.AreEqual(16, calc.Calculate(now.AddDays(9)));

            Assert.AreEqual(16, calc.Days);
        }
    }
}
