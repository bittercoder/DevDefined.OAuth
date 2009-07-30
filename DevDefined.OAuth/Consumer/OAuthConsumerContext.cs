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
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Framework.Signing;

namespace DevDefined.OAuth.Consumer
{
  public class OAuthConsumerContext : IOAuthConsumerContext
  {
    INonceGenerator _nonceGenerator = new GuidNonceGenerator();
    IOAuthContextSigner _signer = new OAuthContextSigner();

    public OAuthConsumerContext()
    {
      SignatureMethod = Framework.SignatureMethod.PlainText;
    }

    public IOAuthContextSigner Signer
    {
      get { return _signer; }
      set { _signer = value; }
    }

    public INonceGenerator NonceGenerator
    {
      get { return _nonceGenerator; }
      set { _nonceGenerator = value; }
    }

    #region IOAuthConsumerContext Members

    public string Realm { get; set; }
    public string ConsumerKey { get; set; }
    public string ConsumerSecret { get; set; }
    public string SignatureMethod { get; set; }
    public AsymmetricAlgorithm Key { get; set; }
    public bool UseHeaderForOAuthParameters { get; set; }

    public void SignContext(IOAuthContext context)
    {
      EnsureStateIsValid();


      context.UseAuthorizationHeader = UseHeaderForOAuthParameters;
      context.Nonce = _nonceGenerator.GenerateNonce(context);
      context.ConsumerKey = ConsumerKey;
      context.Realm = Realm;
      context.SignatureMethod = SignatureMethod;
      context.Timestamp = DateTime.Now.Epoch().ToString();
      context.Version = "1.0";

      context.Nonce = NonceGenerator.GenerateNonce(context);

      string signatureBase = context.GenerateSignatureBase();

      _signer.SignContext(context,
                          new SigningContext
                            {Algorithm = Key, SignatureBase = signatureBase, ConsumerSecret = ConsumerSecret});
    }

    public void SignContextWithToken(IOAuthContext context, IToken token)
    {
      context.Token = token.Token;
      context.TokenSecret = token.TokenSecret;

      SignContext(context);
    }

    #endregion

    void EnsureStateIsValid()
    {
      if (string.IsNullOrEmpty(ConsumerKey)) throw Error.EmptyConsumerKey();
      if (string.IsNullOrEmpty(SignatureMethod)) throw Error.UnknownSignatureMethod(SignatureMethod);
      if ((SignatureMethod == Framework.SignatureMethod.RsaSha1)
          && (Key == null)) throw Error.ForRsaSha1SignatureMethodYouMustSupplyAssymetricKeyParameter();
    }
  }
}