using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using Akshar.Lib;

namespace Akshar
{
    /// <summary>
    /// Summary description for VKMServe
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class VKMServe : System.Web.Services.WebService
    {

        [WebMethod(true)]
public string GetVKL(string vklName, string vklType)
{
var type =(int) ((vklType = vklType.ToUpper()) == "CHARACTER" ? (int)VKLTypes.Character : vklType == "KEY" ? (int) VKLTypes.Key : -1);
if (string.IsNullOrEmpty(vklName) || type == -1) return string.Empty;

var vkl = VKM.GetVKL(vklName,(VKLTypes)type);
if (vkl != null && ((vkl.Visibility == VKLVisibility.Default || vkl.Visibility == VKLVisibility.Public) || (Member.SessionBegun && (Member.Roles.IsSupremeAdministrator || vkl.UserId == Member.CurrentUser))))
return VKM.LoadRawVKLData(vkl.Name, vkl.Type);

return string.Empty;
}

    }
}
