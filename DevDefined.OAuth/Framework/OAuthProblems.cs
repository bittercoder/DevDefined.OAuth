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

namespace DevDefined.OAuth.Framework
{
  public static class OAuthProblems
  {
    public const string AdditionalAuthorizationRequired = "additional_authorization_required";
    public const string ConsumerKeyRefused = "consumer_key_refused";
    public const string ConsumerKeyRejected = "consumer_key_rejected";
    public const string ConsumerKeyUnknown = "consumer_key_unknown";
    public const string NonceUsed = "nonce_used";
    public const string ParameterAbset = "parameter_absent";
    public const string ParameterRejected = "parameter_rejected";
    public const string PermissionDenied = "permission_denied";
    public const string PermissionUnknown = "permission_unknown";
    public const string SignatureInvalid = "signature_invalid";
    public const string SignatureMethodRejected = "signature_method_rejected";
    public const string TimestampRefused = "timestamp_refused";
    public const string TokenExpired = "token_expired";
    public const string TokenRejected = "token_rejected";
    public const string TokenRevoked = "token_revoked";
    public const string TokenUsed = "token_used";
    public const string UserRefused = "user_refused";
    public const string VersionRejected = "version_rejected";
  }
}