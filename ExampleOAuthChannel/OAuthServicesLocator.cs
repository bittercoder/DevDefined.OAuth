using System;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Storage.Basic;
using DevDefined.OAuth.Testing;

namespace ExampleOAuthChannel
{
  public class OAuthServicesLocator
  {
    static readonly ITokenRepository<DevDefined.OAuth.Storage.Basic.AccessToken> _accessTokenRepository;
    static readonly IOAuthProvider _provider;
    static readonly ITokenRepository<DevDefined.OAuth.Storage.Basic.RequestToken> _requestTokenRepository;

    static OAuthServicesLocator()
    {
      var consumerStore = new TestConsumerStore();
      var nonceStore = new TestNonceStore();
      _accessTokenRepository = new InMemoryTokenRepository<DevDefined.OAuth.Storage.Basic.AccessToken>();
      _requestTokenRepository = new InMemoryTokenRepository<DevDefined.OAuth.Storage.Basic.RequestToken>();

      var tokenStore = new SimpleTokenStore(_accessTokenRepository, _requestTokenRepository);

      _provider = new OAuthProvider(tokenStore,
                                    new SignatureValidationInspector(consumerStore),
                                    new NonceStoreInspector(nonceStore),
                                    new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
                                    new ConsumerValidationInspector(consumerStore));
    }

    public static IOAuthProvider Provider
    {
      get { return _provider; }
    }

    public static ITokenRepository<DevDefined.OAuth.Storage.Basic.AccessToken> AccessTokenRepository
    {
      get { return _accessTokenRepository; }
    }

    public static ITokenRepository<DevDefined.OAuth.Storage.Basic.RequestToken> RequestTokenRepository
    {
      get { return _requestTokenRepository; }
    }
  }
}