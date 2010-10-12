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
using System.Text;
using DevDefined.OAuth.Framework;
using Xunit;

namespace DevDefined.OAuth.Tests.Framework
{
	public class OAuthContextTests
	{
		[Fact]
		public void generate_signature_when_token_is_url_encoded()
		{
			var context = new OAuthContext
			              	{
			              		RequestMethod = "GET",
			              		RawUri = new Uri("https://www.google.com/m8/feeds/contacts/default/base"),
			              		Token = "1/2",
			              		ConsumerKey = "context",
			              		SignatureMethod = SignatureMethod.RsaSha1
			              	};

			Assert.Equal(
				"GET&https%3A%2F%2Fwww.google.com%2Fm8%2Ffeeds%2Fcontacts%2Fdefault%2Fbase&oauth_consumer_key%3Dcontext%26oauth_signature_method%3DRSA-SHA1%26oauth_token%3D1%252F2",
				context.GenerateSignatureBase());

			Assert.Equal(
				"https://www.google.com/m8/feeds/contacts/default/base?oauth_token=1%2F2&oauth_consumer_key=context&oauth_signature_method=RSA-SHA1",
				context.GenerateUrl());
		}

		[Fact]
		public void generate_signature_with_hello_world_body()
		{
			// generate a signature base, as per the oauth body hash spec example
			// http://oauth.googlecode.com/svn/spec/ext/body_hash/1.0/oauth-bodyhash.html

			var context = new OAuthContext
			              	{
			              		RequestMethod = "POST",
			              		RawUri = new Uri("http://www.example.com/resource"),
			              		RawContentType = "text/plain",
			              		RawContent = Encoding.UTF8.GetBytes("Hello World!"),
			              		ConsumerKey = "consumer",
			              		SignatureMethod = "HMAC-SHA1",
			              		Timestamp = "1236874236",
			              		Version = "1.0",
			              		IncludeOAuthRequestBodyHashInSignature = true,
			              		Nonce = "10369470270925",
			              		Token = "token"
			              	};

			Assert.Equal(
				"POST&http%3A%2F%2Fwww.example.com%2Fresource&oauth_body_hash%3DLve95gjOVATpfV8EL5X4nxwjKHE%253D%26oauth_consumer_key%3Dconsumer%26oauth_nonce%3D10369470270925%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1236874236%26oauth_token%3Dtoken%26oauth_version%3D1.0",
				context.GenerateSignatureBase());
		}

		[Fact]
		public void generate_signature_for_empty_body()
		{
			var context = new OAuthContext();

			Assert.Equal("2jmj7l5rSw0yVb/vlWAYkK/YBwk=", context.GenerateBodyHash());
		}
	}
}