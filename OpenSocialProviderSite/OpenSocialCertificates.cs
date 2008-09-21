using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OpenSocialProviderSite
{
  public class OpenSocialCertificates
  {
    private const string _friendsterCertificate =
@"-----BEGIN CERTIFICATE-----
MIIB2TCCAYOgAwIBAgIBADANBgkqhkiG9w0BAQUFADAvMQswCQYDVQQGEwJVUzEL
MAkGA1UECBMCQ0ExEzARBgNVBAoTCkZyaWVuZHN0ZXIwHhcNMDgwODEzMTgwMzQ5
WhcNMTQwMjAzMTgwMzQ5WjAvMQswCQYDVQQGEwJVUzELMAkGA1UECBMCQ0ExEzAR
BgNVBAoTCkZyaWVuZHN0ZXIwXDANBgkqhkiG9w0BAQEFAANLADBIAkEAyVjnX2Hr
SLTyAuh2f2/OSRWkLFo3+q+l0Czb48v24Me6CsoexkPgwLOjXmPn/Pt8WtwlisQP
tZ9RX30iymg0owIDAQABo4GJMIGGMB0GA1UdDgQWBBQlDiW+HfExpSnvWqM5a1JD
C+IMyTBXBgNVHSMEUDBOgBQlDiW+HfExpSnvWqM5a1JDC+IMyaEzpDEwLzELMAkG
A1UEBhMCVVMxCzAJBgNVBAgTAkNBMRMwEQYDVQQKEwpGcmllbmRzdGVyggEAMAwG
A1UdEwQFMAMBAf8wDQYJKoZIhvcNAQEFBQADQQCXFtEZswNcPcOTT78oeTuslgmu
0shaZB0PAjA3I89OJZBI7SknIwDxj56kNZpEo6Rhf3uilpj44gkJFecSYnG2
-----END CERTIFICATE-----";

    public static X509Certificate2 FriendsterCertificate
    {
      get { return new X509Certificate2(Encoding.ASCII.GetBytes(_friendsterCertificate)); }
    }
  }
}