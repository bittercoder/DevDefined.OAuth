using System.Collections.Generic;
using System.Linq;
using DevDefined.OAuth.Storage;

namespace ExampleProviderSite.Repositories
{
    /// <summary>
    /// A simplistic in-memory repository for access and request token models - the example implementation of
    /// <see cref="ITokenStore" /> relies on this repository - normally you would make use of repositories
    /// wired up to your domain model i.e. NHibernate, Entity Framework etc.
    /// </summary>    
    public class TokenRepository
    {
        private readonly Dictionary<string, Models.RequestToken> _requestTokens = new Dictionary<string, Models.RequestToken>();
        private readonly Dictionary<string, Models.AccessToken> _accessTokens = new Dictionary<string, Models.AccessToken>();

        public Models.RequestToken GetRequestToken(string token)
        {
            return _requestTokens[token];
        }

        public Models.AccessToken GetAccessToken(string token)
        {
            return _accessTokens[token];
        }

        public void SaveRequestToken(Models.RequestToken token)
        {
            _requestTokens[token.Token] = token;
        }

        public void SaveAccessToken(Models.AccessToken token)
        {
            _accessTokens[token.Token] = token;
        }
    }
}