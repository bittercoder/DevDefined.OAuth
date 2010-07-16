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
using System.Security.Cryptography.X509Certificates;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Tests;
using WatiN.Core;
using Xunit;

namespace DevDefined.Oauth.IntegrationTests.Consumer
{
  public class GoogleIntegrationTests
  {
    readonly X509Certificate2 certificate = TestCertificates.OAuthTestCertificate();

    IOAuthSession CreateGoogleContactsSession()
    {
      var consumerContext = new OAuthConsumerContext
        {
          ConsumerKey = "weitu.googlepages.com",
          SignatureMethod = SignatureMethod.RsaSha1,
          Key = certificate.PrivateKey
        };

      return new OAuthSession(consumerContext, "https://www.google.com/accounts/OAuthGetRequestToken",
                              "https://www.google.com/accounts/accounts/OAuthAuthorizeToken",
                              "https://www.google.com/accounts/OAuthGetAccessToken ")
        .WithQueryParameters(new {scope = "https://www.google.com/m8/feeds"})
        .RequiresCallbackConfirmation();
    }
		
    [Fact]
    public void RequestContacts()
    {
      // this test does a full end-to-end integration (request token, user authoriazation, exchanging request token
      // for an access token and then using then access token to retrieve some data).

      // the access token is directly associated with a google user, by them logging in and granting access
      // for your request - thus the client is never exposed to the users credentials (not even their login).

      IOAuthSession consumer = CreateGoogleContactsSession();

      using (With.NoCertificateValidation())
      {
        IToken requestToken = consumer.GetRequestToken();

        string userAuthorize = consumer.GetUserAuthorizationUrlForToken(requestToken, null);

        string verificationCode;

        using (var ie = new IE(userAuthorize))
        {
          Link overrideLink = ie.Link("overridelink");
          if (overrideLink.Exists) overrideLink.Click();

          if (ie.Form("gaia_loginform").Exists)
          {
            ie.TextField("Email").Value = "oauthdotnet@gmail.com";
            ie.TextField("Passwd").Value = "oauth_password";
            ie.Form("gaia_loginform").Submit();
          }

          ie.Button("allow").Click();

          string html = ie.Html;

          Assert.True(html.Contains("Authorized") || html.Contains("successfully granted"));

          int index = html.IndexOf("verification code:");

          Assert.True(index > 0);

          int startIndex = html.IndexOf("<B>", index, StringComparison.InvariantCultureIgnoreCase);
          int endIndex = html.IndexOf("</B>", startIndex + 1, StringComparison.InvariantCultureIgnoreCase);

          verificationCode = html.Substring(startIndex + 3, endIndex - (startIndex + 3));
        }

        // this will implicitly set AccessToken on the current session... 

        IToken accessToken = consumer.ExchangeRequestTokenForAccessToken(requestToken, verificationCode);

        try
        {         					
					string responseText = consumer.Request().Get().ForUrl("https://www.google.com/m8/feeds/contacts/default/base").ToString();
					Assert.True(responseText.Contains("alex@devdefined.com"));
        }
        catch (WebException webEx)
        {
          var response = (HttpWebResponse) webEx.Response;
          
					using (var reader = new StreamReader(response.GetResponseStream()))
          {
            throw new Exception("WebException was thrown upon request, response content follows:\r\n\r\n" + reader.ReadToEnd(), webEx);
          }
        }
      }
    }


    // TODO: revist this test, I'm not sure if this was a test or just an experiment (wrote it a couple of months ago).

    /*[Fact]
    public void DenyCallback()
    {
      // this test does a full end-to-end integration (request token, user authoriazation, exchanging request token
      // for an access token and then using then access token to retrieve some data).

      // the access token is directly associated with a google user, by them logging in and granting access
      // for your request - thus the client is never exposed to the users credentials (not even their login).

      var consumerContext = new OAuthConsumerContext
      {
        ConsumerKey = "weitu.googlepages.com",
        SignatureMethod = SignatureMethod.RsaSha1,
        Key = certificate.PrivateKey
      };

      var consumer = new OAuthSession(consumerContext, "https://www.google.com/accounts/OAuthGetRequestToken",
                              "https://www.google.com/accounts/accounts/OAuthAuthorizeToken",
                              "https://www.google.com/accounts/OAuthGetAccessToken ", "http://localhost:1897/callback.aspx")
        .WithQueryParameters(new { scope = "https://www.google.com/m8/feeds" })
        .RequiresCallbackConfirmation();

      using (With.NoCertificateValidation())
      {
        IToken requestToken = consumer.GetRequestToken();

        string userAuthorize = consumer.GetUserAuthorizationUrlForToken(requestToken, null);

        string verificationCode;

        using (var ie = new IE(userAuthorize))
        {
          Link overrideLink = ie.Link("overridelink");
          if (overrideLink.Exists) overrideLink.Click();

          if (ie.Form("gaia_loginform").Exists)
          {
            ie.TextField("Email").Value = "oauthdotnet@gmail.com";
            ie.TextField("Passwd").Value = "oauth_password";
            ie.Form("gaia_loginform").Submit();
          }
 ie.Button("deny").Click();

          Console.WriteLine(ie.Url);

          verificationCode = new OAuthContextBuilder().FromUri("GET", ie.Uri).Verifier;                  
        }

        // this will implicitly set AccessToken on the current session... 

        IToken accessToken = consumer.ExchangeRequestTokenForAccessToken(requestToken, verificationCode);

        try
        {
          string responseText = consumer.Request().Get().ForUrl("https://www.google.com/m8/feeds/contacts/default/base").ToString();

          Assert.IsTrue(responseText.Contains("alex@devdefined.com"));
        }
        catch (WebException webEx)
        {
          var response = (HttpWebResponse) webEx.Response;
          using (var reader = new StreamReader(response.GetResponseStream()))
          {
            Console.WriteLine(reader.ReadToEnd());
          }
          Assert.Fail();
        }
      }      
    }*/


    [Fact]
    public void RequestTokenForRsaSha1()
    {
      using (With.NoCertificateValidation())
      {
        // simple test, just requests a token using RSHA1... 

        IOAuthSession session = CreateGoogleContactsSession();

        IToken token = session.GetRequestToken();
        Assert.Equal("weitu.googlepages.com", token.ConsumerKey);
        Assert.True(token.Token.Length > 0);
        Assert.NotNull(token.TokenSecret); // interesting - secret used to be null, now it's a randomly generated value... hmmm
      }
    }
  }
}