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
using DevDefined.OAuth.Provider.Inspectors;
using Xunit;

namespace DevDefined.OAuth.Tests.Provider.Inspectors
{
	public class BodyHashValidationInspectorTests
	{
		const string EmptyBodyHash = "2jmj7l5rSw0yVb/vlWAYkK/YBwk=";
		readonly BodyHashValidationInspector inspector = new BodyHashValidationInspector();

		[Fact]
		public void inspect_body_for_plainttext_signature_does_nothing()
		{
			var context = new OAuthContext
			              	{
			              		UseAuthorizationHeader = true,
			              		BodyHash = "wrong",
			              		SignatureMethod = SignatureMethod.PlainText
			              	};

			Assert.DoesNotThrow(() => inspector.InspectContext(ProviderPhase.AccessProtectedResourceRequest, context));
		}

		[Fact]
		public void inspect_body_for_hmac_sha1_signature_throws_when_hash_does_not_match()
		{
			var context = new OAuthContext
			              	{
			              		UseAuthorizationHeader = true,
			              		BodyHash = "wrong",
			              		SignatureMethod = SignatureMethod.HmacSha1
			              	};

			var ex = Assert.Throws<OAuthException>(() => inspector.InspectContext(ProviderPhase.AccessProtectedResourceRequest, context));

			Assert.Equal("Failed to validate body hash", ex.Message);
		}

		[Fact]
		public void inspect_body_for_hmac_sha1_signature_does_not_throw_when_hash_matches()
		{
			var context = new OAuthContext
			              	{
			              		UseAuthorizationHeader = true,
			              		BodyHash = EmptyBodyHash,
			              		SignatureMethod = SignatureMethod.HmacSha1
			              	};

			Assert.DoesNotThrow(() => inspector.InspectContext(ProviderPhase.AccessProtectedResourceRequest, context));
		}

		[Fact]
		public void inspect_body_for_hmac_sha1_signature_does_not_throw_when_body_hash_is_null()
		{
			var context = new OAuthContext
			              	{
			              		UseAuthorizationHeader = true,
			              		BodyHash = null,
			              		SignatureMethod = SignatureMethod.HmacSha1
			              	};

			Assert.DoesNotThrow(() => inspector.InspectContext(ProviderPhase.AccessProtectedResourceRequest, context));
		}

		[Fact]
		public void inspect_when_context_has_form_parameters_throws()
		{
			var context = new OAuthContext
			              	{
			              		UseAuthorizationHeader = false,
			              		BodyHash = "1234",
			              		SignatureMethod = SignatureMethod.HmacSha1
			              	};

			var ex = Assert.Throws<OAuthException>(() => inspector.InspectContext(ProviderPhase.AccessProtectedResourceRequest, context));

			Assert.Equal("Encountered unexpected oauth_body_hash value in form-encoded request", ex.Message);
		}
	}
}