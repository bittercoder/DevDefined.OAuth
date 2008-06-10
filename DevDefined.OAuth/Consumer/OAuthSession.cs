using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Castle.Core;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Consumer
{
    public class OAuthSession : IOAuthSession
    {
        private readonly NameValueCollection _cookies = new NameValueCollection();
        private readonly NameValueCollection _formParameters = new NameValueCollection();
        private readonly NameValueCollection _headers = new NameValueCollection();
        private readonly NameValueCollection _queryParameters = new NameValueCollection();
        
        public IOAuthConsumerContext ConsumerContext { get; set;}
        public Uri RequestTokenUri { get; set; }
        public Uri AccessTokenUri { get; set; }
        public Uri UserAuthorizeUri { get; set; }
        public IToken AccessToken { get; set; }

        public OAuthSession(IOAuthConsumerContext consumerContext, Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri)
        {
            ConsumerContext = consumerContext;
            RequestTokenUri = requestTokenUri;
            AccessTokenUri = accessTokenUri;
            UserAuthorizeUri = userAuthorizeUri;            
        }

        public OAuthSession(IOAuthConsumerContext consumerContext, string requestTokenUrl, string userAuthorizeUrl, string accessTokenUrl)
            : this(consumerContext, new Uri(requestTokenUrl), new Uri(userAuthorizeUrl), new Uri(accessTokenUrl))
        {
        }
        
        public ConsumerRequest Request()
        {
            OAuthContext context = new OAuthContext();

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
            var token = BuildExchangeRequestTokenForAccessTokenContext(requestToken)
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
            UriBuilder builder = new UriBuilder(UserAuthorizeUri);
            
            NameValueCollection collection = new NameValueCollection();
            
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

            return builder.Uri + "?" + UriUtility.FormatQueryStringWithUrlEncoding(collection);
        }

        private static string ParseResponseParameter(NameValueCollection collection, string parameter)
        {
            string value = (collection[parameter] ?? "").Trim();
            return (value.Length > 0) ? value : null;
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

        private OAuthSession AddItems(NameValueCollection destination, object anonymousClass)
        {
            return AddItems(destination, new ReflectionBasedDictionaryAdapter(anonymousClass));
        }

        private OAuthSession AddItems(NameValueCollection destination, IDictionary additions)
        {
            foreach (string parameter in additions.Keys)
            {
                destination[parameter] = Convert.ToString(additions[parameter]);
            }

            return this;
        }
    }
}