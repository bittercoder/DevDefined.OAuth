namespace DevDefined.OAuth.Core.Signing
{
    public class PlainTextSignatureImplementation : IContextSignatureImplementation
    {
        #region IContextSignatureImplementation Members

        public string MethodName
        {
            get { return SignatureMethod.PlainText; }
        }

        public void SignContext(OAuthContext authContext, SigningContext signingContext)
        {
            authContext.Signature = GenerateSignature(authContext, signingContext);
        }

        public bool ValidateSignature(OAuthContext authContext, SigningContext signingContext)
        {
            return (authContext.Signature == GenerateSignature(authContext, signingContext));
        }

        #endregion

        private string GenerateSignature(OAuthContext authContext, SigningContext signingContext)
        {
            return UriUtility.UrlEncode(string.Format("{0}&{1}", signingContext.ConsumerSecret, authContext.TokenSecret));
        }
    }
}