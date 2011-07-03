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
		byte[] RawContent { get; set; }
		string RawContentType { get; set; }
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
		string BodyHash { get; set; }

    string XAuthMode { get; set; }
    string XAuthUsername { get; set; }
    string XAuthPassword { get; set; }

		bool UseAuthorizationHeader { get; set; }
		bool IncludeOAuthRequestBodyHashInSignature { get; set; }

		Uri GenerateUri();
		string GenerateUrl();
		string GenerateOAuthParametersForHeader();
		Uri GenerateUriWithoutOAuthParameters();
		string GenerateSignatureBase();
		string GenerateBodyHash();
		void GenerateAndSetBodyHash();
		string ToString();
	}
}