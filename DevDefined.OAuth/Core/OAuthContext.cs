using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace DevDefined.OAuth.Core
{
    public class OAuthContext : IToken
    {
        private readonly BoundParameter _consumerKey;
        private readonly BoundParameter _nonce;
        private readonly BoundParameter _signature;
        private readonly BoundParameter _signatureMethod;
        private readonly BoundParameter _timestamp;
        private readonly BoundParameter _token;
        private readonly BoundParameter _tokenSecret;
        private readonly BoundParameter _version;
        private string _normalizedRequestUrl;
        private Uri _rawUri;

        public OAuthContext()
        {
            _consumerKey = new BoundParameter(Parameters.OAuth_Consumer_Key, this);
            _nonce = new BoundParameter(Parameters.OAuth_Nonce, this);
            _signature = new BoundParameter(Parameters.OAuth_Signature, this);
            _signatureMethod = new BoundParameter(Parameters.OAuth_Signature_Method, this);
            _timestamp = new BoundParameter(Parameters.OAuth_Timestamp, this);
            _token = new BoundParameter(Parameters.OAuth_Token, this);
            _tokenSecret = new BoundParameter(Parameters.OAuth_Token_Secret, this);
            _version = new BoundParameter(Parameters.OAuth_Version, this);

            FormEncodedParameters = new NameValueCollection();
            Cookies = new NameValueCollection();
            Headers = new NameValueCollection();
            AuthorizationHeaderParameters = new NameValueCollection();
        }

        public NameValueCollection Headers { get; set; }
        public NameValueCollection QueryParameters { get; set; }
        public NameValueCollection Cookies { get; set; }
        public NameValueCollection FormEncodedParameters { get; set; }
        public NameValueCollection AuthorizationHeaderParameters { get; set; }

        public Uri RawUri
        {
            get { return _rawUri; }
            set
            {
                _rawUri = value;

                QueryParameters = HttpUtility.ParseQueryString(_rawUri.Query);

                _normalizedRequestUrl = UriUtility.NormalizeUri(_rawUri);
            }
        }

        public string NormalizedRequestUrl
        {
            get { return _normalizedRequestUrl; }
        }

        public string RequestMethod { get; set; }

        public string Nonce
        {
            get { return _nonce.Value; }
            set { _nonce.Value = value; }
        }

        public string Signature
        {
            get { return _signature.Value; }
            set { _signature.Value = value; }
        }

        public string SignatureMethod
        {
            get { return _signatureMethod.Value; }
            set { _signatureMethod.Value = value; }
        }

        public string Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp.Value = value; }
        }

        public string Version
        {
            get { return _version.Value; }
            set { _version.Value = value; }
        }

        public bool UseAuthorizationHeader { get; set; }

        #region IToken Members

        public string Realm
        {
            get { return AuthorizationHeaderParameters[Parameters.Realm]; }
            set { AuthorizationHeaderParameters[Parameters.Realm] = value; }
        }

        public string ConsumerKey
        {
            get { return _consumerKey.Value; }
            set { _consumerKey.Value = value; }
        }

        public string Token
        {
            get { return _token.Value; }
            set { _token.Value = value; }
        }

        public string TokenSecret
        {
            get { return _tokenSecret.Value; }
            set { _tokenSecret.Value = value; }
        }

        #endregion

        public Uri GenerateUri()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            string formattedQuery = UriUtility.FormatQueryString(QueryParameters);

            builder.Query = UriUtility.FormatQueryString(QueryParameters);
            return builder.Uri;
        }

        public string GenerateUrl()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            builder.Query = "";

            return builder.Uri + "?" + UriUtility.FormatQueryString(QueryParameters);
        }

        public string GenerateOAuthParametersForHeader()
        {
            var builder = new StringBuilder();

            foreach (QueryParameter parameter in AuthorizationHeaderParameters.ToQueryParameters())
            {
                if (builder.Length > 0) builder.Append(",");
                builder.Append(UriUtility.UrlEncode(parameter.Name)).Append("=\"").Append(
                    UriUtility.UrlEncode(parameter.Value)).Append("\"");
            }

            builder.Insert(0, "OAuth ");

            return builder.ToString();
        }

        public Uri GenerateUriWithoutOAuthParameters()
        {
            var builder = new UriBuilder(NormalizedRequestUrl);

            NameValueCollection parameters = QueryParameters.ToQueryParameters()
                .Where(q => !q.Name.StartsWith(Parameters.OAuthParameterPrefix))
                .ToNameValueCollection();

            builder.Query = UriUtility.FormatQueryString(parameters);

            return builder.Uri;
        }

        public HttpWebRequest GenerateHttpWebRequest()
        {
            var request = (HttpWebRequest) WebRequest.Create(GenerateUri());

            request.Method = RequestMethod;

            if (Headers != null) request.Headers.Add(Headers);

            // TODO: implement support for cookies

            return request;
        }

        public T InvokeHttpWebRequest<T>(Func<NameValueCollection, T> selectFunc)
        {
            try
            {
                HttpWebRequest request = GenerateHttpWebRequest();
                var response = (HttpWebResponse) request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string encodedFormParameters = reader.ReadToEnd();

                    try
                    {
                        NameValueCollection responseValues = HttpUtility.ParseQueryString(encodedFormParameters);

                        return selectFunc(responseValues);
                    }
                    catch (ArgumentNullException)
                    {
                        throw Error.FailedToParseResponse(encodedFormParameters);
                    }
                }
            }
            catch (WebException webEx)
            {
                throw Error.RequestFailed(webEx);
            }
        }

        public string GenerateSignatureBase()
        {
            if (Token == null)
            {
                Token = string.Empty;
            }

            if (string.IsNullOrEmpty(ConsumerKey))
            {
                throw Error.MissingRequiredOAuthParameter(this, Parameters.OAuth_Consumer_Key);
            }

            if (string.IsNullOrEmpty(SignatureMethod))
            {
                throw Error.MissingRequiredOAuthParameter(this, Parameters.OAuth_Signature_Method);
            }

            if (string.IsNullOrEmpty(RequestMethod))
            {
                throw Error.RequestMethodHasNotBeenAssigned("RequestMethod");
            }

            var allParameters = new List<QueryParameter>();

            if (FormEncodedParameters != null) allParameters.AddRange(FormEncodedParameters.ToQueryParameters());
            if (QueryParameters != null) allParameters.AddRange(QueryParameters.ToQueryParameters());
            if (Cookies != null) allParameters.AddRange(Cookies.ToQueryParameters());
            if (AuthorizationHeaderParameters != null)
                allParameters.AddRange(
                    AuthorizationHeaderParameters.ToQueryParameters().Where(q => q.Name != Parameters.Realm));

            // remove the signature parameter

            allParameters.RemoveAll(param => param.Name == Parameters.OAuth_Signature);

            // build the uri

            return UriUtility.FormatParameters(RequestMethod, new Uri(NormalizedRequestUrl), allParameters);
        }

        #region Nested type: BoundParameter

        private class BoundParameter
        {
            private readonly OAuthContext _context;
            private readonly string _name;

            public BoundParameter(string name, OAuthContext context)
            {
                _name = name;
                _context = context;
            }

            public string Value
            {
                get { return Collection[_name]; }
                set
                {
                    if (value == null)
                    {
                        Collection.Remove(_name);
                    }
                    else
                    {
                        Collection[_name] = value;
                    }
                }
            }

            private NameValueCollection Collection
            {
                get
                {
                    if (_context.UseAuthorizationHeader) return _context.AuthorizationHeaderParameters;
                    if (_context.RequestMethod != "GET") return _context.FormEncodedParameters;
                    return _context.QueryParameters;
                }
            }
        }

        #endregion
    }
}