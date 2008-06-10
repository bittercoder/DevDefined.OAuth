// The MIT License
//
// Copyright (c) 2006-2008 DevDefined Limited.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
﻿using System;
using System.Web;
using System.Web.UI;
using DevDefined.OAuth.Framework;
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