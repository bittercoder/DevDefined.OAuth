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
using System.IO;
using System.Net;
using DevDefined.OAuth.Provider;

namespace DevDefined.OAuth.Framework
{
	public static class Error
	{
		public static Exception MissingRequiredOAuthParameter(IOAuthContext context, string parameterName)
		{
			var exception = new OAuthException(context, OAuthProblems.ParameterAbsent,
			                                   string.Format("Missing required parameter : {0}", parameterName));

			exception.Report.ParametersAbsent.Add(parameterName);

			return exception;
		}

		public static Exception OAuthAuthenticationFailure(string errorMessage)
		{
			return new Exception(string.Format("OAuth authentication failed, message was: {0}", errorMessage));
		}

		public static Exception TokenCanNoLongerBeUsed(string token)
		{
			return new Exception(string.Format("Token \"{0}\" is no longer valid", token));
		}

		public static Exception FailedToParseResponse(string parameters)
		{
			return new Exception(string.Format("Failed to parse response string \"{0}\"", parameters));
		}

		public static Exception UnknownSignatureMethod(string signatureMethod)
		{
			return new Exception(string.Format("Unknown signature method \"{0}\"", signatureMethod));
		}

		public static Exception ForRsaSha1SignatureMethodYouMustSupplyAssymetricKeyParameter()
		{
			return
				new Exception(
					"For the RSASSA-PKCS1-v1_5 signature method you must use the constructor which takes an additional AssymetricAlgorithm \"key\" parameter");
		}

		public static Exception RequestFailed(WebException innerException)
		{
			var response = innerException.Response as HttpWebResponse;

			if (response != null)
			{
				using (var reader = new StreamReader(innerException.Response.GetResponseStream()))
				{
					string body = reader.ReadToEnd();

					return
						new Exception(
							string.Format(
								"Request for uri: {0} failed.\r\nstatus code: {1}\r\nheaders: {2}\r\nbody:\r\n{3}",
								response.ResponseUri, response.StatusCode, response.Headers, body), innerException);
				}
			}

			return innerException;
		}

		public static Exception EmptyConsumerKey()
		{
			throw new Exception("Consumer key is null or empty");
		}

		public static Exception RequestMethodHasNotBeenAssigned(string parameter)
		{
			return new Exception(string.Format("The RequestMethod parameter \"{0}\" is null or empty.", parameter));
		}

		public static Exception FailedToValidateSignature(IOAuthContext context)
		{
			return new OAuthException(context, OAuthProblems.SignatureInvalid, "Failed to validate signature");
		}

		public static Exception FailedToValidateBodyHash(IOAuthContext context)
		{
			return new OAuthException(context, OAuthProblems.BodyHashInvalid, "Failed to validate body hash");
		}

		public static Exception UnknownConsumerKey(IOAuthContext context)
		{
			return new OAuthException(context, OAuthProblems.ConsumerKeyUnknown,
			                          string.Format("Unknown Consumer (Realm: {0}, Key: {1})", context.Realm,
			                                        context.ConsumerKey));
		}

		public static Exception AlgorithmPropertyNotSetOnSigningContext()
		{
			return
				new Exception(
					"Algorithm Property must be set on SingingContext when using an Assymetric encryption method such as RSA-SHA1");
		}

		public static Exception SuppliedTokenWasNotIssuedToThisConsumer(string expectedConsumerKey,
		                                                                string actualConsumerKey)
		{
			return
				new Exception(
					string.Format("Supplied token was not issued to this consumer, expected key: {0}, actual key: {1}",
					              expectedConsumerKey, actualConsumerKey));
		}

		public static Exception AccessDeniedToProtectedResource(AccessOutcome outcome)
		{
			Uri uri = outcome.Context.GenerateUri();

			if (string.IsNullOrEmpty(outcome.AdditionalInfo))
			{
				return new AccessDeniedException(outcome, string.Format("Access to resource \"{0}\" was denied", uri));
			}

			return new AccessDeniedException(outcome,
			                                 string.Format("Access to resource: {0} was denied, additional info: {1}",
			                                               uri, outcome.AdditionalInfo));
		}

		public static Exception ConsumerHasNotBeenGrantedAccessYet(IOAuthContext context)
		{
			return new OAuthException(context, OAuthProblems.PermissionUnknown,
			                          "The decision to give access to the consumer has yet to be made, please try again later.");
		}

		public static Exception ConsumerHasBeenDeniedAccess(IOAuthContext context)
		{
			return new OAuthException(context, OAuthProblems.PermissionDenied,
			                          "The consumer was denied access to this resource.");
		}

		public static Exception CantBuildProblemReportWhenProblemEmpty()
		{
			return new Exception("Can't build problem report when \"Problem\" property is null or empty");
		}

		public static Exception NonceHasAlreadyBeenUsed(IOAuthContext context)
		{
			return new OAuthException(context, OAuthProblems.NonceUsed,
			                          string.Format("The nonce value \"{0}\" has already been used", context.Nonce));
		}

		public static Exception ThisConsumerRequestHasAlreadyBeenSigned()
		{
			return new Exception("The consumer request for consumer \"{0}\" has already been signed");
		}

		public static Exception CallbackWasNotConfirmed()
		{
			return new Exception("Callback was not confirmed");
		}

		public static Exception RejectedRequiredOAuthParameter(IOAuthContext context, string parameter)
		{
			return new OAuthException(context, OAuthProblems.ParameterRejected, string.Format("The parameter \"{0}\" was rejected", parameter));
		}

		public static Exception UnknownToken(IOAuthContext context, string token)
		{
			return new OAuthException(context, OAuthProblems.TokenRejected, string.Format("Unknown or previously rejected token \"{0}\"", token));
		}

		public static Exception UnknownToken(IOAuthContext context, string token, Exception exception)
		{
			return new OAuthException(context, OAuthProblems.TokenRejected, string.Format("Unknown or previously rejected token \"{0}\"", token), exception);
		}

		public static Exception RequestForTokenMustNotIncludeTokenInContext(IOAuthContext context)
		{
			throw new OAuthException(context, OAuthProblems.ParameterRejected, "When obtaining a request token, you must not supply the oauth_token parameter");
		}

		public static Exception ExperiencingIssueWithCreatingUriDueToMissingAppConfig(ArgumentNullException argumentException)
		{
			return
				new Exception(
					"It appears this may be the first Uri constructed by this AppDomain, and you have no App.config or Web.config file - which has triggered an unusual edge case: see this blog post from more details - http://ayende.com/Blog/archive/2010/03/04/is-select-system.uri-broken.aspx",
					argumentException);
		}

		public static Exception EncounteredUnexpectedBodyHashInFormEncodedRequest(IOAuthContext context)
		{
			throw new OAuthException(context, OAuthProblems.ParameterRejected, "Encountered unexpected oauth_body_hash value in form-encoded request");
		}

    public static Exception EmptyXAuthMode(IOAuthContext context)
    {
      throw new OAuthException(context, OAuthProblems.ParameterAbsent, "The x_auth_mode parameter must be present");
    }

    public static Exception InvalidXAuthMode(IOAuthContext context)
	  {
      throw new OAuthException(context, OAuthProblems.ParameterRejected, "The x_auth_mode parameter is invalid");
    }

    public static Exception EmptyXAuthUsername(IOAuthContext context)
    {
      throw new OAuthException(context, OAuthProblems.ParameterAbsent, "The x_auth_username parameter must be present");
    }

	  public static Exception EmptyXAuthPassword(IOAuthContext context)
	  {
      throw new OAuthException(context, OAuthProblems.ParameterAbsent, "The x_auth_password parameter must be present");
    }

    public static Exception FailedXAuthAuthentication(IOAuthContext context)
    {
      throw new OAuthException(context, OAuthProblems.ParameterRejected, "Authentication failed with the specified username and password");
    }
	}
}