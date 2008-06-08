using System;
using System.Security.Cryptography;
using System.Text;

namespace DevDefined.OAuth.Core.Signing
{
    public class HmacSha1SignatureImplementation : IContextSignatureImplementation
    {
        #region IContextSignatureImplementation Members

        public string MethodName
        {
            get { return SignatureMethod.HmacSha1; }
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
            string consumerSecret = (signingContext.ConsumerSecret != null)
                                        ? UriUtility.UrlEncode(signingContext.ConsumerSecret)
                                        : "";
            string tokenSecret = (authContext.TokenSecret != null)
                                     ? UriUtility.UrlEncode(authContext.TokenSecret)
                                     : null;
            string hashSource = string.Format("{0}&{1}", consumerSecret, tokenSecret);

            var hashAlgorithm = new HMACSHA1();

            hashAlgorithm.Key = Encoding.ASCII.GetBytes(hashSource);

            return ComputeHash(hashAlgorithm, signingContext.SignatureBase);
        }

        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }
    }
}