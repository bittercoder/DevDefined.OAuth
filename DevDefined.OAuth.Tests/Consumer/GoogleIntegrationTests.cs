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
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using NUnit.Framework;
using WatiN.Core;

namespace DevDefined.OAuth.Tests.Consumer
{
    [TestFixture]
    public class GoogleIntegrationTests
    {
        private readonly X509Certificate2 certificate = TestCertificates.OAuthTestCertificate();

        private IOAuthSession CreateGoogleContactsSession()
        {
            var consumerContext = new OAuthConsumerContext
                                      {
                                          ConsumerKey = "weitu.googlepages.com",
                                          SignatureMethod = SignatureMethod.RsaSha1,
                                          Key = certificate.PrivateKey
                                      };

            return new OAuthSession(consumerContext, "https://www.google.com/accounts/OAuthGetRequestToken",
                                    "https://www.google.com/accounts/accounts/OAuthAuthorizeToken",
                                    "https://www.google.com/accounts/OAuthGetAccessToken ")
                .WithQueryParameters(new {scope = "https://www.google.com/m8/feeds"});
        }

        /*
        private void ExampleForWiki()
        {
string requestUrl = "https://www.google.com/accounts/OAuthGetRequestToken";
string userAuthorizeUrl = "https://www.google.com/accounts/accounts/OAuthAuthorizeToken";
string accessUrl = "https://www.google.com/accounts/OAuthGetAccessToken";
string callBackUrl = "http://www.mysite.com/callback";
var consumerContext = new OAuthConsumerContext
{
    ConsumerKey = "weitu.googlepages.com",
    SignatureMethod = SignatureMethod.RsaSha1,
    Key = certificate.PrivateKey
};

var session = new OAuthSession(consumerContext, requestUrl, userAuthorizeUrl, accessUrl)           
    .WithQueryParameters(new { scope = "http://www.google.com/m8/feeds" });

// get a request token from the provider
IToken requestToken = session.GetRequestToken();

// generate a user authorize url for this token (which you can use in a redirect from the current site)
string authorizationLink = session.GetUserAuthorizationUrlForToken(requestToken, callBackUrl);

// exchange a request token for an access token
IToken accessToken = session.ExchangeRequestTokenForAccessToken(requestToken);

// make a request for a protected resource
string responseText = session.Request().Get().ForUrl("http://www.google.com/m8/feeds/contacts/default/base").ToString();
        }*/

        [Test]
        public void RequestContacts()
        {
            // this test does a full end-to-end integration (request token, user authoriazation, exchanging request token
            // for an access token and then using then access token to retrieve some data).

            // the access token is directly associated with a google user, by them logging in and granting access
            // for your request - thus the client is never exposed to the users credentials (not even their login).

            IOAuthSession consumer = CreateGoogleContactsSession();

            using (With.NoCertificateValidation())
            {
                IToken requestToken = consumer.GetRequestToken();

                string userAuthorize = consumer.GetUserAuthorizationUrlForToken(requestToken, null);

                using (var ie = new IE(userAuthorize))
                {
                    Link overrideLink = ie.Link("overridelink");
                    if (overrideLink.Exists) overrideLink.Click();

                    if (ie.Form("gaia_loginform").Exists)
                    {
                        ie.TextField("Email").Value = "oauthdotnet@gmail.com";
                        ie.TextField("Passwd").Value = "oauth_password";
                        ie.Form("gaia_loginform").Submit();
                    }

                    ie.Button("allow").Click();

                    Assert.IsTrue(ie.Html.Contains("Authorized") || ie.Html.Contains("successfully granted"));
                }

                // this will implicitly set AccessToken on the current session... 

                IToken accessToken = consumer.ExchangeRequestTokenForAccessToken(requestToken);

                try
                {
                    string responseText = consumer.Request().Get().ForUrl("https://www.google.com/m8/feeds/contacts/default/base").ToString();
                    
                    Assert.IsTrue(responseText.Contains("alex@devdefined.com"));
                }
                catch (WebException webEx)
                {
                    HttpWebResponse response = (HttpWebResponse)webEx.Response;
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void RequestTokenForRsaSha1()
        {
            using (With.NoCertificateValidation())
            {
                // simple test, just requests a token using RSHA1... 

                IOAuthSession session = CreateGoogleContactsSession();

                IToken token = session.GetRequestToken();
                Assert.AreEqual("weitu.googlepages.com", token.ConsumerKey);
                Assert.IsTrue(token.Token.Length > 0);
                Assert.IsNull(token.TokenSecret);
            }
        }
    }
}