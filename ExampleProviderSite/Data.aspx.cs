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
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using ExampleProviderSite.Repositories;

namespace ExampleProviderSite
{
    public partial class Data : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthContext context = new OAuthContextBuilder().FromHttpRequest(Request);

            IOAuthProvider provider = OAuthServicesLocator.Services.Provider;

            TokenRepository tokenRepository = OAuthServicesLocator.Services.TokenRepository;

            try
            {
                provider.AccessProtectedResourceRequest(context);

                Models.AccessToken accessToken = tokenRepository.GetAccessToken(context.Token);

                string userName = accessToken.UserName;

                XDocument contactsDocument = GetContactsForUser(userName);

                Response.ContentType = "text/xml";
                Response.Write(contactsDocument);
                Response.End();
            }
            catch (OAuthException authEx)
            {
                // access was denied for some reason, so we set the status code to 403.
                Response.StatusCode = 403;
                Response.Write(authEx.Report.ToString());
                Response.End();
            }
        }

        public XDocument GetContactsForUser(string userName)
        {
            var repository = new ContactsRepository();

            return new XDocument(
                new XElement("contacts",
                             new XAttribute("for", userName),
                             repository.GetContactsForUser(userName)
                                 .Select(contact => new XElement("contact",
                                                                 new XAttribute("name", contact.FullName),
                                                                 new XAttribute("email", contact.Email)))));
        }
    }
}