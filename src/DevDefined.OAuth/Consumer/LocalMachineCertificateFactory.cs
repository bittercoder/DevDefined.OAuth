using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DevDefined.OAuth.Consumer
{
    /// <summary>
    /// Creates X509 certificates from the Local Computer certificate sture based on the certificate subject.
    /// </summary>
    public class LocalMachineCertificateFactory : ICertificateFactory
    {
        private string _certificateSubject;
        private X509FindType _findType;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalMachineCertificateFactory"/> class.
        /// </summary>
        /// <param name="certificateSubject">The certificate subject.</param>
        /// <param name="findType"></param>
        public LocalMachineCertificateFactory(string certificateSubject, X509FindType findType)
        {
            _certificateSubject = certificateSubject;
            _findType = findType;

            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
        }

        /// <summary>
        /// Remotes the certificate validation callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns></returns>
        public bool RemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Creates the certificate.
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 CreateCertificate()
        {
            X509Certificate2Collection certificateCollection = GetCertificateCollection();
            return certificateCollection.Count > 0 ? certificateCollection[0] : null;
        }

        /// <summary>
        /// Counts the matching certificates.
        /// </summary>
        /// <returns></returns>
        public int GetMatchingCertificateCount()
        {
            return GetCertificateCollection().Count;
        }

        /// <summary>
        /// Gets the certificate collection.
        /// </summary>
        /// <returns></returns>
        private X509Certificate2Collection GetCertificateCollection()
        {
            X509Store certStore = new X509Store("My", StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certificateCollection = certStore.Certificates.Find(_findType, _certificateSubject, false);
            certStore.Close();

            return certificateCollection;
        } 
    }
}