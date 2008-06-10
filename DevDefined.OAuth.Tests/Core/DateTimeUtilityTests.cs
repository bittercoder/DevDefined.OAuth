using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            DateTime newYears = new DateTime(2008,1,1,0,0,0);

            long epoch = DateTimeUtility.Epoch(newYears);

            DateTime fromEpoch = DateTimeUtility.FromEpoch((int)epoch);

            Assert.AreEqual(newYears, fromEpoch);
        }
    }
}
