using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Tests;
using ExampleConsumerSite.Properties;

namespace ExampleConsumerSite
{
    public class OAuthPage : System.Web.UI.Page
    {
        protected OAuthConsumer CreateConsumer()
        {
            return new OAuthConsumer(Settings.Default.RequestTokenUrl,
                                     Settings.Default.UserAuthorizationUrl,
                                     Settings.Default.AccessTokenUrl)
            {
                ConsumerKey = "key",
                ConsumerSecret = "secret",
                Key = TestCertificates.OAuthTestCertificate().PrivateKey
            };
        }
    }
}
