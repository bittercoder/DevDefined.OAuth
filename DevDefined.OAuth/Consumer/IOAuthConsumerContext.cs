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

using System.Security.Cryptography;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
  /// <summary>
  /// A consumer context is used to identify a consumer, and to sign a context on behalf 
  /// of a consumer using an optional supplied token.
  /// </summary>
  public interface IOAuthConsumerContext
  {
    string Realm { get; set; }
    string ConsumerKey { get; set; }
    string ConsumerSecret { get; set; }
    string SignatureMethod { get; set; }
    AsymmetricAlgorithm Key { get; set; }
    bool UseHeaderForOAuthParameters { get; set; }
    void SignContext(IOAuthContext context);
    void SignContextWithToken(IOAuthContext context, IToken token);
  }
}