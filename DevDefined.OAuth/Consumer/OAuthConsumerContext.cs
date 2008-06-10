using System;
using System.Security.Cryptography;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Consumer
{
    public class OAuthConsumerContext : IOAuthConsumerContext
    {
        private INonceGenerator _nonceGenerator = new GuidNonceGenerator();
        
        public string Realm { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string SignatureMethod { get; set; }
        public AsymmetricAlgorithm Key { get; set; }
        public bool UseHeaderForOAuthParameters { get; set; }
        
        public OAuthConsumerContext()
        {
            SignatureMethod = Core.SignatureMethod.PlainText;
        }

        public INonceGenerator NonceGenerator
        {
            get { return _nonceGenerator; }
            set { _nonceGenerator = value; }
        }

        public void SignContext(OAuthContext context)
        {
            EnsureStateIsValid();

            var signer = new OAuthContextSigner();

            context.UseAuthorizationHeader = UseHeaderForOAuthParameters;
            context.Nonce = _nonceGenerator.GenerateNonce(context);            
            context.ConsumerKey = ConsumerKey;
            context.Realm = Realm;
            context.SignatureMethod = SignatureMethod;
            context.Timestamp = DateTime.Now.Epoch().ToString();
            context.Version = "1.0";

            context.Nonce = NonceGenerator.GenerateNonce(context);

            string signatureBase = context.GenerateSignatureBase();

            signer.SignContext(context,
                               new SigningContext { Algorithm = Key, SignatureBase = signatureBase, ConsumerSecret = ConsumerSecret });
        }

        public void SignContextWithToken(OAuthContext context, IToken token)
        {
            context.Token = token.Token;
            context.TokenSecret = token.TokenSecret;

            SignContext(context);
        }

        private void EnsureStateIsValid()
        {
            if (string.IsNullOrEmpty(ConsumerKey)) throw Error.EmptyConsumerKey();
            if (string.IsNullOrEmpty(SignatureMethod)) throw Error.UnknownSignatureMethod(SignatureMethod);
            if ((SignatureMethod == Core.SignatureMethod.RsaSha1)
                && (Key == null)) throw Error.ForRsaSha1SignatureMethodYouMustSupplyAssymetricKeyParameter();
        }
    }
}