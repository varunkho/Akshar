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
    public partial class service : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.QueryString.HasKeys() && Request.QueryString["method"] != null)
            {
                System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
                System.Text.StringBuilder text = new System.Text.StringBuilder();
                                switch (Request.QueryString["method"].ToLower())
                {
                    case "getvkl":
                        string name = Request.QueryString["name"], type = Request.QueryString["type"], jvar = Request.QueryString["jvar"], callVkm = Request.QueryString["callVkm"];
                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                            break;

                        json.Serialize(GetVKL(name, type), text);
                        if (string.IsNullOrEmpty(callVkm))
                        {
                            jvar = (string.IsNullOrEmpty(jvar) ? "vkl" : jvar);
                            Response.Write("var " + jvar + "=" + text.ToString());
                        }
                        else
                        {
                            Response.Write("(function() { var vkl=" + text.ToString() + ";VKM.loaded(vkl,\"" + name + "\",\"" + type + "\");})()");
                        }
                        break;

                }
                            }
                        Response.End();
        }


        public string GetVKL(string vklName, string vklType)
        {
            var type = (int)((vklType = vklType.ToUpper()) == "CHARACTER" ? (int)VKLTypes.Character : vklType == "KEY" ? (int)VKLTypes.Key : -1);
            if (string.IsNullOrEmpty(vklName) || type == -1) return string.Empty;

            var vkl = VKM.GetVKL(vklName, (VKLTypes)type);
            if (vkl != null && ((vkl.Visibility == VKLVisibility.Default || vkl.Visibility == VKLVisibility.Public) || (Member.SessionBegun && (Member.Roles.IsSupremeAdministrator || vkl.UserId == Member.CurrentUser))))
                return VKM.LoadRawVKLData(vkl.Name, vkl.Type);
                        return string.Empty;
        }

    }
}
