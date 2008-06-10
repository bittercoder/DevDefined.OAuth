using System.Security.Cryptography.X509Certificates;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Consumer
{
    /// <summary>
    /// These tests check our consumer implementation against the test server at: 
    /// http://term.ie/oauth/example/index.php?sig_method=PLAINTEXT
    /// </summary>
    [TestFixture]
    public class TermIeConsumerIntegrationTests
    {
        private readonly X509Certificate2 certificate = TestCertificates.OAuthTestCertificate();

        private IOAuthSession CreateConsumer(string signatureMethod)
        {
            var consumerContext = new OAuthConsumerContext
                                      {
                                          SignatureMethod = signatureMethod,
                                          ConsumerKey = "key",
                                          ConsumerSecret = "secret",
                                          Key = certificate.PrivateKey,
                                          Realm = "http://term.ie/"
                                      };

            return new OAuthSession(consumerContext, "http://term.ie/oauth/example/request_token.php",
                                    "http://localhost/authorize",
                                    "http://term.ie/oauth/example/access_token.php");
        }

        [Test]
        public void ExchangeRequestTokenForAccessTokenRsaSha1()
        {
            IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);

            IToken requestToken = session.GetRequestToken();

            // now exchange the token

            IToken accessToken = session.ExchangeRequestTokenForAccessToken(requestToken);

            Assert.AreEqual("key", accessToken.ConsumerKey);
            Assert.AreEqual("accesskey", accessToken.Token);
            Assert.AreEqual("accesssecret", accessToken.TokenSecret);
        }

        [Test]
        public void MakeAuthenticatedCallForTokenRsaSha1WithPost()
        {
            IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
            session.AccessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};

            string contents = session.Request().Post().ForUrl("http://term.ie/oauth/example/echo_api.php")
                .WithFormParameters(new {success = "true"})
                .ToString();

            Assert.AreEqual("success=true", contents);
        }

        [Test]
        public void MakeAuthenticatedCallForTokenRsaSha1WithPostAndHeaders()
        {
            IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
            session.AccessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};
            session.ConsumerContext.UseHeaderForOAuthParameters = true;

            ConsumerRequest context = session.Request().Post().ForUrl("http://term.ie/oauth/example/echo_api.php")
                .WithFormParameters(new {success = "true"})
                .SignWithToken();

            string contents = context.ToString();

            Assert.AreEqual("success=true", contents);
        }

        [Test]
        public void RequestTokenForHmacSha1()
        {
            IOAuthSession session = CreateConsumer(SignatureMethod.HmacSha1);

            IToken token = session.GetRequestToken();

            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        public void RequestTokenForPlainText()
        {
            IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);

            IToken token = session.GetRequestToken();

            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        public void RequestTokenForRsaSha1()
        {
            IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);

            IToken token = session.GetRequestToken();

            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        public void RequestTokenForRsaSha1WithAddtionalQueryParameters()
        {
            IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);

            IToken token = session
                .WithQueryParameters(new {scope = "http://term.ie/apps/subscriptions"})
                .GetRequestToken();

            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }
    }
}