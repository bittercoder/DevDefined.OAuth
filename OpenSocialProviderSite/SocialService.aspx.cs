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

        private void ValidateWithDevDefinedOAuth()
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