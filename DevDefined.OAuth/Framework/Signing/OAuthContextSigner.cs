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

using System.Collections.Generic;
using System.Linq;

namespace DevDefined.OAuth.Framework.Signing
{
  public class OAuthContextSigner : IOAuthContextSigner
  {
    readonly List<IContextSignatureImplementation> _implementations =
      new List<IContextSignatureImplementation>();

    public OAuthContextSigner(params IContextSignatureImplementation[] implementations)
    {
      if (implementations != null) _implementations.AddRange(implementations);
    }

    public OAuthContextSigner()
      : this(
        new RsaSha1SignatureImplementation(), new HmacSha1SignatureImplementation(),
        new PlainTextSignatureImplementation())
    {
    }

    public void SignContext(IOAuthContext authContext, SigningContext signingContext)
    {
      signingContext.SignatureBase = authContext.GenerateSignatureBase();
      FindImplementationForAuthContext(authContext).SignContext(authContext, signingContext);
    }

    public bool ValidateSignature(IOAuthContext authContext, SigningContext signingContext)
    {
      signingContext.SignatureBase = authContext.GenerateSignatureBase();
      return FindImplementationForAuthContext(authContext).ValidateSignature(authContext, signingContext);
    }

    IContextSignatureImplementation FindImplementationForAuthContext(IOAuthContext authContext)
    {
      IContextSignatureImplementation impl =
        _implementations.FirstOrDefault(i => i.MethodName == authContext.SignatureMethod);

      if (impl != null) return impl;

      throw Error.UnknownSignatureMethod(authContext.SignatureMethod);
    }
  }
}