using System;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth
{
    /// <summary>
    /// Generates unique nonces (via Guids) to let the server detect duplicated requests.
    /// </summary>
    public class GuidNonceGenerator : INonceGenerator
    {
        protected Random random = new Random();

        #region INonceGenerator Members

        public string GenerateNonce(OAuthContext context)
        {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}