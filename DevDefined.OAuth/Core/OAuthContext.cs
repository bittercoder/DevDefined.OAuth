using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        private NameValueCollection _authorizationHeaderParameters;
        private NameValueCollection _cookies;
        private NameValueCollection _formEncodedParameters;
        private NameValueCollection _headers;
        private string _normalizedRequestUrl;
        private NameValueCollection _queryParameters;
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

        public NameValueCollection Headers
        {
            get
            {
                if (_headers == null) _headers = new NameValueCollection();
                return _headers;
            }
            set { _headers = value; }
        }

        public NameValueCollection QueryParameters
        {
            get
            {
                if (_queryParameters == null) _queryParameters = new NameValueCollection();
                return _queryParameters;
            }
            set { _queryParameters = value; }
        }

        public NameValueCollection Cookies
        {
            get
            {
                if (_cookies == null) _cookies = new NameValueCollection();
                return _cookies;
            }
            set { _cookies = value; }
        }

        public NameValueCollection FormEncodedParameters
        {
            get
            {
                if (_formEncodedParameters == null) _formEncodedParameters = new NameValueCollection();
                return _formEncodedParameters;
            }
            set { _formEncodedParameters = value; }
        }

        public NameValueCollection AuthorizationHeaderParameters
        {
            get
            {
                if (_authorizationHeaderParameters == null) _authorizationHeaderParameters = new NameValueCollection();
                return _authorizationHeaderParameters;
            }
            set { _authorizationHeaderParameters = value; }
        }

        public Uri RawUri
        {
            get { return _rawUri; }
            set
            {
                _rawUri = value;
                
                var newParameters = HttpUtility.ParseQueryString(_rawUri.Query);

                // TODO: tidy this up, bit clunky

                foreach (string parameter in newParameters)
                {
                    QueryParameters[parameter] = newParameters[parameter];
                }

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

            if (Realm != null) builder.Append("realm=\"").Append(Realm).Append("\"");
        
            foreach (QueryParameter parameter in AuthorizationHeaderParameters.ToQueryParameters().Where(p=>p.Name != Parameters.Realm))
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