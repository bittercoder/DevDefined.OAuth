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

using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Consumer
{
  [TestFixture]
  public class OAuthSessionTests
  {
    [Test]
    public void GetRequestTokenForConsumerWithCallbackUrl()
    {
      var consumerContext = new OAuthConsumerContext {ConsumerKey = "key"};

      var session = new OAuthSession(consumerContext, "http://localhost/request",
                                     "http://localhost/userauth", "http://localhost/access", "http://localhost/callback");

      RequestDescription description = session.BuildRequestTokenContext("POST").GetRequestDescription();

      Assert.IsTrue(description.Body.Contains("oauth_callback=http%3A%2F%2Flocalhost%2Fcallback"));
    }

    [Test]
    public void GetRequestTokenForConsumerWithoutCallbackUrl()
    {
      var consumerContext = new OAuthConsumerContext {ConsumerKey = "key"};

      var session = new OAuthSession(consumerContext, "http://localhost/request",
                                     "http://localhost/userauth", "http://localhost/access");

      RequestDescription description = session.BuildRequestTokenContext("POST").GetRequestDescription();

      Assert.IsTrue(description.Body.Contains("oauth_callback=oob"));
    }

    [Test]
    public void GetRequestTokenForMethodGetDoesNotPopulateBody()
    {
      var consumerContext = new OAuthConsumerContext {ConsumerKey = "key"};

      var session = new OAuthSession(consumerContext, "http://localhost/request",
                                     "http://localhost/userauth", "http://localhost/access");

      RequestDescription description = session.BuildRequestTokenContext("GET").GetRequestDescription();

      Assert.IsNull(description.Body);
      Assert.IsNull(description.ContentType);
      Assert.AreEqual("GET", description.Method);
    }

    [Test]
    public void GetUserAuthorizationUriForTokenWithCallback()
    {
      var session = new OAuthSession(new OAuthConsumerContext(), "http://localhost/request",
                                     "http://localhost/userauth", "http://localhost/access");
      string actual = session.GetUserAuthorizationUrlForToken(new TokenBase {Token = "token"},
                                                              "http://localhost/callback");
      Assert.AreEqual(
        "http://localhost/userauth?oauth_token=token&oauth_callback=http%3A%2F%2Flocalhost%2Fcallback", actual);
    }

    [Test]
    public void GetUserAuthorizationUriForTokenWithoutCallback()
    {
      var session = new OAuthSession(new OAuthConsumerContext(), "http://localhost/request",
                                     "http://localhost/userauth", "http://localhost/access");
      string actual = session.GetUserAuthorizationUrlForToken(new TokenBase {Token = "token"}, null);
      Assert.AreEqual("http://localhost/userauth?oauth_token=token", actual);
    }

    [Test]
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

      Assert.IsFalse(description.Headers["Authorization"].Contains(Parameters.OAuth_Token_Secret));
    }

    [Test]
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

      Assert.IsFalse(description.Body.Contains(Parameters.OAuth_Token_Secret));
    }

    [Test]
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

      Assert.IsFalse(description.Url.ToString().Contains(Parameters.OAuth_Token_Secret));
    }
  }
}