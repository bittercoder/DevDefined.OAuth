using System;
using System.Security.Cryptography;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Framework.Signing;

namespace DevDefined.OAuth.Consumer
{
    public class OAuthConsumerContext : IOAuthConsumerContext
    {
        private INonceGenerator _nonceGenerator = new GuidNonceGenerator();
        private IOAuthContextSigner _signer = new OAuthContextSigner();

        public OAuthConsumerContext()
        {
            SignatureMethod = Framework.SignatureMethod.PlainText;
        }

        public IOAuthContextSigner Signer
        {
            get { return _signer; }
            set { _signer = value; }
        }

        public INonceGenerator NonceGenerator
        {
            get { return _nonceGenerator; }
            set { _nonceGenerator = value; }
        }

        #region IOAuthConsumerContext Members

        public string Realm { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string SignatureMethod { get; set; }
        public AsymmetricAlgorithm Key { get; set; }
        public bool UseHeaderForOAuthParameters { get; set; }

        public void SignContext(OAuthContext context)
        {
            EnsureStateIsValid();


            context.UseAuthorizationHeader = UseHeaderForOAuthParameters;
            context.Nonce = _nonceGenerator.GenerateNonce(context);
            context.ConsumerKey = ConsumerKey;
            context.Realm = Realm;
            context.SignatureMethod = SignatureMethod;
            context.Timestamp = DateTime.Now.Epoch().ToString();
            context.Version = "1.0";

            context.Nonce = NonceGenerator.GenerateNonce(context);

            string signatureBase = context.GenerateSignatureBase();

            _signer.SignContext(context,
                                new SigningContext
                                    {Algorithm = Key, SignatureBase = signatureBase, ConsumerSecret = ConsumerSecret});
        }

        public void SignContextWithToken(OAuthContext context, IToken token)
        {
            context.Token = token.Token;
            context.TokenSecret = token.TokenSecret;

            SignContext(context);
        }

        #endregion

        private void EnsureStateIsValid()
        {
            if (string.IsNullOrEmpty(ConsumerKey)) throw Error.EmptyConsumerKey();
            if (string.IsNullOrEmpty(SignatureMethod)) throw Error.UnknownSignatureMethod(SignatureMethod);
            if ((SignatureMethod == Framework.SignatureMethod.RsaSha1)
                && (Key == null)) throw Error.ForRsaSha1SignatureMethodYouMustSupplyAssymetricKeyParameter();
        }
    }
}