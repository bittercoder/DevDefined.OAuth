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
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Threading;
using ExampleDevDefined.OAuth.Wcf.Repositories;
using Microsoft.ServiceModel.Web;

namespace ExampleOAuthChannel
{
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  [ServiceContract]
  public class FeedService
  {
    /// <summary>
    /// Returns an Atom feed.
    /// </summary>
    /// <returns>Atom feed in response to a HTTP GET request at URLs conforming to the URI template of the WebGetAttribute.</returns>
    [WebHelp(Comment = "Sample contacts feed.")]
    [WebGet]
    [OperationContract]
    public Atom10FeedFormatter GetContacts()
    {
      var items = new List<SyndicationItem>();

      var repository = new ContactsRepository();

      foreach (Contact contact in repository.GetContactsForUser(Thread.CurrentPrincipal.Identity.Name))
      {
        items.Add(new SyndicationItem
          {
            Id = String.Format(CultureInfo.InvariantCulture, "http://contacts.org/{0}", Guid.NewGuid()),
            Title = new TextSyndicationContent(contact.FullName),
            LastUpdatedTime = DateTime.UtcNow,
            Authors =
              {
                new SyndicationPerson
                  {
                    Name = "Sample Author"
                  }
              },
            Content = new TextSyndicationContent(contact.Email),
          });
      }

      // create the feed containing the syndication items.

      var feed = new SyndicationFeed
        {
          // The feed must have a unique stable URI id

          Id = "http://contacts/Feed",
          Title = new TextSyndicationContent("Contacts feed"),
          Items = items
        };

      feed.AddSelfLink(WebOperationContext.Current.IncomingRequest.GetRequestUri());

      WebOperationContext.Current.OutgoingResponse.ContentType = ContentTypes.Atom;

      return feed.GetAtom10Formatter();
    }
  }
}