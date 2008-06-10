using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DevDefined.OAuth.Framework.Signing
{
    public class RsaSha1SignatureImplementation : IContextSignatureImplementation
    {
        #region IContextSignatureImplementation Members

        public string MethodName
        {
            get { return SignatureMethod.RsaSha1; }
        }

        public void SignContext(OAuthContext authContext, SigningContext signingContext)
        {
            authContext.Signature = GenerateSignature(authContext, signingContext);
        }

        public bool ValidateSignature(OAuthContext authContext, SigningContext signingContext)
        {
            if (signingContext.Algorithm == null) throw Error.AlgorithmPropertyNotSetOnSigningContext();

            SHA1CryptoServiceProvider sha1 = GenerateHash(signingContext);

            var deformatter = new RSAPKCS1SignatureDeformatter(signingContext.Algorithm);
            deformatter.SetHashAlgorithm("MD5");

            byte[] signature = Convert.FromBase64String(authContext.Signature);

            return deformatter.VerifySignature(sha1, signature);
        }

        #endregion

        private string GenerateSignature(OAuthContext authContext, SigningContext signingContext)
        {
            if (signingContext.Algorithm == null) throw Error.AlgorithmPropertyNotSetOnSigningContext();

            SHA1CryptoServiceProvider sha1 = GenerateHash(signingContext);

            var formatter = new RSAPKCS1SignatureFormatter(signingContext.Algorithm);
            formatter.SetHashAlgorithm("MD5");

            byte[] signature = formatter.CreateSignature(sha1);

            return Convert.ToBase64String(signature);
        }

        private SHA1CryptoServiceProvider GenerateHash(SigningContext signingContext)
        {
            var sha1 = new SHA1CryptoServiceProvider();

            byte[] dataBuffer = Encoding.ASCII.GetBytes(signingContext.SignatureBase);

            var cs = new CryptoStream(Stream.Null, sha1, CryptoStreamMode.Write);
            cs.Write(dataBuffer, 0, dataBuffer.Length);
            cs.Close();
            return sha1;
        }
    }
}