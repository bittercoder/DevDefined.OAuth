using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Framework.Signing;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Provider
{
  [TestFixture]
  public class FriendsterTests
  {
    const string _friendsterCertificate =
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

    [Test]
    public void EnsureSignaturesMatchWithAndWithoutTrailingAmpersand()
    {
      const string expectedSignature =
        "GET&http%3A%2F%2Fdemo.devdefined.com%2FOpenSocial%2FHelloWorld.aspx&container%3Ddefault%26oauth_consumer_key%3Dfriendster.com%26oauth_nonce%3Dc39f4e3e6c309988763eb8af85fcb74b%26oauth_signature_method%3DRSA-SHA1%26oauth_timestamp%3D1221992254%26oauth_token%3D%26opensocial_app_id%3D52ae97f7aa8a7e7565dd40a4e00eb0f5%26opensocial_owner_id%3D82474146%26opensocial_viewer_id%3D82474146%26synd%3Dfriendster%26xoauth_signature_publickey%3Dhttp%253A%252F%252Fwww.fmodules.com%252Fpublic080813.crt";

      var uriWithAmpersand =
        new Uri(
          "http://demo.devdefined.com/OpenSocial/HelloWorld.aspx?oauth_nonce=c39f4e3e6c309988763eb8af85fcb74b&oauth_timestamp=1221992254&oauth_consumer_key=friendster.com&synd=friendster&container=default&opensocial_owner_id=82474146&opensocial_viewer_id=82474146&opensocial_app_id=52ae97f7aa8a7e7565dd40a4e00eb0f5&oauth_token=&xoauth_signature_publickey=http%3A%2F%2Fwww.fmodules.com%2Fpublic080813.crt&oauth_signature_method=RSA-SHA1&oauth_signature=PLOkRKwLLeJRZz18PsAVQgL5y9Rdf0AW5eicdT0xwauRe3bE2NTDFHoMsUtO6UMHEY0v9GRcKbvkgEWEGGtiGA%3D%3D&");
      var uriWithoutAmpersand =
        new Uri(
          "http://demo.devdefined.com/OpenSocial/HelloWorld.aspx?oauth_nonce=c39f4e3e6c309988763eb8af85fcb74b&oauth_timestamp=1221992254&oauth_consumer_key=friendster.com&synd=friendster&container=default&opensocial_owner_id=82474146&opensocial_viewer_id=82474146&opensocial_app_id=52ae97f7aa8a7e7565dd40a4e00eb0f5&oauth_token=&xoauth_signature_publickey=http%3A%2F%2Fwww.fmodules.com%2Fpublic080813.crt&oauth_signature_method=RSA-SHA1&oauth_signature=PLOkRKwLLeJRZz18PsAVQgL5y9Rdf0AW5eicdT0xwauRe3bE2NTDFHoMsUtO6UMHEY0v9GRcKbvkgEWEGGtiGA%3D%3D");

      Assert.AreEqual(expectedSignature, new OAuthContextBuilder().FromUri("GET", uriWithAmpersand).GenerateSignatureBase());
      Assert.AreEqual(expectedSignature, new OAuthContextBuilder().FromUri("GET", uriWithoutAmpersand).GenerateSignatureBase());
    }

    [Test]
    public void ValidateWithTrailingAmpersand()
    {
      // As reported in issue here:  http://code.google.com/p/devdefined-tools/issues/detail?id=1
      // validating OAuth requests from Friendster was failing - turns out to be OpenSocial platforms 
      // incorrectly placing a "&" on the end of their query parameters, which was tripping up 
      // query parameters collection - there is now a fix in the context builder to remove the problematic
      // character when parsing requests/Uri's.

      var uri =
        new Uri(
          "http://demo.devdefined.com/OpenSocial/HelloWorld.aspx?oauth_nonce=c39f4e3e6c309988763eb8af85fcb74b&oauth_timestamp=1221992254&oauth_consumer_key=friendster.com&synd=friendster&container=default&opensocial_owner_id=82474146&opensocial_viewer_id=82474146&opensocial_app_id=52ae97f7aa8a7e7565dd40a4e00eb0f5&oauth_token=&xoauth_signature_publickey=http%3A%2F%2Fwww.fmodules.com%2Fpublic080813.crt&oauth_signature_method=RSA-SHA1&oauth_signature=PLOkRKwLLeJRZz18PsAVQgL5y9Rdf0AW5eicdT0xwauRe3bE2NTDFHoMsUtO6UMHEY0v9GRcKbvkgEWEGGtiGA%3D%3D&");

      IOAuthContext context = new OAuthContextBuilder().FromUri("GET", uri);
      var signer = new OAuthContextSigner();
      var signingContext = new SigningContext {Algorithm = FriendsterCertificate.PublicKey.Key};

      Assert.IsTrue(signer.ValidateSignature(context, signingContext));
    }
  }
}