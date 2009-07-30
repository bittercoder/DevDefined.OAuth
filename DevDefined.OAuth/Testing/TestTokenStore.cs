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
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Testing
{
  public class TestTokenStore : ITokenStore
  {
    public string CallbackUrl { get; set; }
    public string VerificationCode { get; set; }

    public TestTokenStore()
    {
      CallbackUrl = "http://localhost/callback";
      VerificationCode = "GzvVb5WjWfHKa/0JuFupaMyn"; // this is a example google oauth verification code
    }

    #region ITokenStore Members

    public IToken CreateRequestToken(IOAuthContext context)
    {
      EnsureTestConsumer(context);

      return new TokenBase
        {ConsumerKey = "key", Realm = null, Token = "requestkey", TokenSecret = "requestsecret"};
    }

    public void ConsumeRequestToken(IOAuthContext requestContext)
    {
      EnsureTestConsumer(requestContext);

      if (requestContext.Token != "requestkey")
        throw new OAuthException(requestContext, OAuthProblems.TokenRejected,
                                 "The supplied request token is unknown to teh provider.");
    }

    public void ConsumeAccessToken(IOAuthContext accessContext)
    {
      EnsureTestConsumer(accessContext);

      if (accessContext.Token != "accesskey")
        throw new OAuthException(accessContext, OAuthProblems.TokenRejected,
                                 "The supplied access token is unknown to teh provider.");
    }

    public IToken GetAccessTokenAssociatedWithRequestToken(IOAuthContext requestContext)
    {
      EnsureTestConsumer(requestContext);

      if (requestContext.Token != "requestkey")
        throw new OAuthException(requestContext, OAuthProblems.TokenRejected, "Expected Token \"requestkey\"");

      return new TokenBase {ConsumerKey = "key", Realm = null, Token = "accesskey", TokenSecret = "accesssecret"};
    }

    public RequestForAccessStatus GetStatusOfRequestForAccess(IOAuthContext requestContext)
    {
      if (requestContext.ConsumerKey == "key" && requestContext.Token == "requestkey")
        return RequestForAccessStatus.Granted;

      return RequestForAccessStatus.Unknown;
    }

    public string GetCallbackUrlForToken(IToken token)
    {
      return CallbackUrl;
    }

    public void SetVerificationCodeForToken(IToken token, string verificationCode)
    {
      VerificationCode = verificationCode;
    }

    public string GetVerificationCodeForToken(IToken token)
    {
      return VerificationCode;
    }

    #endregion

    public IToken CreateAccessTokenForRequestToken(IOAuthContext requestContext)
    {
      EnsureTestConsumer(requestContext);

      return new TokenBase {ConsumerKey = "key", Realm = null, Token = "accesskey", TokenSecret = "accesssecret"};
    }

    static void EnsureTestConsumer(IConsumer consumer)
    {
      if (consumer == null) throw new ArgumentNullException("consumer");
      if (consumer.Realm != null)
        throw new OAuthException(consumer as OAuthContext, OAuthProblems.ConsumerKeyRejected,
                                 "supplied realm was unknown to the provider");
      if (consumer.ConsumerKey != "key")
        throw new OAuthException(consumer as OAuthContext, OAuthProblems.ConsumerKeyRejected,
                                 "supplied consumer key was unknown to the provider");
    }
  }
}