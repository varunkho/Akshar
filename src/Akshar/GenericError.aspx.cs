using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Akshar.Lib;

namespace Akshar
{
    public partial class GenericError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Context.Items.Contains(Common.C_ErrorHandled))
{
    if (Context.AllErrors == null)
        return;
    for (int i = 0; i < Context.AllErrors.Length;i++ )
        Common.LogError(Context.AllErrors[i].Message + "\r\nStack Trace:\r\n" + Context.AllErrors[i].StackTrace, "Unhandled Error");
} else
Context.Items.Remove(Common.C_ErrorHandled);
        }
    }

}
