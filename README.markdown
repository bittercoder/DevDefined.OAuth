Important Notice
================

This library is no longer actively maintained, and the author does not provide any support.


![DevDefined.OAuth logo][1]

Introduction
------------

The DevDefined.OAuth project is a library for creating both OAuth consumers and providers on the .Net Framework.  It currently targets the .Net Framework 3.5 and above, and is written in C#.

What is OAuth
-------------

The definition (from wikipedia) is:

> OAuth is an open protocol that allows users to share their private resources (e.g. photos, videos, contact lists) stored on one site with another site without having to hand out their username and password.

OAuth provides a standardised way to handle delegated Authentication through a series of exchanges, called an authentication flow:

![OAuth authentication flow][2]


What's supported
----------------

The DevDefined.OAuth library currently supports building consumers (clients) and providers (servers) for both OAuth 1.0 and 1.0a.

The library is designed to be used in both web applications and thick client apps.

Quick Consumer Example
----------------------

    X509Certificate2 certificate = TestCertificates.OAuthTestCertificate();
    
    string requestUrl = "https://www.google.com/accounts/OAuthGetRequestToken";
    string userAuthorizeUrl = "https://www.google.com/accounts/accounts/OAuthAuthorizeToken";
    string accessUrl = "https://www.google.com/accounts/OAuthGetAccessToken";
    string callBackUrl = "http://www.mysite.com/callback";
    
    var consumerContext = new OAuthConsumerContext
    {
    	ConsumerKey = "weitu.googlepages.com",
    	SignatureMethod = SignatureMethod.RsaSha1,
    	Key = certificate.PrivateKey
    };
    
    var session = new OAuthSession(consumerContext, requestUrl, userAuthorizeUrl, accessUrl)
    	.WithQueryParameters(new { scope = "http://www.google.com/m8/feeds" });
    
    // get a request token from the provider
    IToken requestToken = session.GetRequestToken();
    
    // generate a user authorize url for this token (which you can use in a redirect from the current site)
    string authorizationLink = session.GetUserAuthorizationUrlForToken(requestToken, callBackUrl);
    
    // exchange a request token for an access token
    IToken accessToken = session.ExchangeRequestTokenForAccessToken(requestToken);
    
    // make a request for a protected resource
    string responseText = session.Request().Get().ForUrl("http://www.google.com/m8/feeds/contacts/default/base").ToString();
    
Additional Resources
--------------------

**OAuth Resources**

  - [Official OAuth website][3]
  - [OAuth wiki][4]
  - [A guide to how OAuth works - for beginners][5]

**DevDefined OAuth Resources**

  - [OAuth page on googlecode (where the code used to be hosted)][6]
  - [Posts about OAuth / DevDefined.OAuth on the original authors blog (bittercoder)][7]

**Blogs**

  - [Example of using DevDefined.OAuth with Powershell][8]
  - [Owen's look into ASP.Net MVC and OAuth][9]
  - [Example of an OAuth Channel for use with WCF services][10]
  - [Xero's API Guides (feature DevDefined.OAuth examples)][11]

**Forks**

  - [Owen @ Xero's fork of DevDefined.OAuth - with support ASP.Net MVC][12]
  - [Xero's fork of DevDefined.OAuth - as used in their API examples][13]


Downloads/Releases
------------------

You can download releases from the [google code site][14].

  [1]: https://github.com/bittercoder/DevDefined.OAuth/raw/master/artifacts/DevDefinedOAuthTitle.png
  [2]: https://github.com/bittercoder/DevDefined.OAuth/raw/master/artifacts/Oauth_diagram.png
  [3]: http://www.oauth.net/
  [4]: http://wiki.oauth.net/
  [5]: http://dotnetkicks.com/webservices/OAuth_for_Beginners
  [6]: http://code.google.com/p/devdefined-tools/wiki/OAuth
  [7]: http://blog.bittercoder.com/CategoryView,category,OAuth.aspx
  [8]: http://www.leporelo.eu/blog.aspx?id=how-to-use-oauth-to-connect-to-twitter-in-powershell
  [9]: http://bgeek.net/blog/2009/3/3/oauth-mvcnet-revisited.html
  [10]: http://weblogs.asp.net/cibrax/archive/2008/11/14/oauth-channel-for-wcf-restful-services.aspx
  [11]: http://blog.xero.com/developer/api-guides/
  [12]: http://github.com/buildmaster/oauth-mvc.net/
  [13]: http://github.com/XeroAPI
  [14]: http://code.google.com/p/devdefined-tools/downloads/list
