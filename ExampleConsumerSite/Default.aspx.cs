using System;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

namespace ExampleConsumerSite
{
    public partial class _Default : OAuthPage
    {
        protected void oauthRequest_Click(object sender, EventArgs e)
        {
            OAuthSession session = CreateSession();

            IToken requestToken = session.GetRequestToken();

            if (string.IsNullOrEmpty(requestToken.Token))
            {
                throw new Exception("The request token was null or empty");
            }

            Session[requestToken.Token] = requestToken;

            string callBackUrl = "http://localhost:" + HttpContext.Current.Request.Url.Port + "/Callback.aspx";

            string authorizationUrl = session.GetUserAuthorizationUrlForToken(requestToken, callBackUrl);

            Response.Redirect(authorizationUrl, true);
        }
    }
}