#region License

// The MIT License
//
// Copyright (c) 2006-2008 DevDefined Limited.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OpenSocialProviderSite
{
  public class OpenSocialCertificates
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
  }
}