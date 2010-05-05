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
using Akshar.Lib;

namespace Akshar
{
    public partial class feedback : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Member.SessionBegun)
            {
                tbFeed.Rows.Remove(trEmail);
                tbFeed.Rows.Remove(trEmlDesc);
                tbFeed.Rows.Remove(trName);
                            }
        }

#region Validates
        protected void NameRange_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (args.Value.Length > 50)
                args.IsValid = false;
            else
                args.IsValid = true;
        }

        protected void SubjectRange_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (args.Value.Length > 100)
                args.IsValid = false;
            else
                args.IsValid = true;
        }

        protected void DescriptionRange_ServerValidate(Object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (args.Value.Length > 1000)
                args.IsValid = false;
            else
                args.IsValid = true;
        }
#endregion

        protected void submit_Click(object sender, System.EventArgs e)
        {
            string name, email;
if (Member.SessionBegun) {
    if (!SubjectRequired.IsValid || !SubjectRange.IsValid || !DescriptionRequired.IsValid || !DescriptionRange.IsValid) {
        ErrorProvider.Text = "Please correct the error(s) and then try again.";
        return;
    }
    name = User.Identity.Name;
    email = string.Empty;
} else {
    if (!this.IsValid) {
        ErrorProvider.Text = "Please correct the error(s) and then try again.";
        return;
    }
    name = Name.Text;
    email = Email.Text;
}

if (Common.Feedback(Type.SelectedItem.Text, Subject.Text, Description.Text, name, email))
    msg.InnerText = "Thank you for helping us improve Akshar. Your feedback has been successfully submitted to our administration system.";
else
    ErrorProvider.Text = "We are sorry for the inconvenience, but Some error occurred while processing this action, please resubmit your feedback to try again.";
                    }

    }
}
