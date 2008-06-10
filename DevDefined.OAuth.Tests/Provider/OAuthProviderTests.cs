using System;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using NUnit.Framework;
using DevDefined.OAuth.Testing;

namespace DevDefined.OAuth.Tests.Provider
{
    [TestFixture]
    public class OAuthProviderTests
    {
        private OAuthProvider provider;
        
        [TestFixtureSetUp]
        public void SetUpProvider()
        {
            var tokenStore = new TestTokenStore();
            var consumerStore = new TestConsumerStore();
            var nonceStore = new TestNonceStore();

            provider = new OAuthProvider(tokenStore,
                                         new SignatureValidationInspector(consumerStore),
                                         new NonceStoreInspector(nonceStore),
                                         new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
                                         new ConsumerValidationInspector(consumerStore));                
        }

        private IOAuthSession CreateConsumer(string signatureMethod)
        {
            var consumerContext = new OAuthConsumerContext
                              {
                                  SignatureMethod = signatureMethod,
                                  ConsumerKey = "key",
                                  ConsumerSecret = "secret",
                                  Key = TestCertificates.OAuthTestCertificate().PrivateKey
                              };

            var session = new OAuthSession(consumerContext, "http://localhost/oauth/requesttoken.rails",
                                           "http://localhost/oauth/userauhtorize.rails",
                                           "http://localhost/oauth/accesstoken.rails");

            return session;
        }

        [Test]
        public void AccessProtectedResource()
        {
            var session = CreateConsumer(SignatureMethod.RsaSha1);
            session.AccessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};

            OAuthContext context = session.Request().Get().ForUrl("http://localhost/protected.rails").SignWithToken();
            
            provider.AccessProtectedResourceRequest(context);
        }

        [Test]
        public void ExchangeRequestTokenForAccessToken()
        {
            var session = CreateConsumer(SignatureMethod.RsaSha1);
            OAuthContext context =
                session.BuildExchangeRequestTokenForAccessTokenContext(
                    new TokenBase {ConsumerKey = "key", Token = "requestkey", TokenSecret = "requestsecret"});
            var accessToken = provider.ExchangeRequestTokenForAccessToken(context);
            Assert.AreEqual("accesskey", accessToken.Token);
            Assert.AreEqual("accesssecret", accessToken.TokenSecret);
        }

        [Test]
        public void ExchangeRequestTokenForAccessTokenPlainText()
        {
            var session = CreateConsumer(SignatureMethod.PlainText);
            OAuthContext context =
                session.BuildExchangeRequestTokenForAccessTokenContext(
                    new TokenBase {ConsumerKey = "key", Token = "requestkey", TokenSecret = "requestsecret"});
            var accessToken = provider.ExchangeRequestTokenForAccessToken(context);
            Assert.AreEqual("accesskey", accessToken.Token);
            Assert.AreEqual("accesssecret", accessToken.TokenSecret);
        }

        [Test]
        public void RequestTokenWithHmacSha1()
        {
            var session = CreateConsumer(SignatureMethod.HmacSha1);
            OAuthContext context = session.BuildRequestTokenContext();
            var token = provider.GrantRequestToken(context);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        [ExpectedException]
        public void RequestTokenWithHmacSha1WithInvalidSignatureThrows()
        {
            var session = CreateConsumer(SignatureMethod.HmacSha1);
            OAuthContext context = session.BuildRequestTokenContext();
            context.Signature = "wrong";
            provider.GrantRequestToken(context);
        }

        [Test]
        [ExpectedException]
        public void RequestTokenWithInvalidConsumerKeyThrowsException()
        {
            var session = CreateConsumer(SignatureMethod.PlainText);
            session.ConsumerContext.ConsumerKey = "invalid";
            OAuthContext context = session.BuildRequestTokenContext();
            provider.GrantRequestToken(context);
        }

        [Test]
        public void RequestTokenWithPlainText()
        {
            var session = CreateConsumer(SignatureMethod.PlainText);
            OAuthContext context = session.BuildRequestTokenContext();
            var token = provider.GrantRequestToken(context);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        public void RequestTokenWithRsaSha1()
        {
            var session = CreateConsumer(SignatureMethod.RsaSha1);
            OAuthContext context = session.BuildRequestTokenContext();
            var token = provider.GrantRequestToken(context);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        [ExpectedException]
        public void RequestTokenWithRsaSha1WithInvalidSignatureThrows()
        {
            var session = CreateConsumer(SignatureMethod.RsaSha1);
            OAuthContext context = session.BuildRequestTokenContext();
            context.Signature =
                "eeh8hLNIlNNq1Xrp7BOCc+xgY/K8AmjxKNM7UdLqqcvNSmJqcPcf7yQIOvu8oj5R/mDvBpSb3+CEhxDoW23gggsddPIxNdOcDuEOenugoCifEY6nRz8sbtYt3GHXsDS2esEse/N8bWgDdOm2FRDKuy9OOluQuKXLjx5wkD/KYMY=";
            provider.GrantRequestToken(context);
        }
    }
}