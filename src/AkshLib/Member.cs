using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using Akshar.Lib.Data;
using System.IO;
using System.Xml.Linq;

namespace Akshar.Lib
{


    public  class Member : LibBase
    {

        public const int From_Year = 0x76c;
        public const int To_Year = 0x7d0;
public const string S_USETTINGS = "UserSettings";

#region Props
        public static bool SessionBegun
        {
            get
            {
            if (Context.User.Identity.IsAuthenticated) {
                return (Context.Session[Common.S_UID] != null);
                }
                return false;
            }
        }

        public static int CurrentUser
        {
            get
            {
                try
                {
                    return (int)Context.Session[Common.S_UID];
                }
                catch (Exception e)
                {
                    Common.LogError(e.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()));
                    throw;
                }
            }
        }

public static string GetUserDocsDir
{
get {
return Common.PhysicalPath(IOLocations.Mems + Member.CurrentUser + "/docs");
}
}

public static Settings Settings
{
get
{
return (Context.Session[S_USETTINGS] as Settings);
}
}
#endregion

        public static Results BeginSession()
        {
            return BeginSession(0);
        }

        public static Results BeginSession(int userId)
        {
            try
            {
                                if (userId <= 0)
                {
                    userId = Member.UserId(Context.User.Identity.Name);
                    if (userId <= -1)
                    {
                        return Results.SomeError;
                    }
                }
            Context.Session[Common.S_UID] = userId;
                Context.Session[S_USETTINGS] = Settings.Load(userId);
                // MyUserRoles.Load(userId);
                return (Results)userId;
            }
            catch (Exception e)
            {
                Common.LogError(e.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()));
            }
            return Results.SomeError;
        }

public static void EndSession() {
Context.Session[Common.S_UID] = null;
// Context.Session[S_USETTINGS] = null;
}

        public static int Create(string UserName, string Password, string Email, string FullName)
        {
            try
            {
                DataStoreDataContext context = new DataStoreDataContext(Common.ConnectionString);
                WN_Member entity = new WN_Member
                {
                    UserName = UserName,
                    Password = FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5"),
                    Email = Email,
                    FullName = FullName,
                    CreatedON = DateTime.UtcNow,
                    FromIP = Common.ClientIP()
                };
                context.WN_Members.InsertOnSubmit(entity);
                context.SubmitChanges();
string myDir = Common.PhysicalPath(IOLocations.Mems + entity.UserId);
Directory.CreateDirectory(myDir).CreateSubdirectory("docs");
File.Copy(Common.PhysicalPath(IOLocations.USettingTemplate), myDir + @"\settings.nwxd");
                return entity.UserId;
            }
            catch (SqlException sqlEx)
            {
                Common.LogError(sqlEx.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()), sqlEx.Number);
            }
            catch (Exception otherEx)
            {
                Common.LogError(otherEx.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()));
            }
            return -1;
        }

        // The method is for diagnosing purposes.
        public static int UserId(string UserName)
        {
            int uId = Exists(UserName);
            if (uId == 0) // Serious error; somebody might have compromised the system.
            {
                Common.LogError("Unauthorized access {user name does not exist}", "Member.UserId", (int)Results.TamperingAttempt);
                return -1;
            }
            else
                return uId;
        }

        public static int Exists(string UserName)
        {
            try
            {
                using (DataAccess da = new DataAccess())
                {
                    int uId = Convert.ToInt32(da.ExecuteSPForScalar("UserExists", new SqlParameter[] { new SqlParameter { DbType = DbType.String, ParameterName = "@userName", Direction = ParameterDirection.Input, Value = UserName } }));
                    if (uId > 0)
                        return uId;
                    else
                        return 0;
                }
            }
            catch (SqlException sqlEx)
            {
                Common.LogError(sqlEx.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()), sqlEx.Number);
            }
            catch (Exception otherEx)
            {
                Common.LogError(otherEx.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()));
            }
            return (int)Results.SomeError;
        }

        public static int Validate(string UserName, string Password, ref string DBUserName)
    {
      try
      {
                    using (DataAccess da = new DataAccess())
          {
              DataTable dt =  da.ExecuteQuerySPForDataTable("Brief_UserInfo", new SqlParameter[] {new SqlParameter {DbType = DbType.String, ParameterName = "@userName", Direction = ParameterDirection.Input, Value = UserName}});
        if (dt.Rows.Count == 0)
        {
          Common.LogError("Invalid UserName '" + UserName + "'", "Member.Validate",(int) Results.InvalidUserName);
          return (int) Results.InvalidUserName;
        }
        if ((string)dt.Rows[0]["Password"]!= FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5"))
        {
          Common.LogError("Invalid PWD'" + Password + "'", "Member.Validate", (int) Results.InvalidPassword);
          return (int) Results.InvalidPassword;
        }

        DBUserName =dt.Rows[0]["UserName"].ToString();
        return (int)dt.Rows[0]["UserId"];
      }
      }
      catch (SqlException sqlEx)
      {
        Common.LogError(sqlEx.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()), sqlEx.Number);
      }
      catch (Exception otherEx)
      {
        Common.LogError(otherEx.Message, Common.GetMethodName(MethodBase.GetCurrentMethod()));
      }
      return (int) Results.SomeError;
    }



        public  class Roles : LibBase
        {

            public static bool IsSupremeAdministrator
            {
get
{
    if (Context.User.Identity.Name.ToLower() == "varun" || Context.User.Identity.Name.ToLower() == "tarun")
        return true;
    else
        return false;
            }
}

        }

    }

[Serializable]
public class Settings : LibBase
{

private bool _Anonymous, _dirty;


#region props
private Dictionary<string, KeyValuePair<string,string> > _vkls = new Dictionary<string,KeyValuePair<string,string>>();
public KeyValuePair<string,string>[] VKLs
{
get
{
return _vkls.Values.ToArray();
}
}
#endregion

#region Construction
// Must call Load to get an instance
private Settings() { this._Anonymous = true; }

private Settings(XElement xel)
{
foreach (var x in xel.Element("selectedVkls").Elements())
{
string name = x.Attribute("name").Value, type = x.Attribute("type").Value, key = name.ToLower() + (type.ToLower() == "character" ? "_c" : string.Empty);
if (!_vkls.ContainsKey(key))
_vkls.Add(key, new KeyValuePair<string,string>(name, type));
}
}

public static Settings Load()
{
return Load(0);
}

public static Settings Load(int userId)
{
Settings set  = null;
if (userId <= 0)
{
set = new Settings();
} else {
    try
    {
XElement xel = XElement.Load(Common.PhysicalPath(IOLocations.Mems + Member.CurrentUser + "/settings.nwxd"));
set = new Settings(xel);
    }
    catch (Exception ex)
    {
Common.LogError("Unable to lload user settings {" + ex.Message + "}", "Settings.Load", ex.StackTrace,(int) Results.ResourceError);
set = new Settings();
    }
}
return set;
}
#endregion

#region Methods
public void save()
{
if (_Anonymous || !_dirty)
return;

string path = Common.PhysicalPath(IOLocations.Mems + Member.CurrentUser + "/settings.nwxd");
XElement xel = XElement.Load(path);
var  vSec = xel.Element("selectedVkls");
vSec.RemoveAll();
foreach (var v in this._vkls.Values)
vSec.Add(new XElement("vkl", new XAttribute("name", v.Key), new XAttribute("type", v.Value)));

xel.Save(path);
}

public void AddVKL(string name, string type)
{
    string key = name.ToLower() + (type.ToLower() == "character" ? "_c" : string.Empty);
if (!_vkls.ContainsKey(key))
{
this._vkls.Add(key, new KeyValuePair<string,string>(name, type));
this._dirty = true;
}
}

public void ClearVKL()
{
this._vkls.Clear();
}
#endregion


}



        }
