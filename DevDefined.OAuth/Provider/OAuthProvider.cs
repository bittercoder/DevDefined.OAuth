using System;
using System.Collections.Generic;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider
{
    public class OAuthProvider : IOAuthProvider
    {
        private readonly List<IContextInspector> _inspectors = new List<IContextInspector>();
        private readonly ITokenStore _tokenStore;

        public OAuthProvider(ITokenStore tokenStore, params IContextInspector[] inspectors)
        {
            if (tokenStore == null) throw new ArgumentNullException("tokenStore");
            _tokenStore = tokenStore;

            if (inspectors != null) _inspectors.AddRange(inspectors);
        }

        #region IOAuthProvider Members

        public virtual IToken GrantRequestToken(OAuthContext context)
        {
            InspectRequest(context);

            return _tokenStore.CreateRequestToken(context);
        }

        public virtual IToken ExchangeRequestTokenForAccessToken(OAuthContext context)
        {
            InspectRequest(context);

            _tokenStore.ConsumeRequestToken(context);

            switch (_tokenStore.GetStatusOfRequestForAccess(context))
            {
                case RequestForAccessStatus.Granted:
                    break;
                case RequestForAccessStatus.Unknown:
                    throw Error.ConsumerHasNotBeenGrantedAccessYet(context);
                default:
                    throw Error.ConsumerHasBeenDeniedAccess(context);
            }

            return _tokenStore.GetAccessTokenAssociatedWithRequestToken(context);
        }

        public virtual void AccessProtectedResourceRequest(OAuthContext context)
        {
            InspectRequest(context);

            _tokenStore.ConsumeAccessToken(context);
        }

        #endregion

        public void AddInspector(IContextInspector inspector)
        {
            _inspectors.Add(inspector);
        }

        protected virtual void InspectRequest(OAuthContext context)
        {
            foreach (IContextInspector inspector in _inspectors)
            {
                inspector.InspectContext(context);
            }
        }
    }
}