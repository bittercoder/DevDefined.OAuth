using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Core;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Consumer
{
    [TestFixture]
    public class OAuthConsumerTests
    {
        [Test]
        public void GetUserAuthorizationUriForTokenWithoutCallback()
        {
            OAuthConsumer consumer = new OAuthConsumer("http://localhost/request", "http://localhost/userauth", "http://localhost/access");
            string actual = consumer.GetUserAuthorizationUrlForToken(new TokenBase { Token = "token" }, null, null);
            Assert.AreEqual("http://localhost/userauth?oauth_token=token", actual);
        }

        [Test]
        public void GetUserAuthorizationUriForTokenWithCallback()
        {
            OAuthConsumer consumer = new OAuthConsumer("http://localhost/request", "http://localhost/userauth", "http://localhost/access");
            string actual = consumer.GetUserAuthorizationUrlForToken(new TokenBase { Token = "token" }, "http://localhost/callback", null);
            Assert.AreEqual("http://localhost/userauth?oauth_token=token&oauth_callback=http%3A%2F%2Flocalhost%2Fcallback", actual);
        }
    }
}
