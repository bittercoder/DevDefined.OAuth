using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace DevDefined.OAuth.Tests.Provider.Inspectors
{
    [TestFixture]
    public class NonceStoreInspectorTests
    {
        [Test]
        [ExpectedException(ExpectedMessage = "The nonce value \"1\" has already been used")]
        public void InspectContextForRepeatedNonceThrows()
        {
            var repository = new MockRepository();

            var nonceStore = repository.CreateMock<INonceStore>();
            var context = new OAuthContext {Nonce = "1"};

            using (repository.Record())
            {
                Expect.Call(nonceStore.RecordNonceAndCheckIsUnique(context, "1")).Return(false);
            }
            using (repository.Playback())
            {
                var inspector = new NonceStoreInspector(nonceStore);
                inspector.InspectContext(context);
            }
        }

        [Test]
        public void InspectContextForUniqueNoncePasses()
        {
            var repository = new MockRepository();

            var nonceStore = repository.CreateMock<INonceStore>();
            var context = new OAuthContext {Nonce = "2"};

            using (repository.Record())
            {
                Expect.Call(nonceStore.RecordNonceAndCheckIsUnique(context, "2")).Return(true);
            }
            using (repository.Playback())
            {
                var inspector = new NonceStoreInspector(nonceStore);
                inspector.InspectContext(context);
            }
        }
    }
}