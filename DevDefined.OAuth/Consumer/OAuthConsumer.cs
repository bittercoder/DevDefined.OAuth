using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Consumer
{
    public class OAuthConsumer : IOAuthConsumer
    {
        private INonceGenerator _nonceGenerator = new GuidNonceGenerator();

        public INonceGenerator NonceGenerator
        {
            get { return _nonceGenerator; }
            set { _nonceGenerator = value; }
        } 

        public OAuthConsumer(Uri requestTokenUri, Uri userAuthorizeUri, Uri accessTokenUri)
        {
            RequestTokenUri = requestTokenUri;
            AccessTokenUri = accessTokenUri;
            UserAuthorizeUri = userAuthorizeUri;
            SignatureMethod = Core.SignatureMethod.PlainText;
        }

        public OAuthConsumer(string requestTokenUrl, string userAuthorizeUrl, string accessTokenUrl)
            : this(new Uri(requestTokenUrl), new Uri(userAuthorizeUrl), new Uri(accessTokenUrl))
        {
        }

        public string Realm { get; set; }

        #region IOAuthConsumer Members

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string SignatureMethod { get; set; }
        public AsymmetricAlgorithm Key { get; set; }

        public Uri RequestTokenUri { get; set; }
        public Uri AccessTokenUri { get; set; }
        public Uri UserAuthorizeUri { get; set; }

        public bool UseHeaderForOAuthParameters { get; set; }

        public IToken RequestToken(NameValueCollection additionalQueryParameters)
        {
            OAuthContext context = BuildRequestTokenContext(additionalQueryParameters);

            TokenBase token = context.InvokeHttpWebRequest(collection =>
                                                           new TokenBase
                                                               {
                                                                   ConsumerKey = ConsumerKey,
                                                                   Token =
                                                                       ParseResponseParameter(collection,
                                                                                              Parameters.OAuth_Token),
                                                                   TokenSecret =
                                                                       ParseResponseParameter(collection,
                                                                                              Parameters.
                                                                                                  OAuth_Token_Secret)
                                                               });

            return token;
        }

        public IToken ExchangeRequestTokenForAccessToken(IToken requestToken,
                                                            NameValueCollection additionalQueryParameters)
        {
            OAuthContext context = BuildExchangeRequestTokenForAccessTokenContext(requestToken,
                                                                                  additionalQueryParameters);

            TokenBase accessToken = context.InvokeHttpWebRequest(collection =>
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

            return accessToken;
        }

        public HttpWebResponse GetResponse(OAuthContext context, IToken accessToken)
        {
            SignContext(context, accessToken);

            Uri uri = context.GenerateUri();

            Console.WriteLine("Uri: {0}", uri);

            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = context.RequestMethod;

            if ((context.FormEncodedParameters != null) && (context.FormEncodedParameters.Count > 0))
            {
                request.ContentType = "application/x-www-form-urlencoded";
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(UriUtility.FormatQueryString(context.FormEncodedParameters));
                }
            }

            if (UseHeaderForOAuthParameters)
            {
                request.Headers[Parameters.OAuth_Authorization_Header] = context.GenerateOAuthParametersForHeader();
            }

            
            return (HttpWebResponse) request.GetResponse();
        }

        public OAuthContext BuildRequestTokenContext(NameValueCollection additionalQueryParameters)
        {
            EnsureStateIsValid();

            var factory = new OAuthContextFactory();
            var signer = new OAuthContextSigner();
            OAuthContext context = factory.FromUri("GET", RequestTokenUri);

            if (additionalQueryParameters != null) context.QueryParameters.Add(additionalQueryParameters);

            context.ConsumerKey = ConsumerKey;
            context.Realm = Realm;
            context.Realm = Realm;
            context.RequestMethod = "GET";
            context.SignatureMethod = SignatureMethod;
            context.Timestamp = DateTime.Now.EpocString();            
            context.Version = "1.0";

            context.Nonce = NonceGenerator.GenerateNonce(context);

            string signatureBase = context.GenerateSignatureBase();

            Console.WriteLine("signature_base: {0}", signatureBase);

            signer.SignContext(context,
                               new SigningContext
                                   {Algorithm = Key, SignatureBase = signatureBase, ConsumerSecret = ConsumerSecret});

            Console.WriteLine("oauth_singature: {0}", context.Signature);

            Uri uri = context.GenerateUri();

            Console.WriteLine("Uri: {0}", uri);

            return context;
        }

        public OAuthContext BuildExchangeRequestTokenForAccessTokenContext(IToken requestToken,
                                                                           NameValueCollection additionalQueryParameters)
        {
            EnsureStateIsValid();

            if (requestToken.ConsumerKey != ConsumerKey)
                throw Error.SuppliedTokenWasNotIssuedToThisConsumer(ConsumerKey, requestToken.ConsumerKey);

            var factory = new OAuthContextFactory();
            var signer = new OAuthContextSigner();
            OAuthContext context = factory.FromUri("GET", AccessTokenUri);

            if (additionalQueryParameters != null)
                context.QueryParameters.Add(additionalQueryParameters);

            context.ConsumerKey = ConsumerKey;
            context.Realm = Realm;
            context.Token = requestToken.Token;
            context.TokenSecret = requestToken.TokenSecret;
            context.RequestMethod = "GET";
            context.SignatureMethod = SignatureMethod;
            context.Timestamp = DateTime.Now.EpocString();            
            context.Version = "1.0";

            context.Nonce = NonceGenerator.GenerateNonce(context);

            string signatureBase = context.GenerateSignatureBase();

            Console.WriteLine("signature_base: {0}", signatureBase);

            signer.SignContext(context,
                               new SigningContext
                                   {Algorithm = Key, SignatureBase = signatureBase, ConsumerSecret = ConsumerSecret});

            Console.WriteLine("oauth_singature: {0}", context.Signature);

            Uri uri = context.GenerateUri();

            Console.WriteLine("Uri: {0}", uri);

            return context;
        }

        public void SignContext(OAuthContext context, IToken accessToken)
        {
            EnsureStateIsValid();

            if (accessToken.ConsumerKey != ConsumerKey)
                throw Error.SuppliedTokenWasNotIssuedToThisConsumer(ConsumerKey, accessToken.ConsumerKey);

            var signer = new OAuthContextSigner();
            
            context.UseAuthorizationHeader = UseHeaderForOAuthParameters;
            context.ConsumerKey = accessToken.ConsumerKey;
            context.Realm = accessToken.Realm;
            context.Token = accessToken.Token;
            context.TokenSecret = accessToken.TokenSecret;
            context.SignatureMethod = SignatureMethod;
            context.Timestamp = DateTime.Now.EpocString();
            context.Version = "1.0";
            
            context.Nonce = NonceGenerator.GenerateNonce(context);

            string signatureBase = context.GenerateSignatureBase();

            Console.WriteLine("signature_base: {0}", signatureBase);

            signer.SignContext(context,
                               new SigningContext
                                   {Algorithm = Key, SignatureBase = signatureBase, ConsumerSecret = ConsumerSecret});

            Console.WriteLine("oauth_singature: {0}", context.Signature);
        }

        public string GetUserAuthorizationUrlForToken(IToken token, string callbackUrl, NameValueCollection additionalParameters)
        {
            UriBuilder builder = new UriBuilder(UserAuthorizeUri);
            
            NameValueCollection collection = new NameValueCollection();
            
            if (builder.Query != null)
            {
                collection.Add(HttpUtility.ParseQueryString(builder.Query));
            }

            if (additionalParameters != null) collection.Add(additionalParameters);

            collection[Parameters.OAuth_Token] = token.Token;
            
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                collection[Parameters.OAuth_Callback] = callbackUrl;
            }

            builder.Query = "";

            return builder.Uri + "?" + UriUtility.FormatQueryStringWithUrlEncoding(collection);
        }

        #endregion

        private void EnsureStateIsValid()
        {
            if (string.IsNullOrEmpty(ConsumerKey)) throw Error.EmptyConsumerKey();
            if (string.IsNullOrEmpty(SignatureMethod)) throw Error.UnknownSignatureMethod(SignatureMethod);
            if ((SignatureMethod == Core.SignatureMethod.RsaSha1)
                && (Key == null)) throw Error.ForRsaSha1SignatureMethodYouMustSupplyAssymetricKeyParameter();
        }

        private static string ParseResponseParameter(NameValueCollection collection, string parameter)
        {
            string value = (collection[parameter] ?? "").Trim();
            return (value.Length > 0) ? value : null;
        }
    }
}