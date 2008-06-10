using System;
using System.Collections;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Consumer
{
    public interface IOAuthSession
    {
        IOAuthConsumerContext ConsumerContext { get; set; }
        Uri RequestTokenUri { get; set; }
        Uri AccessTokenUri { get; set; }
        Uri UserAuthorizeUri { get; set; }
        IToken AccessToken { get; set; }
        ConsumerRequest Request();
        IToken GetRequestToken();
        IToken ExchangeRequestTokenForAccessToken(IToken requestToken);
        ConsumerRequest BuildRequestTokenContext();
        ConsumerRequest BuildExchangeRequestTokenForAccessTokenContext(IToken requestToken);
        string GetUserAuthorizationUrlForToken(IToken token, string callbackUrl);
        OAuthSession WithFormParameters(IDictionary dictionary);
        OAuthSession WithFormParameters(object anonymousClass);
        OAuthSession WithQueryParameters(IDictionary dictionary);
        OAuthSession WithQueryParameters(object anonymousClass);
        OAuthSession WithCookies(IDictionary dictionary);
        OAuthSession WithCookies(object anonymousClass);
        OAuthSession WithHeaders(IDictionary dictionary);
        OAuthSession WithHeaders(object anonymousClass);
    }
}