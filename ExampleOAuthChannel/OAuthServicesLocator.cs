using System;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Testing;
using DevDefined.OAuth.Wcf.Implementation;
using DevDefined.OAuth.Wcf.Repositories;

namespace ExampleOAuthChannel
{
  public class OAuthServicesLocator
  {
    static readonly ITokenRepository<DevDefined.OAuth.Wcf.Models.AccessToken> _accessTokenRepository;
    static readonly IOAuthProvider _provider;
    static readonly ITokenRepository<DevDefined.OAuth.Wcf.Models.RequestToken> _requestTokenRepository;

    static OAuthServicesLocator()
    {
      var consumerStore = new TestConsumerStore();
      var nonceStore = new TestNonceStore();
      _accessTokenRepository = new InMemoryTokenRepository<DevDefined.OAuth.Wcf.Models.AccessToken>();
      _requestTokenRepository = new InMemoryTokenRepository<DevDefined.OAuth.Wcf.Models.RequestToken>();

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

    public static ITokenRepository<DevDefined.OAuth.Wcf.Models.AccessToken> AccessTokenRepository
    {
      get { return _accessTokenRepository; }
    }

    public static ITokenRepository<DevDefined.OAuth.Wcf.Models.RequestToken> RequestTokenRepository
    {
      get { return _requestTokenRepository; }
    }
  }
}