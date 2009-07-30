#region License

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

#endregion

using System;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Testing;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Provider
{
  [TestFixture]
  public class OAuthProviderTests
  {
    OAuthProvider provider;

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

    IOAuthSession CreateConsumer(string signatureMethod)
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
      IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
      session.AccessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};

      var context = session.Request().Get().ForUrl("http://localhost/protected.rails").SignWithToken().Context;

      provider.AccessProtectedResourceRequest(context);
    }

    [Test]
    public void ExchangeRequestTokenForAccessToken()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
      IOAuthContext context =
        session.BuildExchangeRequestTokenForAccessTokenContext(
          new TokenBase {ConsumerKey = "key", Token = "requestkey", TokenSecret = "requestsecret"},"GET", null).Context;
      IToken accessToken = provider.ExchangeRequestTokenForAccessToken(context);
      Assert.AreEqual("accesskey", accessToken.Token);
      Assert.AreEqual("accesssecret", accessToken.TokenSecret);
    }

    [Test]
    public void ExchangeRequestTokenForAccessTokenPlainText()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
      IOAuthContext context =
        session.BuildExchangeRequestTokenForAccessTokenContext(
          new TokenBase {ConsumerKey = "key", Token = "requestkey", TokenSecret = "requestsecret"}, "GET", null).Context;
      IToken accessToken = provider.ExchangeRequestTokenForAccessToken(context);
      Assert.AreEqual("accesskey", accessToken.Token);
      Assert.AreEqual("accesssecret", accessToken.TokenSecret);
    }

    [Test]
    public void RequestTokenWithHmacSha1()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.HmacSha1);
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      IToken token = provider.GrantRequestToken(context);
      Assert.AreEqual("requestkey", token.Token);
      Assert.AreEqual("requestsecret", token.TokenSecret);
    }

    [Test]
    [ExpectedException]
    public void RequestTokenWithHmacSha1WithInvalidSignatureThrows()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.HmacSha1);
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      context.Signature = "wrong";
      provider.GrantRequestToken(context);
    }

    [Test]
    [ExpectedException]
    public void RequestTokenWithInvalidConsumerKeyThrowsException()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
      session.ConsumerContext.ConsumerKey = "invalid";
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      provider.GrantRequestToken(context);
    }

    [Test]
    public void RequestTokenWithPlainText()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      IToken token = provider.GrantRequestToken(context);
      Assert.AreEqual("requestkey", token.Token);
      Assert.AreEqual("requestsecret", token.TokenSecret);
    }

    [Test]
    public void RequestTokenWithRsaSha1()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      IToken token = provider.GrantRequestToken(context);
      Assert.AreEqual("requestkey", token.Token);
      Assert.AreEqual("requestsecret", token.TokenSecret);
    }

    [Test]
    [ExpectedException]
    public void RequestTokenWithRsaSha1WithInvalidSignatureThrows()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      context.Signature =
        "eeh8hLNIlNNq1Xrp7BOCc+xgY/K8AmjxKNM7UdLqqcvNSmJqcPcf7yQIOvu8oj5R/mDvBpSb3+CEhxDoW23gggsddPIxNdOcDuEOenugoCifEY6nRz8sbtYt3GHXsDS2esEse/N8bWgDdOm2FRDKuy9OOluQuKXLjx5wkD/KYMY=";
      provider.GrantRequestToken(context);
    }
  }
}