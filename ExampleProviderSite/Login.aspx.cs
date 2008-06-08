using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace ExampleProviderSite
{
    public partial class Login : System.Web.UI.Page
    {
        protected void LoginButton_Click(object sender, EventArgs e)
        {
            if (FormsAuthentication.Authenticate(UserName.Text, Password.Text))
                FormsAuthentication.RedirectFromLoginPage(UserName.Text, true);
            else
                FailureText.Text = "Invalid Login";
        }

    }
}
