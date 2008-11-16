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
using ExampleProviderSite.Repositories;

namespace ExampleProviderSite.Implementation
{
  public class SimpleTokenStore : ITokenStore
  {
    readonly TokenRepository _repository;

    public SimpleTokenStore(TokenRepository repository)
    {
      _repository = repository;
    }

    #region ITokenStore Members

    public IToken CreateRequestToken(OAuthContext context)
    {
      var token = new Models.RequestToken
        {
          ConsumerKey = context.ConsumerKey,
          Realm = context.Realm,
          Token = Guid.NewGuid().ToString(),
          TokenSecret = Guid.NewGuid().ToString()
        };

      _repository.SaveRequestToken(token);

      return token;
    }

    public void ConsumeRequestToken(OAuthContext requestContext)
    {
      Models.RequestToken requestToken = _repository.GetRequestToken(requestContext.Token);

      if (requestToken.UsedUp)
      {
        throw new OAuthException(requestContext, OAuthProblems.TokenRejected,
                                 "The request token has already be consumed.");
      }

      requestToken.UsedUp = true;

      _repository.SaveRequestToken(requestToken);
    }

    public void ConsumeAccessToken(OAuthContext accessContext)
    {
      Models.AccessToken accessToken = _repository.GetAccessToken(accessContext.Token);

      if (accessToken.ExpireyDate < DateTime.Now)
      {
        throw new OAuthException(accessContext, OAuthProblems.TokenExpired,
                                 "Token has expired (they're only valid for 1 minute)");
      }
    }

    public IToken GetAccessTokenAssociatedWithRequestToken(OAuthContext requestContext)
    {
      Models.RequestToken request = _repository.GetRequestToken(requestContext.Token);
      return request.AccessToken;
    }

    public RequestForAccessStatus GetStatusOfRequestForAccess(OAuthContext accessContext)
    {
      Models.RequestToken request = _repository.GetRequestToken(accessContext.Token);

      if (request.AccessDenied) return RequestForAccessStatus.Denied;

      if (request.AccessToken == null) return RequestForAccessStatus.Unknown;

      return RequestForAccessStatus.Granted;
    }

    #endregion
  }
}