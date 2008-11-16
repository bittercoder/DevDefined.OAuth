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
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using Castle.Core;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
  public class OAuthSession : IOAuthSession
  {
    readonly NameValueCollection _cookies = new NameValueCollection();
    readonly NameValueCollection _formParameters = new NameValueCollection();
    readonly NameValueCollection _headers = new NameValueCollection();
    readonly NameValueCollection _queryParameters = new NameValueCollection();

    public OAuthSession(IOAuthConsumerContext consumerContext, Uri requestTokenUri, Uri userAuthorizeUri,
                        Uri accessTokenUri)
    {
      ConsumerContext = consumerContext;
      RequestTokenUri = requestTokenUri;
      AccessTokenUri = accessTokenUri;
      UserAuthorizeUri = userAuthorizeUri;
    }

    public OAuthSession(IOAuthConsumerContext consumerContext, string requestTokenUrl, string userAuthorizeUrl,
                        string accessTokenUrl)
      : this(consumerContext, new Uri(requestTokenUrl), new Uri(userAuthorizeUrl), new Uri(accessTokenUrl))
    {
    }

    #region IOAuthSession Members

    public IOAuthConsumerContext ConsumerContext { get; set; }
    public Uri RequestTokenUri { get; set; }
    public Uri AccessTokenUri { get; set; }
    public Uri UserAuthorizeUri { get; set; }
    public IToken AccessToken { get; set; }

    public ConsumerRequest Request()
    {
      var context = new OAuthContext();

      context.UseAuthorizationHeader = ConsumerContext.UseHeaderForOAuthParameters;
      context.Cookies.Add(_cookies);
      context.FormEncodedParameters.Add(_formParameters);
      context.Headers.Add(_headers);
      context.QueryParameters.Add(_queryParameters);

      return new ConsumerRequest(context, ConsumerContext, AccessToken);
    }

    public IToken GetRequestToken()
    {
      return BuildRequestTokenContext().Select(collection =>
                                               new TokenBase
                                                 {
                                                   ConsumerKey = ConsumerContext.ConsumerKey,
                                                   Token =
                                                     ParseResponseParameter(collection,
                                                                            Parameters.OAuth_Token),
                                                   TokenSecret =
                                                     ParseResponseParameter(collection,
                                                                            Parameters.
                                                                              OAuth_Token_Secret)
                                                 });
    }

    public IToken ExchangeRequestTokenForAccessToken(IToken requestToken)
    {
      TokenBase token = BuildExchangeRequestTokenForAccessTokenContext(requestToken)
        .Select(collection =>
                new TokenBase
                  {
                    ConsumerKey = requestToken.ConsumerKey,
                    Token =
                      ParseResponseParameter(collection,
                                             Parameters.
                                               OAuth_Token),
                    TokenSecret =
                      ParseResponseParameter(collection,
                                             Parameters.
                                               OAuth_Token_Secret)
                  });

      AccessToken = token;

      return token;
    }

    public ConsumerRequest BuildRequestTokenContext()
    {
      return Request().Get().ForUri(RequestTokenUri).SignWithoutToken();
    }

    public ConsumerRequest BuildExchangeRequestTokenForAccessTokenContext(IToken requestToken)
    {
      return Request().Get().ForUri(AccessTokenUri).SignWithToken(requestToken);
    }

    public string GetUserAuthorizationUrlForToken(IToken token, string callbackUrl)
    {
      var builder = new UriBuilder(UserAuthorizeUri);

      var collection = new NameValueCollection();

      if (builder.Query != null)
      {
        collection.Add(HttpUtility.ParseQueryString(builder.Query));
      }

      if (_queryParameters != null) collection.Add(_queryParameters);

      collection[Parameters.OAuth_Token] = token.Token;

      if (!string.IsNullOrEmpty(callbackUrl))
      {
        collection[Parameters.OAuth_Callback] = callbackUrl;
      }

      builder.Query = "";

      return builder.Uri + "?" + UriUtility.FormatQueryString(collection);
    }

    public OAuthSession WithFormParameters(IDictionary dictionary)
    {
      return AddItems(_formParameters, dictionary);
    }

    public OAuthSession WithFormParameters(object anonymousClass)
    {
      return AddItems(_formParameters, anonymousClass);
    }

    public OAuthSession WithQueryParameters(IDictionary dictionary)
    {
      return AddItems(_queryParameters, dictionary);
    }

    public OAuthSession WithQueryParameters(object anonymousClass)
    {
      return AddItems(_queryParameters, anonymousClass);
    }

    public OAuthSession WithCookies(IDictionary dictionary)
    {
      return AddItems(_cookies, dictionary);
    }

    public OAuthSession WithCookies(object anonymousClass)
    {
      return AddItems(_cookies, anonymousClass);
    }

    public OAuthSession WithHeaders(IDictionary dictionary)
    {
      return AddItems(_headers, dictionary);
    }

    public OAuthSession WithHeaders(object anonymousClass)
    {
      return AddItems(_headers, anonymousClass);
    }

    #endregion

    static string ParseResponseParameter(NameValueCollection collection, string parameter)
    {
      string value = (collection[parameter] ?? "").Trim();
      return (value.Length > 0) ? value : null;
    }

    OAuthSession AddItems(NameValueCollection destination, object anonymousClass)
    {
      return AddItems(destination, new ReflectionBasedDictionaryAdapter(anonymousClass));
    }

    OAuthSession AddItems(NameValueCollection destination, IDictionary additions)
    {
      foreach (string parameter in additions.Keys)
      {
        destination[parameter] = Convert.ToString(additions[parameter]);
      }

      return this;
    }
  }
}