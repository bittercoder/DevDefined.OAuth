using System;
using DevDefined.OAuth.Core;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Testing
{
    public class TestTokenStore : ITokenStore
    {
        #region ITokenStore Members

        public IToken CreateRequestToken(OAuthContext context)
        {
            EnsureTestConsumer(context);

            return new TokenBase
                       {ConsumerKey = "key", Realm = null, Token = "requestkey", TokenSecret = "requestsecret"};
        }

        public void ConsumeRequestToken(OAuthContext requestContext)
        {
            EnsureTestConsumer(requestContext);

            if (requestContext.Token != "requestkey")
                throw new OAuthException(requestContext, OAuthProblems.TokenRejected,
                                         "The supplied request token is unknown to teh provider.");
        }

        public IToken CreateAccessTokenForRequestToken(OAuthContext requestContext)
        {
            EnsureTestConsumer(requestContext);

            return new TokenBase {ConsumerKey = "key", Realm = null, Token = "accesskey", TokenSecret = "accesssecret"};
        }

        public void ConsumeAccessToken(OAuthContext accessContext)
        {
            EnsureTestConsumer(accessContext);

            if (accessContext.Token != "accesskey")
                throw new OAuthException(accessContext, OAuthProblems.TokenRejected,
                                         "The supplied access token is unknown to teh provider.");
        }

        public IToken GetAccessTokenAssociatedWithRequestToken(OAuthContext requestContext)
        {
            EnsureTestConsumer(requestContext);

            if (requestContext.Token != "requestkey")
                throw new OAuthException(requestContext, OAuthProblems.TokenRejected, "Expected Token \"requestkey\"");

            return new TokenBase {ConsumerKey = "key", Realm = null, Token = "accesskey", TokenSecret = "accesssecret"};
        }

        public RequestForAccessStatus GetStatusOfRequestForAccess(OAuthContext requestContext)
        {
            if (requestContext.ConsumerKey == "key" && requestContext.Token == "requestkey")
                return RequestForAccessStatus.Granted;

            return RequestForAccessStatus.Unknown;
        }

        #endregion

        private static void EnsureTestConsumer(IConsumer consumer)
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