using System;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage.Basic;
using Xunit;

namespace DevDefined.OAuth.Tests.Consumer
{
	public class ConsumerRequestTests
	{
		readonly AccessToken accessToken;
		readonly OAuthConsumerContext consumerContext;
		readonly OAuthContext context;

		public ConsumerRequestTests()
		{
			context = new OAuthContext {RequestMethod = "POST", RawUri = new Uri("http://localhost/svc")};
			consumerContext = new OAuthConsumerContext {ConsumerKey = "key", ConsumerSecret = "secret", SignatureMethod = SignatureMethod.PlainText};
			accessToken = new AccessToken();
		}

		[Fact]
		public void get_request_description_copies_headers_from_context_to_description()
		{
			context.Headers["a-key"] = "a-value";
			var request = new ConsumerRequest(context, consumerContext, accessToken);
			RequestDescription description = request.GetRequestDescription();
			Assert.Equal("a-value", description.Headers["a-key"]);
		}
	}
}