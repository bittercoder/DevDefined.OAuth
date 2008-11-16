using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;

namespace DevDefined.OAuth.Wcf
{
  public class PrincipalAuthorizationPolicy : IAuthorizationPolicy
  {
    readonly string _id = Guid.NewGuid().ToString();
    readonly IPrincipal _principal;

    public PrincipalAuthorizationPolicy(IPrincipal principal)
    {
      if (principal == null) throw new ArgumentNullException("principal");
      _principal = principal;
    }

    #region IAuthorizationPolicy Members

    public ClaimSet Issuer
    {
      get { return ClaimSet.System; }
    }

    public string Id
    {
      get { return _id; }
    }

    public bool Evaluate(EvaluationContext evaluationContext, ref object state)
    {
      evaluationContext.AddClaimSet(this, new DefaultClaimSet(Claim.CreateNameClaim(_principal.Identity.Name)));
      evaluationContext.Properties["Identities"] = new List<IIdentity>(new[] {_principal.Identity});
      evaluationContext.Properties["Principal"] = _principal;
      return true;
    }

    #endregion
  }
}