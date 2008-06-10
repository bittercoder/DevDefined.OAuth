using System;
using System.Web;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage;
using ExampleProviderSite.Repositories;

namespace ExampleProviderSite.Implementation
{
    public class SimpleTokenStore : ITokenStore
    {
        private readonly TokenRepository _repository;

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