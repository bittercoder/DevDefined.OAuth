using System.Collections.Generic;
using System.Linq;
using DevDefined.OAuth.Framework.Signing;

namespace DevDefined.OAuth.Framework.Signing
{
    public class OAuthContextSigner : IOAuthContextSigner
    {
        private readonly List<IContextSignatureImplementation> _implementations =
            new List<IContextSignatureImplementation>();

        public OAuthContextSigner(params IContextSignatureImplementation[] implementations)
        {
            if (implementations != null) _implementations.AddRange(implementations);
        }

        public OAuthContextSigner()
            : this(
                new RsaSha1SignatureImplementation(), new HmacSha1SignatureImplementation(),
                new PlainTextSignatureImplementation())
        {
        }

        public void SignContext(OAuthContext authContext, SigningContext signingContext)
        {
            signingContext.SignatureBase = authContext.GenerateSignatureBase();
            FindImplementationForAuthContext(authContext).SignContext(authContext, signingContext);
        }

        private IContextSignatureImplementation FindImplementationForAuthContext(OAuthContext authContext)
        {
            IContextSignatureImplementation impl =
                _implementations.FirstOrDefault(i => i.MethodName == authContext.SignatureMethod);

            if (impl != null) return impl;

            throw Error.UnknownSignatureMethod(authContext.SignatureMethod);
        }

        public bool ValidateSignature(OAuthContext authContext, SigningContext signingContext)
        {
            signingContext.SignatureBase = authContext.GenerateSignatureBase();
            return FindImplementationForAuthContext(authContext).ValidateSignature(authContext, signingContext);
        }
    }
}