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

namespace DevDefined.OAuth.Framework.Signing
{
  public class PlainTextSignatureImplementation : IContextSignatureImplementation
  {
    #region IContextSignatureImplementation Members

    public string MethodName
    {
      get { return SignatureMethod.PlainText; }
    }

    public void SignContext(IOAuthContext authContext, SigningContext signingContext)
    {
      authContext.Signature = GenerateSignature(authContext, signingContext);
    }

    public bool ValidateSignature(IOAuthContext authContext, SigningContext signingContext)
    {
      return (authContext.Signature == GenerateSignature(authContext, signingContext));
    }

    #endregion

    string GenerateSignature(IOAuthContext authContext, SigningContext signingContext)
    {
      return UriUtility.UrlEncode(string.Format("{0}&{1}", signingContext.ConsumerSecret, authContext.TokenSecret));
    }
  }
}