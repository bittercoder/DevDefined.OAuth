using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Tests;
using Xunit;

namespace DevDefined.OAuth.ExamplesForWiki
{
	public class ConsumerWikiExamples
	{
		[Fact(Skip="This is just an example of flow - when executed it will fail when attempting to exchange the request token for the access token")]
		public void GoogleConsumer()
		{
			X509Certificate2 certificate = TestCertificates.OAuthTestCertificate();

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
		}
	}
}
