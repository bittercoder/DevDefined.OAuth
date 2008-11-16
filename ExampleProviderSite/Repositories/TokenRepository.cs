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

using System.Collections.Generic;
using DevDefined.OAuth.Storage;

namespace ExampleProviderSite.Repositories
{
  /// <summary>
  /// A simplistic in-memory repository for access and request token models - the example implementation of
  /// <see cref="ITokenStore" /> relies on this repository - normally you would make use of repositories
  /// wired up to your domain model i.e. NHibernate, Entity Framework etc.
  /// </summary>    
  public class TokenRepository
  {
    readonly Dictionary<string, Models.AccessToken> _accessTokens = new Dictionary<string, Models.AccessToken>();
    readonly Dictionary<string, Models.RequestToken> _requestTokens = new Dictionary<string, Models.RequestToken>();

    public Models.RequestToken GetRequestToken(string token)
    {
      return _requestTokens[token];
    }

    public Models.AccessToken GetAccessToken(string token)
    {
      return _accessTokens[token];
    }

    public void SaveRequestToken(Models.RequestToken token)
    {
      _requestTokens[token.Token] = token;
    }

    public void SaveAccessToken(Models.AccessToken token)
    {
      _accessTokens[token.Token] = token;
    }
  }
}