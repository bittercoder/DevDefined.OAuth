using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Core;
using NUnit.Framework;
using WatiN.Core;

namespace DevDefined.OAuth.Tests.Consumer
{
    [TestFixture]
    public class GoogleIntegrationTests
    {
        private readonly X509Certificate2 certificate = TestCertificates.OAuthTestCertificate();

        private OAuthConsumer CreateGoogleContactsConsumer()
        {
            return new OAuthConsumer("https://www.google.com/accounts/OAuthGetRequestToken",
                                        "https://www.google.com/accounts/accounts/OAuthAuthorizeToken",
                                             "https://www.google.com/accounts/OAuthGetAccessToken ")
            {
                ConsumerKey = "weitu.googlepages.com",
                SignatureMethod = SignatureMethod.RsaSha1,
                Key = certificate.PrivateKey
            };
        }

        [Test]
        public void RequestTokenForRsaSha1()
        {
            // simple test, just requests a token using RSHA1... 

            var consumer = CreateGoogleContactsConsumer();

            var parameters = new NameValueCollection
                                 {
                                     {"scope", "http://www.google.com/m8/feeds"}
                                 };

            var token = consumer.RequestToken(parameters);
            Assert.AreEqual("weitu.googlepages.com", token.ConsumerKey);
            Assert.IsTrue(token.Token.Length > 0);
            Assert.IsNull(token.TokenSecret);
        }

        [Test]
        public void RequestContacts()
        {     
            // this test does a full end-to-end demo (request token, user authoriazation, exchanging request token
            // for an access token and then using then access token to retrieve some data).

            // note the access token is directly associated with a google user, by them logging in and granting access
            // for your request - thus the client is never exposed to the users credentials (not even their login).
            
            var consumer = CreateGoogleContactsConsumer();

            var parameters = new NameValueCollection
                                 {
                                     {"scope", "http://www.google.com/m8/feeds"}
                                 };

            using (With.NoCertificateValidation())
            {
                var requestToken = consumer.RequestToken(parameters);

                string userAuthorize = consumer.GetUserAuthorizationUrlForToken(requestToken, null, null);

                using (IE ie = new IE(userAuthorize))
                {
                    var overrideLink = ie.Link("overridelink");
                    if (overrideLink.Exists) overrideLink.Click();

                    if (ie.Form("gaia_loginform").Exists)
                    {
                        ie.TextField("Email").Value = "oauthdotnet@gmail.com";
                        ie.TextField("Passwd").Value = "oauth_password";
                        ie.Form("gaia_loginform").Submit();
                    }

                    ie.Button("allow").Click();

                    Assert.IsTrue(ie.Html.Contains("Authorized"));
                }

                var accessToken = consumer.ExchangeRequestTokenForAccessToken(requestToken, parameters);

                // access some protected resource

                OAuthContext context = new OAuthContextFactory().FromUri("GET", new Uri("http://www.google.com/m8/feeds/contacts/default/base"));
                context.QueryParameters.Add(parameters);

                var response = consumer.GetResponse(context, accessToken);
                using (var stream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        string content = streamReader.ReadToEnd();
                        
                        // check to see if one of the sample contacts was returned in the list

                        Assert.IsTrue(content.Contains("alex@devdefined.com"));
                    }
                }
            }
        }
    }
}