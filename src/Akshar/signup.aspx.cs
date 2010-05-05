using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text.RegularExpressions;

using System.Web.Security;
using Akshar.Lib;


namespace Akshar
{

    public partial class signup : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
if (Context.User.Identity.IsAuthenticated)
{
    Response.Redirect("~/account.aspx");
return;
}

newMember.DefaultButton = Submit.ClientID;
           }

protected void submit_Click(object sender, System.EventArgs e)
{
        if (!this.IsValid) return;

            try
{
var res = Member.Exists(this.UserName.Text);
                if (res > 0)
{
                        this.ErrorProvider.Text = "User Name is already taken.";
                        return;
}
                    else if (res == (int)Results.SomeError)
{
                        Response.Redirect(IOLocations.Error_Page, true);
                        return;
                }

                 var NewUser = Member.Create(this.UserName.Text, this.Password.Text, this.Email.Text, this.Name.Text);
if (NewUser > 0)
{
                                            Member.BeginSession(NewUser);
                        FormsAuthentication.RedirectFromLoginPage(this.UserName.Text, this.RememberMe.Checked);
                        }
                    else
                        Response.Redirect(IOLocations.Error_Page, true);
                }
            catch (Exception ex) {
                Common.LogError(ex.Message, Common.GetMethodName(MethodBase.GetCurrentMethod(   )));
            }
        }

#region Validates
    protected void UserNameRange_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
{
        if (args.Value.Length < 5 || args.Value.Length > 15)
            args.IsValid = false;
        else
            args.IsValid = true;
        }

    protected void PasswordRange_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
{
        if (args.Value.Length < 6 || args.Value.Length > 20)
            args.IsValid = false;
        else
            args.IsValid = true;
        }

    protected void NameRange_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
{
        if (args.Value.Length < 4 || args.Value.Length > 50)
            args.IsValid = false;
        else
            args.IsValid = true;
    }
#endregion

    }




}
