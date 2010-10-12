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

using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
	public class ClientCertEnabledConsumerRequestFactory : IConsumerRequestFactory
	{
		readonly ICertificateFactory _certificateFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientCertEnabledConsumerRequestFactory"/> class.
		/// </summary>
		/// <param name="certificateFactory">The certificate factory.</param>
		public ClientCertEnabledConsumerRequestFactory(ICertificateFactory certificateFactory)
		{
			_certificateFactory = certificateFactory;
		}

		/// <summary>
		/// Creates the consumer request.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="consumerContext">The consumer context.</param>
		/// <param name="token">The token.</param>
		/// <returns></returns>
		public IConsumerRequest CreateConsumerRequest(IOAuthContext context, IOAuthConsumerContext consumerContext, IToken token)
		{
			return new ClientCertEnabledConsumerRequest(_certificateFactory, context, consumerContext, token);
		}
	}
}