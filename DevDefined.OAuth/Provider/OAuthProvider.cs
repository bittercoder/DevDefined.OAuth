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
using System.Collections.Generic;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider
{
  public class OAuthProvider : IOAuthProvider
  {
    readonly List<IContextInspector> _inspectors = new List<IContextInspector>();
    readonly ITokenStore _tokenStore;

    public OAuthProvider(ITokenStore tokenStore, params IContextInspector[] inspectors)
    {
      RequiresCallbackUrlInRequest = true;

      if (tokenStore == null) throw new ArgumentNullException("tokenStore");
      _tokenStore = tokenStore;

      if (inspectors != null) _inspectors.AddRange(inspectors);
    }

    public bool RequiresCallbackUrlInRequest { get; set; }

    public virtual IToken GrantRequestToken(IOAuthContext context)
    {
      AssertContextDoesNotIncludeToken(context);

      InspectRequest(ProviderPhase.GrantRequestToken, context);

      return _tokenStore.CreateRequestToken(context);
    }

    void AssertContextDoesNotIncludeToken(IOAuthContext context)
    {
      if (context.Token != null)
      {
        throw Error.RequestForTokenMustNotIncludeTokenInContext(context);
      }
    }

    public virtual IToken ExchangeRequestTokenForAccessToken(IOAuthContext context)
    {
      InspectRequest(ProviderPhase.ExchangeRequestTokenForAccessToken, context);

      _tokenStore.ConsumeRequestToken(context);

      switch (_tokenStore.GetStatusOfRequestForAccess(context))
      {
        case RequestForAccessStatus.Granted:
          break;
        case RequestForAccessStatus.Unknown:
          throw Error.ConsumerHasNotBeenGrantedAccessYet(context);
        default:
          throw Error.ConsumerHasBeenDeniedAccess(context);
      }

      return _tokenStore.GetAccessTokenAssociatedWithRequestToken(context);
    }

    public virtual void AccessProtectedResourceRequest(IOAuthContext context)
    {
      InspectRequest(ProviderPhase.AccessProtectedResourceRequest, context);

      _tokenStore.ConsumeAccessToken(context);
    }

    public void AddInspector(IContextInspector inspector)
    {
      _inspectors.Add(inspector);
    }

    protected virtual void InspectRequest(ProviderPhase phase, IOAuthContext context)
    {
      AssertContextDoesNotIncludeTokenSecret(context);

      AddStoredTokenSecretToContext(context, phase);

      ApplyInspectors(context, phase);
    }

    void ApplyInspectors(IOAuthContext context, ProviderPhase phase)
    {
      foreach (IContextInspector inspector in _inspectors)
      {
        inspector.InspectContext(phase, context);
      }
    }

    void AddStoredTokenSecretToContext(IOAuthContext context, ProviderPhase phase)
    {
      if (phase == ProviderPhase.ExchangeRequestTokenForAccessToken)
      {
        string secret = _tokenStore.GetRequestTokenSecret(context);

        context.TokenSecret = secret;
      }

      else if (phase == ProviderPhase.AccessProtectedResourceRequest)
      {
        string secret = _tokenStore.GetAccessTokenSecret(context);

        context.TokenSecret = secret;
      }
    }

    static void AssertContextDoesNotIncludeTokenSecret(IOAuthContext context)
    {
      if (!string.IsNullOrEmpty(context.TokenSecret))
      {
        throw new OAuthException(context, OAuthProblems.ParameterRejected, "The oauth_token_secret must not be transmitted to the provider.");
      }
    }
  }
}