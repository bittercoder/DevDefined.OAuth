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
		const string OAuthAuthorizationHeaderStart = "OAuth";
		static readonly string[] HexEscapedUriRfc3986CharsToEscape;
		static readonly string[] QuoteCharacters = new[] {"\"", "'"};
		static readonly string[] UriRfc3986CharsToEscape = new[] {"!", "*", "'", "(", ")"};

		static UriUtility()
		{
			HexEscapedUriRfc3986CharsToEscape = UriRfc3986CharsToEscape.Select(c => Uri.HexEscape(c[0])).ToArray();
		}

		// see http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986 for details
		static string EscapeUriDataStringRfc3986(string value)
		{
			// Fix for the exception Uri.EscapeDataString throws when the string is longer than 32766
			// Microsoft documentation http://msdn.microsoft.com/en-us/library/system.uri.escapedatastring.aspx
			var escaped = new StringBuilder();
			const int maxChunkSize = 32766;
			for (int i = 0; i <= value.Length; i += maxChunkSize)
			{
				string substring = value.Substring(i, Math.Min(value.Length - i, maxChunkSize));
				escaped.Append(Uri.EscapeDataString(substring));
			}

			for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
			{
				escaped.Replace(UriRfc3986CharsToEscape[i], HexEscapedUriRfc3986CharsToEscape[i]);
			}

			return escaped.ToString();
		}

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
					if (!string.IsNullOrEmpty(s) && !s.StartsWith(Parameters.OAuthParameterPrefix) && !s.StartsWith(Parameters.XAuthParameterPrefix))
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
		/// Extracts all the parameters from the supplied encoded parameters. 
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static List<QueryParameter> GetHeaderParameters(string parameters)
		{
			parameters = parameters.Trim();

			var result = new List<QueryParameter>();

			if (!parameters.StartsWith(OAuthAuthorizationHeaderStart, StringComparison.InvariantCultureIgnoreCase))
			{
				return result;
			}

			parameters = parameters.Substring(OAuthAuthorizationHeaderStart.Length).Trim();

			if (!string.IsNullOrEmpty(parameters))
			{
				string[] p = parameters.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

				foreach (string s in p)
				{
					if (string.IsNullOrEmpty(s)) continue;
					QueryParameter parameter = ParseAuthorizationHeaderKeyValuePair(s);					
					result.Add(parameter);
				}
			}

			return result;
		}

		public static QueryParameter ParseAuthorizationHeaderKeyValuePair(string keyEqualValuePair)
		{
			int indexOfEqualSign = keyEqualValuePair.IndexOf('=');

			if (indexOfEqualSign > 0)
			{
				string key = keyEqualValuePair.Substring(0, indexOfEqualSign).Trim();

				string quotedValue = keyEqualValuePair.Substring(indexOfEqualSign + 1);

				string itemValue = StripQuotes(quotedValue);

				itemValue = HttpUtility.UrlDecode(itemValue);

				return new QueryParameter(key, itemValue);
			}

			return new QueryParameter(keyEqualValuePair.Trim(), string.Empty);
		}

		static string StripQuotes(string quotedValue)
		{
			foreach (string quoteCharacter in QuoteCharacters)
			{
				if (quotedValue.StartsWith(quoteCharacter) && quotedValue.EndsWith(quoteCharacter) && quotedValue.Length > 1)
				{
					return quotedValue.Substring(1, quotedValue.Length - 2);
				}
			}

			return quotedValue;
		}

		public static string UrlEncode(string value)
		{
			return EscapeUriDataStringRfc3986(value);
		}

		/// <summary>
		/// Formats a set of query parameters, as per query string encoding.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static string FormatQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
		{
			var builder = new StringBuilder();

			if (parameters != null)
			{
				foreach (var pair in parameters)
				{
					if (builder.Length > 0) builder.Append("&");
					builder.Append(pair.Key).Append("=");
					builder.Append(UrlEncode(pair.Value));
				}
			}

			return builder.ToString();
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
					builder.Append(key).Append("=");
					builder.Append(UrlEncode(parameters[key]));
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
				.OrderBy(x => x.Key, StringComparer.Ordinal)
				.ThenBy(x => x.Value)
				.Select(
					x => new QueryParameter(x.Key, UrlEncode(x.Value)));

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

		public static IEnumerable<QueryParameter> ToQueryParametersExcludingTokenSecret(this NameValueCollection source)
		{
			foreach (string key in source.AllKeys)
			{
				if (key != Parameters.OAuth_Token_Secret)
				{
					yield return new QueryParameter(key, source[key]);
				}
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