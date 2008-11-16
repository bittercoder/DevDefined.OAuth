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
using System.Web.UI;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Framework.Signing;

namespace OpenSocialProviderSite
{
  public partial class SocialService : Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      ValidateWithDevDefinedOAuth();

      // now handle the request...
    }

    void ValidateWithDevDefinedOAuth()
    {
      try
      {
        OAuthContext context = new OAuthContextBuilder().FromHttpRequest(Request);
        var signer = new OAuthContextSigner();
        var signingContext = new SigningContext {Algorithm = OpenSocialCertificates.FriendsterCertificate.PublicKey.Key};

        if (!signer.ValidateSignature(context, signingContext))
        {
          throw new OAuthException(context, OAuthProblems.SignatureInvalid, "check certificate is still valid");
        }
      }
      catch (OAuthException authEx)
      {
        Response.Clear();
        Response.Write(authEx.Report.ToString());
        Response.End();
      }
    }
  }
}