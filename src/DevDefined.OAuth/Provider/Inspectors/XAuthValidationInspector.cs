using System;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Provider.Inspectors
{
  public class XAuthValidationInspector : IContextInspector
  {
    private readonly Func<string, bool> _validateModeFunc;
    private readonly Func<string, string, bool> _authenticateFunc;

    public XAuthValidationInspector(Func<string, bool> validateModeFunc, Func<string, string, bool> authenticateFunc)
    {
      _validateModeFunc = validateModeFunc;
      _authenticateFunc = authenticateFunc;
    }

    public void InspectContext(ProviderPhase phase, IOAuthContext context)
    {
      if (phase != ProviderPhase.CreateAccessToken) {
        return;
      }

      var authMode = context.XAuthMode;
      if (string.IsNullOrEmpty(authMode)) {
        throw Error.EmptyXAuthMode(context);
      }

      if (!_validateModeFunc(authMode)) {
        throw Error.InvalidXAuthMode(context);
      }

      var username = context.XAuthUsername;
      if (string.IsNullOrEmpty(username)) {
        throw Error.EmptyXAuthUsername(context);
      }

      var password = context.XAuthPassword;
      if (string.IsNullOrEmpty(password)) {
        throw Error.EmptyXAuthPassword(context);
      }

      if (!_authenticateFunc(username, password)) {
        throw Error.FailedXAuthAuthentication(context);
      }
    }
  }
}