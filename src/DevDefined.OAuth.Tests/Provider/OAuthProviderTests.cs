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
using Xunit;

namespace DevDefined.OAuth.Tests.Provider
{
	public class OAuthProvider10Tests
	{
		readonly OAuthProvider provider;

		public OAuthProvider10Tests()
		{
			var tokenStore = new TestTokenStore();
			var consumerStore = new TestConsumerStore();
			var nonceStore = new TestNonceStore();

			provider = new OAuthProvider(tokenStore,
			                             new SignatureValidationInspector(consumerStore),
			                             new NonceStoreInspector(nonceStore),
			                             new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
			                             new ConsumerValidationInspector(consumerStore),
                                   new XAuthValidationInspector(ValidateXAuthMode, AuthenticateXAuthUsernameAndPassword));
		}

		static IOAuthSession CreateConsumer(string signatureMethod)
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

    public bool ValidateXAuthMode(string authMode)
    {
      return authMode == "client_auth";
    }

    public bool AuthenticateXAuthUsernameAndPassword(string username, string password)
    {
      return username == "username" && password == "password";
    }

		[Fact]
		public void AccessProtectedResource()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
			session.AccessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};
			IOAuthContext context = session.Request().Get().ForUrl("http://localhost/protected.rails").SignWithToken().Context;
			context.TokenSecret = null;
			provider.AccessProtectedResourceRequest(context);
		}

		[Fact]
		public void AccessProtectedResourceWithPlainText()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
			session.AccessToken = new TokenBase {ConsumerKey = "key", Token = "accesskey", TokenSecret = "accesssecret"};
			IOAuthContext context = session.Request().Get().ForUrl("http://localhost/protected.rails").SignWithToken().Context;
			context.TokenSecret = null;
			provider.AccessProtectedResourceRequest(context);
		}

		[Fact]
		public void ExchangeRequestTokenForAccessToken()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
			IOAuthContext context =
				session.BuildExchangeRequestTokenForAccessTokenContext(
					new TokenBase {ConsumerKey = "key", Token = "requestkey", TokenSecret = "requestsecret"}, "GET", null).Context;
			context.TokenSecret = null;
			IToken accessToken = provider.ExchangeRequestTokenForAccessToken(context);
			Assert.Equal("accesskey", accessToken.Token);
			Assert.Equal("accesssecret", accessToken.TokenSecret);
		}

		[Fact]
		public void ExchangeRequestTokenForAccessTokenPlainText()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
			IOAuthContext context =
				session.BuildExchangeRequestTokenForAccessTokenContext(
					new TokenBase {ConsumerKey = "key", Token = "requestkey", TokenSecret = "requestsecret"}, "GET", null).Context;
			context.TokenSecret = null;
			IToken accessToken = provider.ExchangeRequestTokenForAccessToken(context);
			Assert.Equal("accesskey", accessToken.Token);
			Assert.Equal("accesssecret", accessToken.TokenSecret);
		}

		[Fact]
		public void ExchangeTokensWhenVerifierIsMatchDoesNotThrowException()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
			IOAuthContext context = session.BuildExchangeRequestTokenForAccessTokenContext(
				new TokenBase {ConsumerKey = "key", Token = "requestkey"}, "GET", "GzvVb5WjWfHKa/0JuFupaMyn").Context;
			provider.ExchangeRequestTokenForAccessToken(context);
		}

		[Fact]
		public void RequestTokenWithHmacSha1()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.HmacSha1);
			IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
			IToken token = provider.GrantRequestToken(context);
			Assert.Equal("requestkey", token.Token);
			Assert.Equal("requestsecret", token.TokenSecret);
		}

		[Fact]
		public void RequestTokenWithHmacSha1WithInvalidSignatureThrows()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.HmacSha1);
			IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
			context.Signature = "wrong";
			var ex = Assert.Throws<OAuthException>(() => (provider.GrantRequestToken(context)));
			Assert.Equal("Failed to validate signature", ex.Message);
		}

		[Fact]
		public void RequestTokenWithInvalidConsumerKeyThrowsException()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
			session.ConsumerContext.ConsumerKey = "invalid";
			IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
			var ex = Assert.Throws<OAuthException>(() => provider.GrantRequestToken(context));
			Assert.Equal("Unknown Consumer (Realm: , Key: invalid)", ex.Message);
		}

		[Fact]
		public void RequestTokenWithPlainText()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
			IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
			IToken token = provider.GrantRequestToken(context);
			Assert.Equal("requestkey", token.Token);
			Assert.Equal("requestsecret", token.TokenSecret);
		}

		[Fact]
		public void RequestTokenWithRsaSha1()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
			IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
			IToken token = provider.GrantRequestToken(context);
			Assert.Equal("requestkey", token.Token);
			Assert.Equal("requestsecret", token.TokenSecret);
		}

		[Fact]
		public void RequestTokenWithRsaSha1WithInvalidSignatureThrows()
		{
			IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
			IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
			context.Signature =
				"eeh8hLNIlNNq1Xrp7BOCc+xgY/K8AmjxKNM7UdLqqcvNSmJqcPcf7yQIOvu8oj5R/mDvBpSb3+CEhxDoW23gggsddPIxNdOcDuEOenugoCifEY6nRz8sbtYt3GHXsDS2esEse/N8bWgDdOm2FRDKuy9OOluQuKXLjx5wkD/KYMY=";
			var ex = Assert.Throws<OAuthException>(() => provider.GrantRequestToken(context));
			Assert.Equal("Failed to validate signature", ex.Message);
		}

		[Fact]
		public void RequestTokenWithTokenSecretParamterThrowsException()
		{
			IOAuthContext context = new OAuthContext {TokenSecret = "secret"};
			var ex = Assert.Throws<OAuthException>(() => provider.ExchangeRequestTokenForAccessToken(context));
			Assert.Equal("The oauth_token_secret must not be transmitted to the provider.", ex.Message);
		}

    [Fact]
    public void AccessTokenWithHmacSha1()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.HmacSha1);
      IOAuthContext context = session.BuildAccessTokenContext("GET", "client_auth", "username", "password").Context;
      context.TokenSecret = null;
      IToken accessToken = provider.CreateAccessToken(context);
      Assert.Equal("accesskey", accessToken.Token);
      Assert.Equal("accesssecret", accessToken.TokenSecret);
    }
  }
}