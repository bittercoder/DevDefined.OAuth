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
using DevDefined.OAuth.Wcf.Models;
using DevDefined.OAuth.Wcf.Repositories;

namespace DevDefined.OAuth.Wcf.Implementation
{
  public class SimpleTokenStore : ITokenStore
  {
    readonly ITokenRepository<AccessToken> _accessTokenRepository;
    readonly ITokenRepository<RequestToken> _requestTokenRepository;

    public SimpleTokenStore(ITokenRepository<AccessToken> accessTokenRepository, ITokenRepository<RequestToken> requestTokenRepository)
    {
      if (accessTokenRepository == null) throw new ArgumentNullException("accessTokenRepository");
      if (requestTokenRepository == null) throw new ArgumentNullException("requestTokenRepository");
      _accessTokenRepository = accessTokenRepository;
      _requestTokenRepository = requestTokenRepository;
    }

    #region ITokenStore Members

    public IToken CreateRequestToken(OAuthContext context)
    {
      if (context == null) throw new ArgumentNullException("context");

      var token = new RequestToken
        {
          ConsumerKey = context.ConsumerKey,
          Realm = context.Realm,
          Token = Guid.NewGuid().ToString(),
          TokenSecret = Guid.NewGuid().ToString()
        };

      _requestTokenRepository.SaveToken(token);

      return token;
    }

    public void ConsumeRequestToken(OAuthContext requestContext)
    {
      if (requestContext == null) throw new ArgumentNullException("requestContext");

      RequestToken requestToken = _requestTokenRepository.GetToken(requestContext.Token);

      UseUpRequestToken(requestContext, requestToken);

      _requestTokenRepository.SaveToken(requestToken);
    }

    public void ConsumeAccessToken(OAuthContext accessContext)
    {
      AccessToken accessToken = _accessTokenRepository.GetToken(accessContext.Token);

      if (accessToken.ExpireyDate < DateTime.Now)
      {
        throw new OAuthException(accessContext, OAuthProblems.TokenExpired,
                                 "Token has expired (they're only valid for 1 minute)");
      }
    }

    public IToken GetAccessTokenAssociatedWithRequestToken(OAuthContext requestContext)
    {
      RequestToken request = _requestTokenRepository.GetToken(requestContext.Token);
      return request.AccessToken;
    }

    public RequestForAccessStatus GetStatusOfRequestForAccess(OAuthContext accessContext)
    {
      RequestToken request = _requestTokenRepository.GetToken(accessContext.Token);

      if (request.AccessDenied) return RequestForAccessStatus.Denied;

      if (request.AccessToken == null) return RequestForAccessStatus.Unknown;

      return RequestForAccessStatus.Granted;
    }

    #endregion

    static void UseUpRequestToken(OAuthContext requestContext, RequestToken requestToken)
    {
      if (requestToken.UsedUp)
      {
        throw new OAuthException(requestContext, OAuthProblems.TokenRejected,
                                 "The request token has already be consumed.");
      }

      requestToken.UsedUp = true;
    }
  }
}