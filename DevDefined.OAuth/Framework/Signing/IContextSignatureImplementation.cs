namespace DevDefined.OAuth.Framework.Signing
{
    public interface IContextSignatureImplementation
    {
        string MethodName { get; }
        void SignContext(OAuthContext authContext, SigningContext signingContext);
        bool ValidateSignature(OAuthContext authContext, SigningContext signingContext);
    }
}