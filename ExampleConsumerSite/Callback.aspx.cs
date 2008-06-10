using System;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

namespace ExampleConsumerSite
{
    public partial class Callback : OAuthPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var session = CreateSession();

            string requestTokenString = Request["oauth_token"];

            var requestToken = (IToken) Session[requestTokenString];

            IToken accessToken = session.ExchangeRequestTokenForAccessToken(requestToken);

            Session[requestTokenString] = null;
            Session[accessToken.Token] = accessToken;

            Response.Redirect("ViewData.aspx?oauth_token=" + accessToken.Token);
        }
    }
}