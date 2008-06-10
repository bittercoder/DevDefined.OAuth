using System;
using DevDefined.OAuth.Framework;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Core
{
    [TestFixture]
    public class DateTimeUtilityTests
    {
        [Test]
        public void RoundTripEpoch()
        {
            var newYears = new DateTime(2008, 1, 1, 0, 0, 0);

            long epoch = newYears.Epoch();

            DateTime fromEpoch = DateTimeUtility.FromEpoch((int) epoch);

            Assert.AreEqual(newYears, fromEpoch);
        }
    }
}