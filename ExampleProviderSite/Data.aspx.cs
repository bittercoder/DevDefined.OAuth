using System;
using System.Linq;
using System.Web.UI;
using System.Xml.Linq;
using DevDefined.OAuth.Core;
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