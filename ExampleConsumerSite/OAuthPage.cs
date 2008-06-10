using System.Web.UI;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Tests;
using ExampleConsumerSite.Properties;

namespace ExampleConsumerSite
{
    public class OAuthPage : Page
    {
        protected OAuthSession CreateSession()
        {
            return new OAuthSession(new OAuthConsumerContext
                                        {
                                            ConsumerKey = "key",
                                            ConsumerSecret = "secret",
                                            Key = TestCertificates.OAuthTestCertificate().PrivateKey
                                        },
                                    Settings.Default.RequestTokenUrl,
                                    Settings.Default.UserAuthorizationUrl,
                                    Settings.Default.AccessTokenUrl);
        }
    }
}