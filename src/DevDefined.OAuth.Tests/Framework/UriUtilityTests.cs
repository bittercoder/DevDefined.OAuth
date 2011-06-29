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

using System.Collections.Generic;
using System.Linq;
using DevDefined.OAuth.Framework;
using Xunit;
using QueryParameter = System.Collections.Generic.KeyValuePair<string, string>;

namespace DevDefined.OAuth.Tests.Framework
{
	public class UriUtilityTests
	{
		[Fact]
		public void GetHeaderParameters_ReturnsAllParameters()
		{
			List<QueryParameter> parameters =
				UriUtility.GetHeaderParameters("OAuth realm=\"http:\\\\somerealm.com\", oauth_consumer_key=\"consumerKey\"");

			Assert.Equal(2, parameters.Count);
			Assert.Equal("consumerKey", parameters.Single(p => p.Key == "oauth_consumer_key").Value);
			Assert.Equal(@"http:\\somerealm.com", parameters.Single(p => p.Key == "realm").Value);
		}

		[Fact]
		public void GetHeaderParametersWhenAuthorizationHeaderDoesNotContainOAuthReturnsEmptyCollection()
		{
			List<QueryParameter> parameters =
				UriUtility.GetHeaderParameters("realm=\"http:\\somerealm.com\", oauth_consumer_key=\"\"");

			Assert.Equal(0, parameters.Count);
		}

		[Fact]
		public void GetHeaderParametersWhenKeysValueIsEmpty()
		{
			List<QueryParameter> parameters =
				UriUtility.GetHeaderParameters("OAuth realm=\"http:\\somerealm.com\", oauth_consumer_key=\"\"");

			Assert.Equal("", parameters.Single(p => p.Key == "oauth_consumer_key").Value);
		}

		[Fact]
		public void GetQueryParametersWithoutQuestionMark()
		{
			List<QueryParameter> parameters = UriUtility.GetQueryParameters("key1=value1&key2=value2");
			Assert.Equal(2, parameters.Count);
			Assert.Equal("value1", parameters.Single(p => p.Key == "key1").Value);
			Assert.Equal("value2", parameters.Single(p => p.Key == "key2").Value);
		}

		[Fact]
		public void GetQueryParametersWithQuestionMark()
		{
			List<QueryParameter> parameters = UriUtility.GetQueryParameters("?key1=value1&key2=value2");
			Assert.Equal(2, parameters.Count);
			Assert.Equal("value1", parameters.Single(p => p.Key == "key1").Value);
			Assert.Equal("value2", parameters.Single(p => p.Key == "key2").Value);
		}

		[Fact]
		public void Issue8_WhenSignatureContainsEqualSign_ParseAuthorizationHeaderKeyValuePairDropsCharactersAfterEqualSign()
		{
			string signatureInHeader = "auth_signature=\"uZF3aYQFtyK0F1FFHY+w7/Be+m4=\"";

			var paramter = UriUtility.ParseAuthorizationHeaderKeyValuePair(signatureInHeader);

			Assert.Equal("auth_signature", paramter.Key);
			Assert.Equal("uZF3aYQFtyK0F1FFHY w7/Be m4=", paramter.Value);
		}

		[Fact]
		public void NormalizeRequestParameters_ReturnsParametersInOrdinalOrder()
		{
			var parameters = new Dictionary<string, string> {{"ZIP", "123"}, {"CVV", "123"}, {"ccid", "123"}};

			Assert.Equal("CVV=123&ZIP=123&ccid=123", UriUtility.NormalizeRequestParameters(parameters));
		}
	}
}