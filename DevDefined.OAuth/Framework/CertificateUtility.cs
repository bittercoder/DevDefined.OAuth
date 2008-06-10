using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DevDefined.OAuth.KeyInterop;

namespace DevDefined.OAuth.Framework
{
    public static class CertificateUtility
    {
        /// <summary>
        /// Loads a certificate given both it's private and public keys - generally used to 
        /// load keys provided on the OAuth wiki's for verification of implementation correctness.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertificateFromStrings(string privateKey, string certificate)
        {
            var parser = new AsnKeyParser(Convert.FromBase64String(privateKey));
            RSAParameters parameters = parser.ParseRSAPrivateKey();
            var x509 = new X509Certificate2(Encoding.ASCII.GetBytes(certificate));
            var provider = new RSACryptoServiceProvider();
            provider.ImportParameters(parameters);
            x509.PrivateKey = provider;

            return x509;
        }
    }
}