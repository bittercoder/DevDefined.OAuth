using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Core;
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

        private OAuthConsumer CreateConsumer(string signatureMethod)
        {
            return new OAuthConsumer("http://term.ie/oauth/example/request_token.php",
                                     "http://localhost/authorize",
                                     "http://term.ie/oauth/example/access_token.php")
                       {
                           SignatureMethod = signatureMethod,
                           ConsumerKey = "key",
                           ConsumerSecret = "secret",
                           Key = certificate.PrivateKey
                       };
        }

        [Test]
        public void ExchangeRequestTokenForAccessTokenRsaSha1()
        {
            OAuthConsumer consumer = CreateConsumer(SignatureMethod.RsaSha1);

            var requestToken = consumer.RequestToken(null);

            // now exchange the token

            var accessToken = consumer.ExchangeRequestTokenForAccessToken(requestToken, null);

            Assert.AreEqual("key", accessToken.ConsumerKey);
            Assert.AreEqual("accesskey", accessToken.Token);
            Assert.AreEqual("accesssecret", accessToken.TokenSecret);
        }

        [Test]
        public void MakeAuthenticatedCallForTokenRsaSha1WithPost()
        {
            var accessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};
            OAuthConsumer consumer = CreateConsumer(SignatureMethod.RsaSha1);

            var factory = new OAuthContextFactory();
            OAuthContext context = factory.FromUri("POST", new Uri("http://term.ie/oauth/example/echo_api.php"));
            context.FormEncodedParameters.Add("success", "true");

            HttpWebResponse response = consumer.GetResponse(context, accessToken);

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string contents = reader.ReadToEnd();
                Assert.AreEqual("success=true", contents);
            }
        }

        [Test]
        public void MakeAuthenticatedCallForTokenRsaSha1WithPostAndHeaders()
        {
            var accessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};
            OAuthConsumer consumer = CreateConsumer(SignatureMethod.RsaSha1);
            consumer.UseHeaderForOAuthParameters = true;

            var factory = new OAuthContextFactory();
            OAuthContext context = factory.FromUri("POST", new Uri("http://term.ie/oauth/example/echo_api.php"));
            context.FormEncodedParameters.Add("success", "true");
            context.Realm = "http://term.ie/";

            HttpWebResponse response = consumer.GetResponse(context, accessToken);

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string contents = reader.ReadToEnd();
                Assert.AreEqual("success=true", contents);
            }
        }

        [Test]
        public void RequestTokenForHmacSha1()
        {
            OAuthConsumer consumer = CreateConsumer(SignatureMethod.HmacSha1);

            var token = consumer.RequestToken(null);

            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        public void RequestTokenForPlainText()
        {
            OAuthConsumer consumer = CreateConsumer(SignatureMethod.PlainText);

            var token = consumer.RequestToken(null);

            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        public void RequestTokenForRsaSha1()
        {
            OAuthConsumer consumer = CreateConsumer(SignatureMethod.RsaSha1);

            var token = consumer.RequestToken(null);

            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }

        [Test]
        public void RequestTokenForRsaSha1WithAddtionalQueryParameters()
        {
            OAuthConsumer consumer = CreateConsumer(SignatureMethod.RsaSha1);

            var parameters = new NameValueCollection
                                 {
                                     {"scope", "http://www.google.com/m8/feeds"}
                                 };

            var token = consumer.RequestToken(parameters);
            Assert.AreEqual("key", token.ConsumerKey);
            Assert.AreEqual("requestkey", token.Token);
            Assert.AreEqual("requestsecret", token.TokenSecret);
        }
    }
}