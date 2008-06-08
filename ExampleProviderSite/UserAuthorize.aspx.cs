using System;
using System.Web;
using System.Web.UI;
using DevDefined.OAuth.Core;
using ExampleProviderSite.Repositories;

namespace ExampleProviderSite
{
    public partial class UserAuthorize : Page
    {
        private TokenRepository Repository
        {
            get { return OAuthServicesLocator.Services.TokenRepository; }
        }

        public string ConsumerKey
        {
            get
            {
                return Repository.GetRequestToken(GetTokenString()).ConsumerKey;
            }
        }

        private string GetTokenString()
        {
            string token = Request[Parameters.OAuth_Token];

            if (string.IsNullOrEmpty(token)) throw new Exception("The token string in the parameters collection was invalid/missing");

            return token;
        }

        protected void Approve_Click(object sender, EventArgs e)
        {
            string token = GetTokenString();

            ApproveRequestForAccess(token);

            RedirectToClient(token, true);
        }

        protected void Deny_Click(object sender, EventArgs e)
        {
            string token = GetTokenString();

            DenyRequestForAccess(token);

            RedirectToClient(token, false);
        }

        private void ApproveRequestForAccess(string tokenString)
        {
            Models.RequestToken requestToken = Repository.GetRequestToken(tokenString);

            var accessToken = new Models.AccessToken
                                  {
                                      ConsumerKey = requestToken.ConsumerKey,
                                      Realm = requestToken.Realm,
                                      Token = Guid.NewGuid().ToString(),
                                      TokenSecret = Guid.NewGuid().ToString(),
                                      UserName = HttpContext.Current.User.Identity.Name,
                                      ExpireyDate = DateTime.Now.AddMinutes(1)
                                  };

            Repository.SaveAccessToken(accessToken);

            requestToken.AccessToken = accessToken;

            Repository.SaveRequestToken(requestToken);
        }

        private void DenyRequestForAccess(string tokenString)
        {
            Models.RequestToken requestToken = Repository.GetRequestToken(tokenString);

            requestToken.AccessDenied = true;

            Repository.SaveRequestToken(requestToken);
        }

        private void RedirectToClient(string token, bool granted)
        {
            string callBackUrl = Request[Parameters.OAuth_Callback];

            if (!string.IsNullOrEmpty(callBackUrl))
            {
                if (!callBackUrl.Contains("?")) callBackUrl += "?";
                else callBackUrl += "&";

                callBackUrl += Parameters.OAuth_Token + "=" + UriUtility.UrlEncode(token);

                Response.Redirect(callBackUrl, true);
            }
            else
            {
                if (granted) Response.Write("Authorized");
                else Response.Write("Denied");

                Response.End();
            }
        }
    }
}