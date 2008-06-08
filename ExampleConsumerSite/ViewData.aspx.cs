using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Core;
using DevDefined.OAuth.Tests;
using ExampleConsumerSite.Properties;

namespace ExampleConsumerSite
{
    public partial class ViewData : OAuthPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var consumer = CreateConsumer();

            string accessTokenString = Request[Parameters.OAuth_Token];

            var accessToken = (IToken)Session[accessTokenString];

            var dataContext = new OAuthContextFactory().FromUri("GET", new Uri(Settings.Default.DataUrl));

            try
            {
                var response = consumer.GetResponse(dataContext, accessToken);
                BindResultsInResponse(response);
            }
            catch (WebException webEx)
            {
                var response = (HttpWebResponse)webEx.Response;

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ResultsPanel.Visible = false;

                    var parameters = HttpUtility.ParseQueryString(ReadBody(response));

                    ErrorInfo.Text = "Access was denied to resource.<br/><br/>";

                    foreach (string key in parameters.Keys)
                        ErrorInfo.Text += key + " => " + parameters[key] + "<br/>";
                }
            }
        }

        private string ReadBody(WebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private void BindResultsInResponse(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                using (var xmlReader = XmlReader.Create(stream))
                {
                    var root = XDocument.Load(xmlReader).Root;

                    UserName.Text = root.Attribute("for").Value;

                    ContactsList.DataSource = root.Elements("contact").Select(
                        element => new { Email = element.Attribute("email"), FullName = element.Attribute("name").Value });

                    ContactsList.DataTextField = "FullName";
                    ContactsList.DataValueField = "Email";
                    ContactsList.DataBind();
                }
            }
        }
    }
}
