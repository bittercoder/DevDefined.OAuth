using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Consumer
{
    public interface IOAuthConsumer
    {
        string Realm { get; set; }
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        string SignatureMethod { get; set; }
        AsymmetricAlgorithm Key { get; set; }
        Uri RequestTokenUri { get; }
        Uri AccessTokenUri { get; }
        bool UseHeaderForOAuthParameters { get; set; }
        IToken RequestToken(NameValueCollection additionalQueryParameters);

        IToken ExchangeRequestTokenForAccessToken(IToken requestToken,
                                                     NameValueCollection additionalQueryParameters);

        HttpWebResponse GetResponse(OAuthContext context, IToken accessToken);
        OAuthContext BuildRequestTokenContext(NameValueCollection additionalQueryParameters);

        OAuthContext BuildExchangeRequestTokenForAccessTokenContext(IToken requestToken,
                                                                    NameValueCollection additionalQueryParameters);
        string GetUserAuthorizationUrlForToken(IToken token, string callbackUrl, NameValueCollection additionalQueryParameters);
        
        void SignContext(OAuthContext context, IToken accessToken);
    }
}