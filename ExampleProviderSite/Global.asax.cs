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
using System.Web;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Testing;
using ExampleProviderSite.Implementation;
using ExampleProviderSite.Repositories;

namespace ExampleProviderSite
{
  public interface IOAuthServices
  {
    IOAuthProvider Provider { get; }
    TokenRepository TokenRepository { get; }
  }

  public static class OAuthServicesLocator
  {
    public static IOAuthServices Services
    {
      get { return (HttpContext.Current.ApplicationInstance as IOAuthServices); }
    }
  }

  public class Global : HttpApplication, IOAuthServices
  {
    static IOAuthProvider _provider;
    static TokenRepository _repository;

    #region IOAuthServices Members

    public IOAuthProvider Provider
    {
      get { return _provider; }
    }

    public TokenRepository TokenRepository
    {
      get { return _repository; }
    }

    #endregion

    protected void Application_Start(object sender, EventArgs e)
    {
      var consumerStore = new TestConsumerStore();
      var nonceStore = new TestNonceStore();
      _repository = new TokenRepository();
      var tokenStore = new SimpleTokenStore(_repository);

      _provider = new OAuthProvider(tokenStore,
                                    new SignatureValidationInspector(consumerStore),
                                    new NonceStoreInspector(nonceStore),
                                    new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
                                    new ConsumerValidationInspector(consumerStore));
    }
  }
}