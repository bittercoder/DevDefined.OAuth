using System.Security.Cryptography.X509Certificates;

namespace DevDefined.OAuth.Consumer
{
    public class NullCertificateFactory : ICertificateFactory
    {
        public X509Certificate2 CreateCertificate()
        {
            return null;
        }

        public int GetMatchingCertificateCount()
        {
            return 0;
        }
    }
}