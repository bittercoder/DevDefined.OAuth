using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Provider
{
    public interface IOAuthProvider
    {
        IToken GrantRequestToken(OAuthContext context);
        IToken ExchangeRequestTokenForAccessToken(OAuthContext context);
        void AccessProtectedResourceRequest(OAuthContext context);
    }
}