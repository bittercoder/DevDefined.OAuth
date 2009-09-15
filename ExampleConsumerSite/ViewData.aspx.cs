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
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Utility;
using ExampleConsumerSite.Properties;

namespace ExampleConsumerSite
{
  public partial class ViewData : OAuthPage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      var session = CreateSession();

      string accessTokenString = Request[Parameters.OAuth_Token];

      session.AccessToken = ((IToken) Session[accessTokenString]) ?? new TokenBase { ConsumerKey = "fake", Token = "fake"};

      try
      {
        string response = session.Request()
          .Get()
          .ForUrl(Settings.Default.DataUrl)
          .SignWithToken()
          .ReadBody();

        xmlFeed.DocumentContent = response;
      }
      catch (WebException webEx)
      {
        var response = (HttpWebResponse) webEx.Response;

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
          ShowOAuthProblemDetails(response);
        }
        else
        {
          ShowStatusCodeDetails(response);
        }
      }
    }

    void ShowStatusCodeDetails(HttpWebResponse response)
    {
      ResultsPanel.Visible = false;

      NameValueCollection parameters = HttpUtility.ParseQueryString(response.ReadToEnd());

      ErrorInfo.Text = string.Format("Request failed, status code was: {0}, status description: {1}", response.StatusCode, response.StatusDescription);
    }

    void ShowOAuthProblemDetails(WebResponse response)
    {
      ResultsPanel.Visible = false;

      NameValueCollection parameters = HttpUtility.ParseQueryString(response.ReadToEnd());

      ErrorInfo.Text = "Access was denied to resource.<br/><br/>";

      foreach (string key in parameters.Keys)
        ErrorInfo.Text += key + " => " + parameters[key] + "<br/>";
    }
  }
}