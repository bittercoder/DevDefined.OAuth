using System;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider.Inspectors
{
  /// <summary>
  /// This inspector implements additional behavior required by the 1.0a version of OAuth.
  /// </summary>
  public class OAuth10AInspector : IContextInspector
  {
    readonly ITokenStore _tokenStore;

    public OAuth10AInspector(ITokenStore tokenStore)
    {
      if (tokenStore == null) throw new ArgumentNullException("tokenStore");
      _tokenStore = tokenStore;
    }

    public void InspectContext(ProviderPhase phase, IOAuthContext context)
    {
      if (phase == ProviderPhase.GrantRequestToken)
      {
        ValidateCallbackUrlIsPartOfRequest(context);
      }
      else if (phase == ProviderPhase.ExchangeRequestTokenForAccessToken)
      {
        ValidateVerifierMatchesStoredVerifier(context);
      }
    }

    void ValidateVerifierMatchesStoredVerifier(IOAuthContext context)
    {
      string actual = context.Verifier;

      if (string.IsNullOrEmpty(actual))
      {
        throw Error.MissingRequiredOAuthParameter(context, Parameters.OAuth_Verifier);
      }

      string expected = _tokenStore.GetVerificationCodeForRequestToken(context);

      if (expected != actual.Trim())
      {
        throw Error.RejectedRequiredOAuthParameter(context, Parameters.OAuth_Verifier);
      }
    }

    static void ValidateCallbackUrlIsPartOfRequest(IOAuthContext context)
    {
      if (string.IsNullOrEmpty(context.CallbackUrl))
      {
        throw Error.MissingRequiredOAuthParameter(context, Parameters.OAuth_Callback);
      }
    }
  }
}