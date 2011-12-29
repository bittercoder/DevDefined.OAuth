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

using System.Text;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Xunit;

namespace DevDefined.OAuth.Tests.Consumer
{
	public class OAuthSessionTests
	{
		[Fact]
		public void GetRequestTokenForConsumerWithCallbackUrl()
		{
			var consumerContext = new OAuthConsumerContext {ConsumerKey = "key"};

			var session = new OAuthSession(consumerContext, "http://localhost/request",
			                               "http://localhost/userauth", "http://localhost/access", "http://localhost/callback");

			RequestDescription description = session.BuildRequestTokenContext("POST").GetRequestDescription();

			Assert.True(description.Body.Contains("oauth_callback=http%3A%2F%2Flocalhost%2Fcallback"));
		}

		[Fact]
		public void GetRequestTokenForConsumerWithoutCallbackUrl()
		{
			var consumerContext = new OAuthConsumerContext {ConsumerKey = "key"};

			var session = new OAuthSession(consumerContext, "http://localhost/request", "http://localhost/userauth", "http://localhost/access");

			RequestDescription description = session.BuildRequestTokenContext("POST").GetRequestDescription();

			Assert.True(description.Body.Contains("oauth_callback=oob"));
		}

		[Fact]
		public void GetRequestTokenForMethodGetDoesNotPopulateBody()
		{
			var consumerContext = new OAuthConsumerContext {ConsumerKey = "key"};

			var session = new OAuthSession(consumerContext, "http://localhost/request",
			                               "http://localhost/userauth", "http://localhost/access");

			RequestDescription description = session.BuildRequestTokenContext("GET").GetRequestDescription();

			Assert.Null(description.Body);
			Assert.Null(description.ContentType);
			Assert.Equal("GET", description.Method);
		}

		[Fact]
		public void GetUserAuthorizationUriForTokenWithCallback()
		{
			var session = new OAuthSession(new OAuthConsumerContext(), "http://localhost/request",
			                               "http://localhost/userauth", "http://localhost/access");
			string actual = session.GetUserAuthorizationUrlForToken(new TokenBase {Token = "token"},
			                                                        "http://localhost/callback");
			Assert.Equal(
				"http://localhost/userauth?oauth_token=token&oauth_callback=http%3A%2F%2Flocalhost%2Fcallback", actual);
		}

		[Fact]
		public void GetUserAuthorizationUriForTokenWithoutCallback()
		{
			var session = new OAuthSession(new OAuthConsumerContext(), "http://localhost/request",
			                               "http://localhost/userauth", "http://localhost/access");
			string actual = session.GetUserAuthorizationUrlForToken(new TokenBase {Token = "token"}, null);
			Assert.Equal("http://localhost/userauth?oauth_token=token", actual);
		}

		[Fact]
		public void TokenSecretNotIncludedInAuthorizationHeaderForPostRequestWithUseAuthorizationHeaders()
		{
			var session = new OAuthSession(new OAuthConsumerContext {ConsumerKey = "consumer", UseHeaderForOAuthParameters = true}, "http://localhost/request",
			                               "http://localhost/userauth", "http://localhost/access");

			var accessToken = new TokenBase {ConsumerKey = "consumer", Token = "token", TokenSecret = "secret"};

			RequestDescription description = session
				.Request(accessToken)
				.Post()
				.ForUrl("http://localhost/")
				.SignWithToken()
				.GetRequestDescription();

			Assert.False(description.Headers["Authorization"].Contains(Parameters.OAuth_Token_Secret));
		}

		[Fact]
		public void TokenSecretNotIncludedInBodyParametersForPostRequest()
		{
			var session = new OAuthSession(new OAuthConsumerContext {ConsumerKey = "consumer"}, "http://localhost/request",
			                               "http://localhost/userauth", "http://localhost/access");

			var accessToken = new TokenBase {ConsumerKey = "consumer", Token = "token", TokenSecret = "secret"};

			RequestDescription description = session
				.Request(accessToken)
				.Post()
				.ForUrl("http://localhost/")
				.SignWithToken()
				.GetRequestDescription();

			Assert.False(description.Body.Contains(Parameters.OAuth_Token_Secret));
		}

		[Fact]
		public void TokenSecretNotIncludedInQueryParametersForGetRequest()
		{
			var session = new OAuthSession(new OAuthConsumerContext {ConsumerKey = "consumer"}, "http://localhost/request",
			                               "http://localhost/userauth", "http://localhost/access");

			var accessToken = new TokenBase {ConsumerKey = "consumer", Token = "token", TokenSecret = "secret"};

			RequestDescription description = session
				.Request(accessToken)
				.Get()
				.ForUrl("http://localhost/")
				.SignWithToken()
				.GetRequestDescription();

			Assert.False(description.Url.ToString().Contains(Parameters.OAuth_Token_Secret));
		}

		[Fact]
		public void generate_request_with_raw_body_includes_body_hash()
		{
			var session = new OAuthSession(new OAuthConsumerContext {ConsumerKey = "consumer", UseHeaderForOAuthParameters = true}, "http://localhost/request", "http://localhost/userauth",
			                               "http://localhost/access");

			var accessToken = new TokenBase {ConsumerKey = "consumer", Token = "token", TokenSecret = "secret"};

			byte[] rawContents = Encoding.UTF8.GetBytes("Hello World!");

			IConsumerRequest content = session
				.EnableOAuthRequestBodyHashes()
				.Request(accessToken)
				.Post()
				.ForUrl("http://localhost/resource")
				.WithRawContent(rawContents);

			RequestDescription description = content.GetRequestDescription();

			Assert.Equal(rawContents, description.RawBody);

			Assert.Contains("oauth_body_hash=\"Lve95gjOVATpfV8EL5X4nxwjKHE%3D\"", description.Headers[Parameters.OAuth_Authorization_Header]);
		}

        [Fact]
        public void create_session_using_context_only_constructor_does_not_throw()
        {
            var session = new OAuthSession(new OAuthConsumerContext());

            Assert.NotNull(session);
        }
	}
}