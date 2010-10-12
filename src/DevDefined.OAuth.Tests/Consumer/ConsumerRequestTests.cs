#region License

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

#endregion

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