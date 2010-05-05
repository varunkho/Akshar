using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using Akshar.Lib;
using Akshar.Lib.Data;



namespace Akshar
{
    public partial class addVKL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlDataReader sdr;
            using ( DataAccess da = new DataAccess())
{
                        if (User.Identity.IsAuthenticated)
  sdr =(SqlDataReader) da.ExecuteSPForDataReader("Sel_VKLs", new SqlParameter[] {Parameters.Int("@userId", Member.CurrentUser)});
            else
                sdr = (SqlDataReader) da.ExecuteSPForDataReader("Sel_VKLs", null);

                        this.dsVkl.Text = "<script>\n" + JS.CreateObject(sdr, "vkls", true) + "</script>";
                sdr.Close();
}
            }



        protected void cmbLang_DataBound(object sender, EventArgs e)
        {
            cmbLang.Items.Insert(0, new ListItem { Value = string.Empty, Text = "--Select--" });
                    }

    }
}
