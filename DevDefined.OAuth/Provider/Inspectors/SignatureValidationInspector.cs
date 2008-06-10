using System.Security.Cryptography.X509Certificates;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Framework.Signing;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider.Inspectors
{
    public class SignatureValidationInspector : IContextInspector
    {
        private readonly IConsumerStore _consumerStore;
        private readonly IOAuthContextSigner _signer;

        public SignatureValidationInspector(IConsumerStore consumerStore)
            : this(consumerStore, new OAuthContextSigner())
        {
        }

        public SignatureValidationInspector(IConsumerStore consumerStore, IOAuthContextSigner signer)
        {
            _consumerStore = consumerStore;
            _signer = signer;
        }

        #region IContextInspector Members

        public virtual void InspectContext(OAuthContext context)
        {
            SigningContext signingContext = CreateSignatureContextForConsumer(context);

            if (!_signer.ValidateSignature(context, signingContext))
            {
                throw Error.FailedToValidateSignature(context);
            }
        }

        #endregion

        protected virtual bool SignatureMethodRequiresCertificate(string signatureMethod)
        {
            return ((signatureMethod != SignatureMethod.HmacSha1) && (signatureMethod != SignatureMethod.PlainText));
        }

        protected virtual SigningContext CreateSignatureContextForConsumer(OAuthContext context)
        {
            var signingContext = new SigningContext {ConsumerSecret = _consumerStore.GetConsumerSecret(context)};

            if (SignatureMethodRequiresCertificate(context.SignatureMethod))
            {
                X509Certificate2 cert = _consumerStore.GetConsumerCertificate(context);
                signingContext.Algorithm = cert.PublicKey.Key;
            }

            return signingContext;
        }
    }
}