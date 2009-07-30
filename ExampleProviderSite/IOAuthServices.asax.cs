using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Storage.Basic;

namespace ExampleProviderSite
{
  public interface IOAuthServices
  {
    IOAuthProvider Provider { get; }
    ITokenRepository<DevDefined.OAuth.Storage.Basic.AccessToken> AccessTokenRepository { get; }
    ITokenRepository<DevDefined.OAuth.Storage.Basic.RequestToken> RequestTokenRepository { get; }
  }
}