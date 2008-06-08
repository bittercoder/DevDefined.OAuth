using System;
using System.Web;
using System.Web.UI;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Core;
using DevDefined.OAuth.Tests;
using ExampleConsumerSite.Properties;

namespace ExampleConsumerSite
{
    public partial class _Default : OAuthPage
    {
        protected void oauthRequest_Click(object sender, EventArgs e)
        {
            OAuthConsumer consumer = CreateConsumer();

            IToken requestToken = consumer.RequestToken(null);

            if (string.IsNullOrEmpty(requestToken.Token))
            {
                throw new Exception("The request token was null or empty");
            }

            // throw the request token in the session, so we can grab it upon call-back

            Session[requestToken.Token] = requestToken;

            string callBackUrl = "http://localhost:" + HttpContext.Current.Request.Url.Port + "/Callback.aspx";

            string authorizationUrl = consumer.GetUserAuthorizationUrlForToken(requestToken, callBackUrl, null);

            Response.Redirect(authorizationUrl, true);            
        }
    }
}