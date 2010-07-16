using System;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Testing;
using Xunit;

namespace DevDefined.OAuth.Tests.Provider
{
  public class OAuthProvider1ATests
  {
  	readonly OAuthProvider provider;

		public OAuthProvider1ATests()
    {
      var tokenStore = new TestTokenStore();
      var consumerStore = new TestConsumerStore();
      var nonceStore = new TestNonceStore();

      provider = new OAuthProvider(tokenStore,
                                   new SignatureValidationInspector(consumerStore),
                                   new NonceStoreInspector(nonceStore),
                                   new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
                                   new ConsumerValidationInspector(consumerStore),
                                   new OAuth10AInspector(tokenStore));
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

    [Fact]
    public void ExchangeTokensWhenVerifierIsMatchDoesNotThrowException()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
      IOAuthContext context = session.BuildExchangeRequestTokenForAccessTokenContext(
        new TokenBase {ConsumerKey = "key", Token = "requestkey"}, "GET", "GzvVb5WjWfHKa/0JuFupaMyn").Context;
      provider.ExchangeRequestTokenForAccessToken(context);
    }

    [Fact]
    public void ExchangeTokensWhenVerifierIsMissingThrowsException()
    {
      string verifier = null;

      IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
      IOAuthContext context = session.BuildExchangeRequestTokenForAccessTokenContext(
        new TokenBase {ConsumerKey = "key", Token = "requestkey"}, "GET", verifier).Context;
    	var ex = Assert.Throws<OAuthException>(() => provider.ExchangeRequestTokenForAccessToken(context));
    	Assert.Equal("Missing required parameter : oauth_verifier", ex.Message);
    }

    [Fact]
    public void ExchangeTokensWhenVerifierIsWrongThrowsException()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.RsaSha1);
      IOAuthContext context = session.BuildExchangeRequestTokenForAccessTokenContext(
        new TokenBase {ConsumerKey = "key", Token = "requestkey"}, "GET", "wrong").Context;
    	var ex = Assert.Throws<OAuthException>(() => provider.ExchangeRequestTokenForAccessToken(context));
			Assert.Equal("The parameter \"oauth_verifier\" was rejected", ex.Message);
    }

    [Fact]
    public void RequestTokenWithCallbackDoesNotThrowException()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      provider.GrantRequestToken(context);
    }

    [Fact]
    public void RequestTokenWithoutCallbackWillThrowException()
    {
      IOAuthSession session = CreateConsumer(SignatureMethod.PlainText);
      IOAuthContext context = session.BuildRequestTokenContext("GET").Context;
      context.CallbackUrl = null; // clear parameter, as it will default to "oob"
    	var ex = Assert.Throws<OAuthException>(() => provider.GrantRequestToken(context));
			Assert.Equal("Missing required parameter : oauth_callback", ex.Message);
    }
  }
}