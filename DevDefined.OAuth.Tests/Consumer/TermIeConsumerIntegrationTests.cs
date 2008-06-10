// The MIT License
//
// Copyright (c) 2006-2008 DevDefined Limited.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
﻿using System.Security.Cryptography.X509Certificates;
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