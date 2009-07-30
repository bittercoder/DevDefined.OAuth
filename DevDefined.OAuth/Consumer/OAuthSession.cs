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
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Utility;

namespace DevDefined.OAuth.Consumer
{
  public class OAuthSession : IOAuthSession
  {
    readonly NameValueCollection _cookies = new NameValueCollection();
    readonly NameValueCollection _formParameters = new NameValueCollection();
    readonly NameValueCollection _headers = new NameValueCollection();
    readonly NameValueCollection _queryParameters = new NameValueCollection();
    IConsumerRequestFactory _consumerRequestFactory = DefaultConsumerRequestFactory.Instance;

    public IConsumerRequestFactory ConsumerRequestFactory
    {
      get { return _consumerRequestFactory; }
      set
      {
        if (_consumerRequestFactory == null) throw new ArgumentNullException("value");
        _consumerRequestFactory = value;
      }
    }

    public OAuthSession(IOAuthConsumerContext consumerContext, Uri requestTokenUri, Uri userAuthorizeUri,
                        Uri accessTokenUri)
      : this(consumerContext, requestTokenUri, userAuthorizeUri, accessTokenUri, null)
    {
    }

    public OAuthSession(IOAuthConsumerContext consumerContext, Uri requestTokenUri, Uri userAuthorizeUri,
                        Uri accessTokenUri, Uri callBackUri)
    {
      ConsumerContext = consumerContext;
      RequestTokenUri = requestTokenUri;
      AccessTokenUri = accessTokenUri;
      UserAuthorizeUri = userAuthorizeUri;
      CallbackUri = callBackUri;
    }

    public OAuthSession(IOAuthConsumerContext consumerContext, string requestTokenUrl, string userAuthorizeUrl,
                        string accessTokenUrl, string callBackUrl)
      : this(consumerContext, new Uri(requestTokenUrl), new Uri(userAuthorizeUrl), new Uri(accessTokenUrl), ParseCallbackUri(callBackUrl))
    {
    }

    public OAuthSession(IOAuthConsumerContext consumerContext, string requestTokenUrl, string userAuthorizeUrl,
                        string accessTokenUrl)
      : this(consumerContext, requestTokenUrl, userAuthorizeUrl, accessTokenUrl, null)
    {
    }

    public bool CallbackMustBeConfirmed { get; set; }

    public Uri CallbackUri { get; set; }

    public IOAuthConsumerContext ConsumerContext { get; set; }
    public Uri RequestTokenUri { get; set; }
    public Uri AccessTokenUri { get; set; }
    public Uri UserAuthorizeUri { get; set; }
    public IToken AccessToken { get; set; }

    public IConsumerRequest Request()
    {
      var context = new OAuthContext
        {
          UseAuthorizationHeader = ConsumerContext.UseHeaderForOAuthParameters          
        };

      context.Cookies.Add(_cookies);
      context.FormEncodedParameters.Add(_formParameters);
      context.Headers.Add(_headers);
      context.QueryParameters.Add(_queryParameters);

      return _consumerRequestFactory.CreateConsumerRequest(context, ConsumerContext, AccessToken);
    }

    public IToken GetRequestToken()
    {
      return GetRequestToken("GET");
    }

    public IToken GetRequestToken(string method)
    {
      var results = BuildRequestTokenContext(method).Select(collection =>
                                               new {
                                                   ConsumerKey = ConsumerContext.ConsumerKey,
                                                   Token =
                                                     ParseResponseParameter(collection,
                                                                            Parameters.OAuth_Token),
                                                   TokenSecret =
                                                     ParseResponseParameter(collection,
                                                                            Parameters.
                                                                              OAuth_Token_Secret),
                                                   CallackConfirmed = WasCallbackConfimed(collection)
                                                 });

      if (!results.CallackConfirmed && CallbackMustBeConfirmed)
      {
        throw Error.CallbackWasNotConfirmed();
      }

      return new TokenBase
        {
          ConsumerKey = results.ConsumerKey,
          Token = results.Token,
          TokenSecret = results.TokenSecret
        };
    }

    static bool WasCallbackConfimed(NameValueCollection parameters)
    {
      var value = ParseResponseParameter(parameters, Parameters.OAuth_Callback_Confirmed);
      return (value == "true");
    }

    public IToken ExchangeRequestTokenForAccessToken(IToken requestToken)
    {
      return ExchangeRequestTokenForAccessToken(requestToken, "GET", null);
    }

    public IToken ExchangeRequestTokenForAccessToken(IToken requestToken, string verificationCode)
    {
      return ExchangeRequestTokenForAccessToken(requestToken, "GET", verificationCode);
    }

    public IToken ExchangeRequestTokenForAccessToken(IToken requestToken, string method, string verificationCode)
    {
      TokenBase token = BuildExchangeRequestTokenForAccessTokenContext(requestToken, method, verificationCode)
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

    public IConsumerRequest BuildRequestTokenContext(string method)
    {
      return Request()
        .ForMethod(method)
        .AlterContext(context=>context.CallbackUrl = (CallbackUri == null) ? "oob" : CallbackUri.ToString())
        .ForUri(RequestTokenUri)
        .SignWithoutToken();
    }

    public IConsumerRequest BuildExchangeRequestTokenForAccessTokenContext(IToken requestToken, string method, string verificationCode)
    {
      return Request()
        .ForMethod(method)
        .AlterContext(context=>context.Verifier = verificationCode)
        .ForUri(AccessTokenUri)
        .SignWithToken(requestToken);
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

    public IOAuthSession WithFormParameters(IDictionary dictionary)
    {
      return AddItems(_formParameters, dictionary);
    }

    public IOAuthSession WithFormParameters(object anonymousClass)
    {
      return AddItems(_formParameters, anonymousClass);
    }

    public IOAuthSession WithQueryParameters(IDictionary dictionary)
    {
      return AddItems(_queryParameters, dictionary);
    }

    public IOAuthSession WithQueryParameters(object anonymousClass)
    {
      return AddItems(_queryParameters, anonymousClass);
    }

    public IOAuthSession WithCookies(IDictionary dictionary)
    {
      return AddItems(_cookies, dictionary);
    }

    public IOAuthSession WithCookies(object anonymousClass)
    {
      return AddItems(_cookies, anonymousClass);
    }

    public IOAuthSession WithHeaders(IDictionary dictionary)
    {
      return AddItems(_headers, dictionary);
    }

    public IOAuthSession WithHeaders(object anonymousClass)
    {
      return AddItems(_headers, anonymousClass);
    }

    public IOAuthSession RequiresCallbackConfirmation()
    {
      CallbackMustBeConfirmed = true;
      return this;
    }

    static Uri ParseCallbackUri(string callBackUrl)
    {
      if (string.IsNullOrEmpty(callBackUrl)) return null;
      if (callBackUrl.Equals("oob", StringComparison.InvariantCultureIgnoreCase)) return null;
      return new Uri(callBackUrl);
    }

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