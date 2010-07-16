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
  public static class Parameters
  {
    public const string OAuth_Acceptable_Timestamps = "oauth_acceptable_timestamps";
    public const string OAuth_Acceptable_Versions = "oauth_acceptable_versions";
    public const string OAuth_Authorization_Header = "Authorization";
    public const string OAuth_Callback = "oauth_callback";
    public const string OAuth_Callback_Confirmed = "oauth_callback_confirmed";
    public const string OAuth_Consumer_Key = "oauth_consumer_key";
    public const string OAuth_Nonce = "oauth_nonce";
    public const string OAuth_Parameters_Absent = "oauth_parameters_absent";
    public const string OAuth_Parameters_Rejected = "oauth_parameters_rejected";
    public const string OAuth_Problem = "oauth_problem";
    public const string OAuth_Problem_Advice = "oauth_problem_advice";
    public const string OAuth_Signature = "oauth_signature";
    public const string OAuth_Signature_Method = "oauth_signature_method";
    public const string OAuth_Timestamp = "oauth_timestamp";
    public const string OAuth_Token = "oauth_token";
    public const string OAuth_Token_Secret = "oauth_token_secret";
    public const string OAuth_Verifier = "oauth_verifier";
    public const string OAuth_Version = "oauth_version";
    public const string OAuth_Session_Handle = "oauth_session_handle";
    public const string OAuth_Expires_In = "oauth_expires_in";
    public const string OAuth_Authorization_Expires_In = "oauth_authorization_expires_in";
    public const string OAuthParameterPrefix = "oauth_";
    public const string Realm = "realm";
    public const string HttpFormEncoded = "application/x-www-form-urlencoded; charset=utf-8";
  }
}