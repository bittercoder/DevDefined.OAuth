using System;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Core;

namespace ExampleConsumerSite
{
    public partial class Callback1 : OAuthPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthConsumer consumer = CreateConsumer();

            string requestTokenString = Request["oauth_token"];

            var requestToken = (IToken) Session[requestTokenString];

            IToken accessToken = consumer.ExchangeRequestTokenForAccessToken(requestToken, null);

            Session[requestTokenString] = null;
            Session[accessToken.Token] = accessToken;

            Response.Redirect("ViewData.aspx?oauth_token=" + accessToken.Token);
        }
    }
}