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
﻿using System.Security.Cryptography.X509Certificates;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Framework.Signing;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider.Inspectors
{
    public class SignatureValidationInspector : IContextInspector
    {
        private readonly IConsumerStore _consumerStore;
        private readonly IOAuthContextSigner _signer;

        public SignatureValidationInspector(IConsumerStore consumerStore)
            : this(consumerStore, new OAuthContextSigner())
        {
        }

        public SignatureValidationInspector(IConsumerStore consumerStore, IOAuthContextSigner signer)
        {
            _consumerStore = consumerStore;
            _signer = signer;
        }

        #region IContextInspector Members

        public virtual void InspectContext(OAuthContext context)
        {
            SigningContext signingContext = CreateSignatureContextForConsumer(context);

            if (!_signer.ValidateSignature(context, signingContext))
            {
                throw Error.FailedToValidateSignature(context);
            }
        }

        #endregion

        protected virtual bool SignatureMethodRequiresCertificate(string signatureMethod)
        {
            return ((signatureMethod != SignatureMethod.HmacSha1) && (signatureMethod != SignatureMethod.PlainText));
        }

        protected virtual SigningContext CreateSignatureContextForConsumer(OAuthContext context)
        {
            var signingContext = new SigningContext {ConsumerSecret = _consumerStore.GetConsumerSecret(context)};

            if (SignatureMethodRequiresCertificate(context.SignatureMethod))
            {
                X509Certificate2 cert = _consumerStore.GetConsumerCertificate(context);
                signingContext.Algorithm = cert.PublicKey.Key;
            }

            return signingContext;
        }
    }
}