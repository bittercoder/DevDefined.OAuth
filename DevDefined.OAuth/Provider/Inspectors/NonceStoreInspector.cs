using System;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider.Inspectors
{
    public class NonceStoreInspector : IContextInspector
    {
        private readonly INonceStore _nonceStore;

        public NonceStoreInspector(INonceStore nonceStore)
        {
            if (nonceStore == null) throw new ArgumentNullException("nonceStore");
            _nonceStore = nonceStore;
        }

        #region IContextInspector Members

        public void InspectContext(OAuthContext context)
        {
            if (!_nonceStore.RecordNonceAndCheckIsUnique(context, context.Nonce))
            {
                throw Error.NonceHasAlreadyBeenUsed(context);
            }
        }

        #endregion
    }
}