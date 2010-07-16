using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DevDefined.OAuth.Consumer
{
    public class LocalFileCertificateFactory : ICertificateFactory
    {
        private readonly string _filename;
        private readonly string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCertificateFactory"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="password">The password.</param>
        public LocalFileCertificateFactory(string filename, string password)
        {
            _filename = filename;
            _password = password;      
            
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("The certificate file could not be located on disk.", filename);
            }

            if (CreateCertificate() == null)
            {
                throw new ApplicationException("The certificate could not be loaded from disk.");
            }
        }

        /// <summary>
        /// Creates the certificate.
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 CreateCertificate()
        {
            if (!File.Exists(_filename))
            {
                return null;
            }

            try
            {
                var certificate = new X509Certificate2(_filename, _password);
                Debug.Assert(certificate.Subject != string.Empty);
                return certificate;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        /// <summary>
        /// Counts the matching certificates.
        /// </summary>
        /// <returns></returns>
        public int GetMatchingCertificateCount()
        {
            return CreateCertificate() != null ? 1 : 0;
        }
    }
}