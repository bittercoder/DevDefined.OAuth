namespace DevDefined.OAuth.Framework.Signing
{
    public interface IOAuthContextSigner
    {
        void SignContext(OAuthContext authContext, SigningContext signingContext);
        bool ValidateSignature(OAuthContext authContext, SigningContext signingContext);
    }
}