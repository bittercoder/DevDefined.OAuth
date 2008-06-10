using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Framework.Signing;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace DevDefined.OAuth.Tests.Provider.Inspectors
{
    [TestFixture]
    public class SignatureValidationInspectorTests
    {
        [Test]
        [ExpectedException(ExpectedMessage = "Failed to validate signature")]
        public void InvalidSignatureThrows()
        {
            var repository = new MockRepository();

            var consumerStore = repository.DynamicMock<IConsumerStore>();
            var signer = repository.CreateMock<IOAuthContextSigner>();

            var context = new OAuthContext {ConsumerKey = "key", SignatureMethod = SignatureMethod.PlainText};

            using (repository.Record())
            {
                Expect.Call(signer.ValidateSignature(null, null)).IgnoreArguments().Return(false);
            }
            using (repository.Playback())
            {
                var inspector = new SignatureValidationInspector(consumerStore, signer);
                inspector.InspectContext(context);
            }
        }

        [Test]
        public void PlainTextSignatureMethodDoesNotFetchCertificate()
        {
            var repository = new MockRepository();

            var consumerStore = repository.DynamicMock<IConsumerStore>();
            var signer = repository.CreateMock<IOAuthContextSigner>();

            var context = new OAuthContext {ConsumerKey = "key", SignatureMethod = SignatureMethod.PlainText};

            using (repository.Record())
            {
                Expect.Call(signer.ValidateSignature(null, null)).IgnoreArguments().Return(true);
            }
            using (repository.Playback())
            {
                var inspector = new SignatureValidationInspector(consumerStore, signer);
                inspector.InspectContext(context);
            }
        }

        [Test]
        public void RsaSha1SignatureMethodFetchesCertificate()
        {
            var repository = new MockRepository();

            var consumerStore = repository.DynamicMock<IConsumerStore>();
            var signer = repository.CreateMock<IOAuthContextSigner>();

            var context = new OAuthContext {ConsumerKey = "key", SignatureMethod = SignatureMethod.RsaSha1};

            using (repository.Record())
            {
                Expect.Call(consumerStore.GetConsumerCertificate(context)).Return(
                    TestCertificates.OAuthTestCertificate());
                Expect.Call(signer.ValidateSignature(null, null)).IgnoreArguments().Return(true);
            }
            using (repository.Playback())
            {
                var inspector = new SignatureValidationInspector(consumerStore, signer);
                inspector.InspectContext(context);
            }
        }
    }
}