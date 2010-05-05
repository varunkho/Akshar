using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Akshar.Lib;

namespace Akshar
{
    public partial class signin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/account.aspx");
                return;
            }

form1.DefaultButton = Submit.ClientID;
        }

protected void submit_Click(object sender, System.EventArgs e)
{
        if (!this.IsValid) return;

string DBUserName = string.Empty;
            var ValidUser = Member.Validate(this.UserName.Text, this.Password.Text, ref DBUserName);
            if (ValidUser > 0)
{
                    Member.BeginSession(ValidUser);
                    FormsAuthentication.RedirectFromLoginPage(DBUserName, this.RememberMe.Checked);
}
                else if (ValidUser == (int)Results.InvalidUserName || ValidUser == (int)Results.InvalidPassword)
                    this.ErrorProvider.Text = "User Name/Password does not match.";
                else
                {
                Context.Items.Add(Common.C_ErrorHandled, true);
                    Response.Redirect(IOLocations.Error_Page, true);
                    }
}

    }
}
