using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using QueryParameter = System.Collections.Generic.KeyValuePair<string, string>;

namespace DevDefined.OAuth.Framework
{
    public static class UriUtility
    {
        private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Extracts all the parameters from the supplied encoded parameters. 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(Parameters.OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// An OAuth specific implementation of Url Encoding - we need this to produce consistent base
        /// strings to other platforms such as php or ruby, as the <see cref="HttpUtility.UrlEncode" />
        /// doesn't encode in the same way.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(string value)
        {
            if (value == null) return null;

            var result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int) symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Formats a set of query parameters, as per query string encoding.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string FormatQueryString(NameValueCollection parameters)
        {
            var builder = new StringBuilder();

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    if (builder.Length > 0) builder.Append("&");
                    builder.Append(key).Append("=").Append(UrlEncode(parameters[key]));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Takes an http method, url and a set of parameters and formats them as a signature base as per the OAuth core spec.
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string FormatParameters(string httpMethod, Uri url, List<QueryParameter> parameters)
        {
            string normalizedRequestParameters = NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());

            signatureBase.AppendFormat("{0}&", UrlEncode(NormalizeUri(url)));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Normalizes a sequence of key/value pair parameters as per the OAuth core specification.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string NormalizeRequestParameters(IEnumerable<QueryParameter> parameters)
        {
            IEnumerable<QueryParameter> orderedParameters = parameters
                .OrderBy(x => x.Key)
                .ThenBy(x => x.Value)
                .Select(
                x =>
                (x.Key.StartsWith(Parameters.OAuthParameterPrefix))
                    ? x
                    : new QueryParameter(x.Key, UrlEncode(x.Value)));

            var builder = new StringBuilder();

            foreach (var parameter in orderedParameters)
            {
                if (builder.Length > 0) builder.Append("&");

                builder.Append(parameter.Key).Append("=").Append(parameter.Value);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Normalizes a Url according to the OAuth specification (this ensures http or https on a default port is not displayed
        /// with the :XXX following the host in the url).
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string NormalizeUri(Uri uri)
        {
            string normalizedUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);

            if (!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)))
            {
                normalizedUrl += ":" + uri.Port;
            }

            return normalizedUrl + ((uri.AbsolutePath == "/") ? "" : uri.AbsolutePath);
        }

        public static IEnumerable<QueryParameter> ToQueryParameters(this NameValueCollection source)
        {
            foreach (string key in source.AllKeys)
            {
                yield return new QueryParameter(key, source[key]);
            }
        }

        public static NameValueCollection ToNameValueCollection(this IEnumerable<QueryParameter> source)
        {
            var collection = new NameValueCollection();

            foreach (var parameter in source)
            {
                collection[parameter.Key] = parameter.Value;
            }

            return collection;
        }

        public static string FormatTokenForResponse(IToken token)
        {
            var builder = new StringBuilder();

            builder.Append(Parameters.OAuth_Token).Append("=").Append(UrlEncode(token.Token))
                .Append("&").Append(Parameters.OAuth_Token_Secret).Append("=").Append(UrlEncode(token.TokenSecret));

            return builder.ToString();
        }
    }
}