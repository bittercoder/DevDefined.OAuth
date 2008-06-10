using System;
using DevDefined.OAuth.Core;
using DevDefined.OAuth.Provider;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Provider.Inspectors
{
    [TestFixture]
    public class TimestapmRangeInspectorTests
    {
        [Test]
        [ExpectedException(typeof (OAuthException))]
        public void OutsideAfterRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(0, 0, 0), new TimeSpan(1, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 13, 0, 1).Epoch().ToString();

            inspector.InspectContext(context);
        }

        [Test]
        [ExpectedException(typeof (OAuthException))]
        public void OutsideBeforeRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 10, 59, 59).Epoch().ToString();

            inspector.InspectContext(context);
        }

        [Test]
        public void WithAfterRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(0, 0, 0), new TimeSpan(1, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 13, 0, 0).Epoch().ToString();

            inspector.InspectContext(context);
        }

        [Test]
        public void WithinBeforeRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 11, 0, 0).Epoch().ToString();

            inspector.InspectContext(context);
        }
    }
}