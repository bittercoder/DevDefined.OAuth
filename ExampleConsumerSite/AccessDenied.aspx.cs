using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using DevDefined.OAuth.Framework;

namespace ExampleConsumerSite
{
  public partial class AccessDenied : Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      var report = (OAuthProblemReport) Session["problem"];

      NameValueCollection parameters = HttpUtility.ParseQueryString(report.ToString());

      foreach (string key in parameters.Keys)
        problemReport.Text += key + " => " + parameters[key] + "<br/>";
    }
  }
}