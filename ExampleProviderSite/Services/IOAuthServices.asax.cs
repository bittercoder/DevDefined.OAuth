using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Storage.Basic;

namespace ExampleProviderSite
{
  public interface IOAuthServices
  {
    IOAuthProvider Provider { get; }
    ITokenRepository<AccessToken> AccessTokenRepository { get; }
    ITokenRepository<RequestToken> RequestTokenRepository { get; }
  }
}