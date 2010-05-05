using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Akshar.Lib;
using Akshar.Lib.Data;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Akshar
{

    public partial class account : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
                Response.Redirect("~/signin.aspx");

            if (Request.QueryString["act"] == "vis")
            {
                int vklId, visibility;
                if (int.TryParse(Request.QueryString["id"], out vklId))
                {
                    if (Member.Roles.IsSupremeAdministrator)
                    {
                        if (!int.TryParse(Request.QueryString["val"], out visibility) || !Enum.IsDefined(typeof(VKLVisibility), visibility))
                        {
Common.LogError("Illegal parameters {" + Request.RawUrl + "}", "Account.init");
                            Response.End();
                            return;
                        }
                    }
                        else
                        visibility = (int)VKLVisibility.Public;

                    using (DataAccess da = new DataAccess())
                    {
                        da.ExecuteSP("Upd_VKLVisibility", new System.Data.Common.DbParameter[] {new SqlParameter("@id", vklId), new SqlParameter("@newVisibility", visibility), (Member.Roles.IsSupremeAdministrator ? null : new SqlParameter("@userId", Member.CurrentUser))});
                    }
                                                    }

                Response.End();
                return;
            }

            this.VKLLists.ItemCreated += new RepeaterItemEventHandler(VKLLists_ItemCreated);
            this.LoginStatus1.LoggedOut += new EventHandler(LoginStatus1_LoggedOut);
        }

        protected void LoginStatus1_LoggedOut(object sender, EventArgs e)
        {
Member.EndSession();
        }

        void VKLLists_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem == null)
                return;

            Literal l = e.Item.FindControl("lstate") as Literal;
            if (l != null)
            {
            VKLVisibility visibility =(VKLVisibility) Enum.Parse(typeof(VKLVisibility), ((DataRowView)e.Item.DataItem)["Visibility"].ToString());
switch (visibility) {
    case VKLVisibility.Public:
        l.Text = "Yes";
        break;
    case VKLVisibility.Default:
        l.Text = "Yes (default)";
        break;
}
            }
        }

        protected string VKLLink(object visibility, object VKLId)
        {
if (Convert.ToInt32(visibility) != (int)VKLVisibility.Default || Member.Roles.IsSupremeAdministrator)
return "keyboard.aspx?vid=" + VKLId.ToString();
else
return string.Empty;
        }

        protected void SqlDataSource1_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {
e.Command.Parameters[0].Value = Member.CurrentUser;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
if (!User.Identity.IsAuthenticated)
Response.Redirect("~/signin.aspx");

if (!IsPostBack)
{
VKLLists.DataSource = SqlDataSource1;
VKLLists.DataBind();
}
                    }

[System.Web.Services.WebMethod]
public static void setVKLs(string name, string type)
{
if (Member.SessionBegun)
{
try
{
string[] vkls = name.Split(new char[] {'|'}), types = type.Split(new char[] {'|'});
var set = Member.Settings;
set.ClearVKL();
for (int i = 0;i<vkls.Length;i++)
{
if (string.IsNullOrEmpty(vkls[i]))
continue;

set.AddVKL(vkls[i],types[i]);
}
set.save();
} catch (Exception ex)
{
Common.LogError(ex.Message, "Account.SetVKLs", (int)Results.IncomingDataFalt);
}
}
     }

}

}