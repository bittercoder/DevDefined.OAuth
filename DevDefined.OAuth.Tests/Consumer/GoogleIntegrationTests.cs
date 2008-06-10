using System.Security.Cryptography.X509Certificates;
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
                .WithQueryParameters(new {scope = "http://www.google.com/m8/feeds"});
        }

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

                    Assert.IsTrue(ie.Html.Contains("Authorized"));
                }

                // this will implicitly set AccessToken on the current session... 

                IToken accessToken = consumer.ExchangeRequestTokenForAccessToken(requestToken);

                string responseText =
                    consumer.Request().Get().ForUrl("http://www.google.com/m8/feeds/contacts/default/base").ToString();

                Assert.IsTrue(responseText.Contains("alex@devdefined.com"));
            }
        }

        [Test]
        public void RequestTokenForRsaSha1()
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