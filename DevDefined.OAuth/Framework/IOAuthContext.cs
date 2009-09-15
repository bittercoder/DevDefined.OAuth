using System;
using System.Collections.Specialized;

namespace DevDefined.OAuth.Framework
{
  public interface IOAuthContext : IToken
  {
    NameValueCollection Headers { get; set; }
    NameValueCollection QueryParameters { get; set; }
    NameValueCollection Cookies { get; set; }
    NameValueCollection FormEncodedParameters { get; set; }
    NameValueCollection AuthorizationHeaderParameters { get; set; }

    Uri RawUri { get; set; }

    string NormalizedRequestUrl { get; }
    string RequestMethod { get; set; }
    string Nonce { get; set; }
    string Signature { get; set; }
    string SignatureMethod { get; set; }
    string Timestamp { get; set; }
    string Version { get; set; }
    string CallbackUrl { get; set; }
    string Verifier { get; set; }

    bool UseAuthorizationHeader { get; set; }

    Uri GenerateUri();
    string GenerateUrl();
    string GenerateOAuthParametersForHeader();
    Uri GenerateUriWithoutOAuthParameters();
    string GenerateSignatureBase();
    string ToString();
  }
}