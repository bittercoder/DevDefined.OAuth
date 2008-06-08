using System;
using System.Web.UI;
using DevDefined.OAuth.Core;
using DevDefined.OAuth.Provider;

namespace ExampleProviderSite
{
    public partial class RequestToken : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthContext context = new OAuthContextFactory().FromHttpRequest(Request);

            IOAuthProvider provider = OAuthServicesLocator.Services.Provider;

            IToken token = provider.GrantRequestToken(context);

            Response.Write(token);
            Response.End();
        }
    }
}