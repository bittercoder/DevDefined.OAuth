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
using DevDefined.OAuth.Utility;

namespace DevDefined.OAuth.Storage.Basic
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

		public IToken CreateRequestToken(IOAuthContext context)
		{
			if (context == null) throw new ArgumentNullException("context");

			var token = new RequestToken
			            	{
			            		ConsumerKey = context.ConsumerKey,
			            		Realm = context.Realm,
			            		Token = Guid.NewGuid().ToString(),
			            		TokenSecret = Guid.NewGuid().ToString(),
			            		CallbackUrl = context.CallbackUrl
			            	};

			_requestTokenRepository.SaveToken(token);

			return token;
		}

    /// <summary>
    /// Create an access token using xAuth.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public IToken CreateAccessToken(IOAuthContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      var accessToken = new AccessToken
        {
          ConsumerKey = context.ConsumerKey,
          ExpiryDate = DateTime.UtcNow.AddDays(20),
          Realm = context.Realm,
          Token = Guid.NewGuid().ToString(),
          TokenSecret = Guid.NewGuid().ToString(),
          UserName = Guid.NewGuid().ToString(),
        };

      _accessTokenRepository.SaveToken(accessToken);

      return accessToken;
    }

		public void ConsumeRequestToken(IOAuthContext requestContext)
		{
			if (requestContext == null) throw new ArgumentNullException("requestContext");

			RequestToken requestToken = GetRequestToken(requestContext);

			UseUpRequestToken(requestContext, requestToken);

			_requestTokenRepository.SaveToken(requestToken);
		}

		public void ConsumeAccessToken(IOAuthContext accessContext)
		{
			AccessToken accessToken = GetAccessToken(accessContext);

			if (accessToken.ExpiryDate < Clock.Now)
			{
				throw new OAuthException(accessContext, OAuthProblems.TokenExpired, "Token has expired");
			}
		}

		public IToken GetAccessTokenAssociatedWithRequestToken(IOAuthContext requestContext)
		{
			RequestToken requestToken = GetRequestToken(requestContext);
			return requestToken.AccessToken;
		}

		public RequestForAccessStatus GetStatusOfRequestForAccess(IOAuthContext accessContext)
		{
			RequestToken request = GetRequestToken(accessContext);

			if (request.AccessDenied) return RequestForAccessStatus.Denied;

			if (request.AccessToken == null) return RequestForAccessStatus.Unknown;

			return RequestForAccessStatus.Granted;
		}

		public string GetCallbackUrlForToken(IOAuthContext requestContext)
		{
			RequestToken requestToken = GetRequestToken(requestContext);
			return requestToken.CallbackUrl;
		}

		public string GetVerificationCodeForRequestToken(IOAuthContext requestContext)
		{
			RequestToken requestToken = GetRequestToken(requestContext);

			return requestToken.Verifier;
		}

		public string GetRequestTokenSecret(IOAuthContext context)
		{
			RequestToken requestToken = GetRequestToken(context);

			return requestToken.TokenSecret;
		}

		public string GetAccessTokenSecret(IOAuthContext context)
		{
			AccessToken token = GetAccessToken(context);

			return token.TokenSecret;
		}

		public IToken RenewAccessToken(IOAuthContext requestContext)
		{
			throw new NotImplementedException();
		}

		RequestToken GetRequestToken(IOAuthContext context)
		{
			try
			{
				return _requestTokenRepository.GetToken(context.Token);
			}
			catch (Exception exception)
			{
				// TODO: log exception
				throw Error.UnknownToken(context, context.Token, exception);
			}
		}

		AccessToken GetAccessToken(IOAuthContext context)
		{
			try
			{
				return _accessTokenRepository.GetToken(context.Token);
			}
			catch (Exception exception)
			{
				// TODO: log exception
				throw Error.UnknownToken(context, context.Token, exception);
			}
		}

		public IToken GetToken(IOAuthContext context)
		{
			var token = (IToken) null;
			if (!string.IsNullOrEmpty(context.Token))
			{
				try
				{
					token = _accessTokenRepository.GetToken(context.Token) ??
					        (IToken) _requestTokenRepository.GetToken(context.Token);
				}
				catch (Exception ex)
				{
					// TODO: log exception
					throw Error.UnknownToken(context, context.Token, ex);
				}
			}
			return token;
		}

	  static void UseUpRequestToken(IOAuthContext requestContext, RequestToken requestToken)
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