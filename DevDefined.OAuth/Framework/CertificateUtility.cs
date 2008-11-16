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