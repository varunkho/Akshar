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
using System.IO;
using System.Text;


namespace Akshar
{
    public partial class Akshar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
if (Member.SessionBegun)
{
var set = Member.Settings;
StringBuilder jSettings = new StringBuilder(set.VKLs.Length * 20 + 40);
jSettings.Append("var settings={vkls:[");
if (set.VKLs.Length > 0)
{
foreach (var v in set.VKLs)
{
jSettings.Append("{name:'" + v.Key + "',type:'" + v.Value + "'},");
}
jSettings.Length -= 1;
}
jSettings.Append("]}");
ClientScript.RegisterClientScriptBlock(this.GetType(), "settings", jSettings.ToString(), true);
}
        }


#region Web Methods
[System.Web.Services.WebMethod]
        public static string GetVKL(string langCode, string vklName, int vklType)
        {
            if (string.IsNullOrEmpty(langCode) && string.IsNullOrEmpty(vklName) || !Enum.IsDefined(typeof(VKLTypes), vklType)) return string.Empty;

            var vkl = VKM.GetVKL(vklName, (VKLTypes)vklType);
            if (vkl != null && ((vkl.Visibility == VKLVisibility.Default || vkl.Visibility == VKLVisibility.Public) || (Member.SessionBegun && (Member.Roles.IsSupremeAdministrator || vkl.UserId == Member.CurrentUser))))
                return VKM.LoadRawVKLData(vkl.Name, vkl.Type);

            return string.Empty;
        }

[System.Web.Services.WebMethod]
public static string GetDocsListings()
{
if (!Member.SessionBegun)
    return "{result:'failure',msg:'<span>Forbidden (you must first <a href=\"signin.aspx\" target=\"_blank\">Sign in</a>.)</span>'}";

string docs = Member.GetUserDocsDir;
if (!Directory.Exists(docs))
    return "{result:'failure',msg:'<span>You are not having personal documents directory.</span>'}";

return "{result:'success',o:" + EnumDir(new DirectoryInfo(docs), true) + "}";
}

private static string EnumDir(DirectoryInfo di, bool isRoot)
{
DirectoryInfo[] subDirs = di.GetDirectories();
FileInfo[] files = di.GetFiles("*.aks");
StringBuilder json = new StringBuilder(subDirs.Length * 10 + files.Length * 15);
if (isRoot)
json.Append("{name:'Root',isRoot:true,");
else
    json.Append("{name:'" + di.Name + "',");

json.Append("dirs:[");
for (int i = 0;i<subDirs.Length;i++)
{
json.Append(EnumDir(subDirs[i], false));
if (i < (subDirs.Length - 1))
json.Append(",");
}
json.Append("],files:[");
for (int i = 0;i<files.Length;i++)
{
json.Append("{name:'" + Path.GetFileNameWithoutExtension(files[i].Name) + "'}");
if (i < (files.Length - 1))
json.Append(",");
}
json.Append("]}");
return json.ToString();
}

[System.Web.Services.WebMethod]
public static string ExecCommand(string cmd, bool IncludeListingOnError)
{
    if (!Member.SessionBegun)
        return "{result:'failure',msg:'<span>Forbidden (you must first <a href=\"signin.aspx\" target=\"_blank\">Sign in</a>.)</span>'}";

    if (cmd.IndexOf("..") >= 0)
    {
        Common.LogError("Potentially dangerous command {" + cmd + "}", "akshar.ExecCommand", (int)Results.TamperingAttempt);
        return "{result:'failure',msg:'<span>Command is in invalid format.</span>'}";
    }

    string docs = Member.GetUserDocsDir;
    if (!Directory.Exists(docs))
        return "{result:'failure',msg:'<span>You are not having personal documents directory.</span>'}";

    try
    {
        foreach (string c in cmd.Split(new char[] { '|' }))
        {
            if (string.IsNullOrEmpty(c) || c.Trim() == string.Empty)
                continue;
            string[] pars = c.Split(new char[] { ':' });
            switch (pars[0])
            {
                case "delf":
                    File.Delete(docs + @"\" + pars[1] + ".aks");
                    break;
                case "deld":
                    Directory.Delete(docs + @"\" + pars[1], true);
                    break;
                case "newd":
                    Directory.CreateDirectory(docs + @"\" + pars[1]);
                    break;
            }
        }
        return "{result:'success'}";
    }
    catch (Exception ex)
    {
        Common.LogError("error executing commands {" + cmd + "}", "Akshar.ExecCommand");
    }
    if (IncludeListingOnError)
        return "{result:'failure',msg:'<span>Error occured while processing specified commands.</span>',o:" + EnumDir(new DirectoryInfo(docs), true) + "}";
    else
        return "{result:'failure',msg:'<span>Error occured while processing specified commands.</span>'}";
}

[System.Web.Services.WebMethod]
public static string GetDoc(string path)
{
    if (!Member.SessionBegun)
        return "{result:'failure',msg:'Forbidden (you must first sign in.)'}";

    if (path.IndexOf("..") >= 0)
    {
        Common.LogError("Potentially dangerous command {" + path + "}", "akshar.GetDoc", (int)Results.TamperingAttempt);
        return "{result:'failure',msg:'Command is in invalid format.'}";
    }

    string docs = Member.GetUserDocsDir;
    // We employ additional security check here.
    // Only files with the extension .aks can be requested (the same is appended transparently when a file is saved).
    string file = docs + path + ".aks";
    if (File.Exists(file))
    {
        return "{result:'success',text:'" + File.ReadAllText(file) + "'}";
    }
    else
        return "{result:'failure',msg:'File not found.'}";
}

[System.Web.Services.WebMethod]
public static string SaveDoc(string path, string text)
{
    if (!Member.SessionBegun)
        return "{result:'failure',msg:'Forbidden (you must first sign in.)'}";

    if (path.IndexOf("..") >= 0)
    {
        Common.LogError("Potentially dangerous command {" + path + "}", "akshar.SaveDoc", (int)Results.TamperingAttempt);
        return "{result:'failure',msg:'Command is in invalid format.'}";
    }

    string docs = Member.GetUserDocsDir;
    File.WriteAllText(docs + path + ".aks", text, Encoding.UTF8);
        return "{result:'success'}";
}
#endregion

    }
}
