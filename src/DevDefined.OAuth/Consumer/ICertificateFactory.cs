using System.Security.Cryptography.X509Certificates;

namespace DevDefined.OAuth.Consumer
{
    public interface ICertificateFactory
    {
        X509Certificate2 CreateCertificate();

        int GetMatchingCertificateCount();
    }
}