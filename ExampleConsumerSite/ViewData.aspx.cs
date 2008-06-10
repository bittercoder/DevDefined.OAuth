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
﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using ExampleConsumerSite.Properties;

namespace ExampleConsumerSite
{
    public partial class ViewData : OAuthPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthSession session = CreateSession();

            string accessTokenString = Request[Parameters.OAuth_Token];

            session.AccessToken = (IToken) Session[accessTokenString];

            try
            {
                var results = session.Request()
                    .Get()
                    .ForUrl(Settings.Default.DataUrl)
                    .SignWithToken()
                    .ToDocument();

                BindResults(results);
            }
            catch (WebException webEx)
            {
                var response = (HttpWebResponse) webEx.Response;

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ShowErrorsInResultsPanelFor(response);
                }
            }
        }

        private void BindResults(XDocument document)
        {
            UserName.Text = document.Root.Attribute("for").Value;

            ContactsList.DataSource = document.Root.Elements("contact").Select(
                element => new { Email = element.Attribute("email"), FullName = element.Attribute("name").Value });

            ContactsList.DataTextField = "FullName";
            ContactsList.DataValueField = "Email";
            ContactsList.DataBind();
        }

        private static string ReadBody(WebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private void ShowErrorsInResultsPanelFor(WebResponse response)
        {
            ResultsPanel.Visible = false;

            NameValueCollection parameters = HttpUtility.ParseQueryString(ReadBody(response));

            ErrorInfo.Text = "Access was denied to resource.<br/><br/>";

            foreach (string key in parameters.Keys)
                ErrorInfo.Text += key + " => " + parameters[key] + "<br/>";
        }        
    }
}