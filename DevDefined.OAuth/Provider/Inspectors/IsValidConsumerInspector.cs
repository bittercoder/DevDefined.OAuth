using System;
using DevDefined.OAuth.Core;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider
{
    public class IsValidConsumerInspector : IContextInspector
    {
        private readonly IConsumerStore _consumerStore;

        public IsValidConsumerInspector(IConsumerStore consumerStore)
        {
            if (consumerStore == null) throw new ArgumentNullException("consumerStore");
            _consumerStore = consumerStore;
        }

        #region IContextInspector Members

        public void InspectContext(OAuthContext context)
        {
            if (!_consumerStore.IsConsumer(context))
            {
                throw Error.UnknownConsumerKey(context);
            }
        }

        #endregion
    }
}