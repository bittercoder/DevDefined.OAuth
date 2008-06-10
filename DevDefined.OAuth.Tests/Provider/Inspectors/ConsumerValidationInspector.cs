using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace DevDefined.OAuth.Tests.Provider.Inspectors
{
    [TestFixture]
    public class ConsumerValidationInspector
    {
        [Test]
        [ExpectedException]
        public void InValidConsumerThrows()
        {
            var repository = new MockRepository();

            var consumerStore = repository.CreateMock<IConsumerStore>();
            var context = new OAuthContext {ConsumerKey = "key"};

            using (repository.Record())
            {
                Expect.Call(consumerStore.IsConsumer(context)).Return(false);
            }
            using (repository.Playback())
            {
                var inspector = new OAuth.Provider.Inspectors.ConsumerValidationInspector(consumerStore);
                inspector.InspectContext(context);
            }
        }

        [Test]
        public void ValidConsumerPassesThrough()
        {
            var repository = new MockRepository();

            var consumerStore = repository.CreateMock<IConsumerStore>();
            var context = new OAuthContext {ConsumerKey = "key"};

            using (repository.Record())
            {
                Expect.Call(consumerStore.IsConsumer(context)).Return(true);
            }
            using (repository.Playback())
            {
                var inspector = new OAuth.Provider.Inspectors.ConsumerValidationInspector(consumerStore);
                inspector.InspectContext(context);
            }
        }
    }
}