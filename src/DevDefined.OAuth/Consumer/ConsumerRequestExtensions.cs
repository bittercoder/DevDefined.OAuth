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
using System.Net;
using System.Text;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Utility;

namespace DevDefined.OAuth.Consumer
{
	public static class ConsumerRequestExtensions
	{
		public static IConsumerRequest Get(this IConsumerRequest request)
		{
			return request.ForMethod("GET");
		}

		public static IConsumerRequest Delete(this IConsumerRequest request)
		{
			return request.ForMethod("DELETE");
		}

		public static IConsumerRequest Put(this IConsumerRequest request)
		{
			return request.ForMethod("PUT");
		}

		public static IConsumerRequest Post(this IConsumerRequest request)
		{
			return request.ForMethod("POST");
		}

		static void ApplyParameters(NameValueCollection destination, object anonymousClass)
		{
			ApplyParameters(destination, new ReflectionBasedDictionaryAdapter(anonymousClass));
		}

		static void ApplyParameters(NameValueCollection destination, IDictionary additions)
		{
			if (additions == null) throw new ArgumentNullException("additions");

			foreach (string parameter in additions.Keys)
			{
				destination[parameter] = Convert.ToString(additions[parameter]);
			}
		}

		public static IConsumerRequest ForMethod(this IConsumerRequest request, string method)
		{
			request.Context.RequestMethod = method;
			return request;
		}

		public static IConsumerRequest ForUri(this IConsumerRequest request, Uri uri)
		{
			request.Context.RawUri = uri;
			return request;
		}

		public static IConsumerRequest ForUrl(this IConsumerRequest request, string url)
		{
			request.Context.RawUri = new Uri(url);
			return request;
		}

		public static IConsumerRequest WithFormParameters(this IConsumerRequest request, IDictionary dictionary)
		{
			ApplyParameters(request.Context.FormEncodedParameters, dictionary);
			return request;
		}

		public static IConsumerRequest WithAcceptHeader(this IConsumerRequest request, string acceptsType)
		{
			request.AcceptsType = acceptsType;
			return request;
		}

		public static IConsumerRequest WithBody(this IConsumerRequest request, string requestBody)
		{
			request.RequestBody = requestBody;
			return request;
		}

		public static IConsumerRequest WithFormParameters(this IConsumerRequest request, object anonymousClass)
		{
			ApplyParameters(request.Context.FormEncodedParameters, anonymousClass);
			return request;
		}

		public static IConsumerRequest WithQueryParameters(this IConsumerRequest request, IDictionary dictionary)
		{
			ApplyParameters(request.Context.QueryParameters, dictionary);
			return request;
		}

		public static IConsumerRequest WithQueryParameters(this IConsumerRequest request, object anonymousClass)
		{
			ApplyParameters(request.Context.QueryParameters, anonymousClass);
			return request;
		}

		public static IConsumerRequest WithCookies(this IConsumerRequest request, IDictionary dictionary)
		{
			ApplyParameters(request.Context.Cookies, dictionary);
			return request;
		}

		public static IConsumerRequest WithCookies(this IConsumerRequest request, object anonymousClass)
		{
			ApplyParameters(request.Context.Cookies, anonymousClass);
			return request;
		}

		public static IConsumerRequest WithHeaders(this IConsumerRequest request, IDictionary dictionary)
		{
			ApplyParameters(request.Context.Headers, dictionary);
			return request;
		}

		public static IConsumerRequest WithHeaders(this IConsumerRequest request, object anonymousClass)
		{
			ApplyParameters(request.Context.Headers, anonymousClass);
			return request;
		}

		public static IConsumerRequest AlterContext(this IConsumerRequest request, Action<IOAuthContext> alteration)
		{
			alteration(request.Context);
			return request;
		}

		public static string ReadBody(this IConsumerRequest request)
		{
			HttpWebResponse response = request.ToWebResponse();

			return response.ReadToEnd();
		}

		public static T Select<T>(this IConsumerRequest request, Func<NameValueCollection, T> selectFunc)
		{
			try
			{
				return selectFunc(request.ToBodyParameters());
			}
			catch (ArgumentNullException argumentException)
			{
				if (argumentException.Message.Contains("Value cannot be null.\r\nParameter name: str"))
				{
					throw Error.ExperiencingIssueWithCreatingUriDueToMissingAppConfig(argumentException);
				}

				throw Error.FailedToParseResponse(request.ToString());
			}
		}

		public static IConsumerRequest IncludeRequestBodyHash(this IConsumerRequest request)
		{
			request.Context.IncludeOAuthRequestBodyHashInSignature = true;
			return request;
		}

		public static IConsumerRequest WithRawContent(this IConsumerRequest request, string rawContent, Encoding encoding, bool addHash)
		{
			return WithRawContent(request, encoding.GetBytes(rawContent), addHash);
		}

		public static IConsumerRequest WithRawContent(this IConsumerRequest request, byte[] rawContent, bool addHash)
		{
			request.Context.RawContent = rawContent;
			request.Context.IncludeOAuthRequestBodyHashInSignature = addHash;
			return request;
		}

		public static IConsumerRequest WithRawContent(this IConsumerRequest request, string rawContent, Encoding encoding)
		{
			return WithRawContent(request, encoding.GetBytes(rawContent));
		}

		public static IConsumerRequest WithRawContent(this IConsumerRequest request, byte[] rawContent)
		{
			request.Context.RawContent = rawContent;
			return request;
		}

		public static IConsumerRequest WithRawContentType(this IConsumerRequest request, string rawContentType)
		{
			request.Context.RawContentType = rawContentType;
			return request;
		}

		/// <summary>
		/// Override the default request timeout in milliseconds.
		/// Sets the <see cref="System.Net.HttpWebRequest.Timeout"/> property.
		/// </summary>
		public static IConsumerRequest WithTimeout(this IConsumerRequest request, int timeout)
		{
			request.Timeout = timeout;
			return request;
		}
	}
}