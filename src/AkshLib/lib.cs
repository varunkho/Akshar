using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Web.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

namespace Akshar.Lib
{

    #region Enums
    public enum Activities : byte
    {
        All = 1,
        Viewing,
        Creating,
        Created,
        Editing,
        Edited,
        Replying,
        Replied,
        Deleting,
        Deleted
    }

    public enum NWObjects : byte
    {
        All = 1,
        Profile,
        Forum,
        Topic,
        Post,
        Language
    }

    public enum UserSettings
    {
        FPStyle = 1,
        // Forum 
        TPStyle,
        // Topic 
        PPStyle,
        // Post 
        DontSavePS
    }

    public enum ResourceStatus : byte
    {
        Unavailable = 1,
        Initiated,
        Done,
        Beta,
        Approved,
        Reinitiated,
        Redone
    }

    public enum URoles : byte
    {
        SupremeAdministrator = 1,
        Member,
        Admin,
        Moderator,
        Anonymous,
        Privilaged,
        Denied
    }

    public enum ContentPresentationStyles : byte
    {
        Tabular = 1,
        SelectionList
    }

    public enum Priorities
    {
        Low = 1,
        Medium,
        High,
        Severe
    }

    public enum Results
    {
        Yes = 1,
        No = 0,
        Ok = 1,
        SomeError = -1,
        NotAvailable = -2,
AlreadyExists = -2,
        DBError = -3,
        TamperingAttempt = -9999,
        InvalidUserName = -10000,
        InvalidPassword = -10001,
        InvalidPath = -10002,
        NotFound = -10003,
        InvalidAction = -10004,
        ResourceError = -10005,
        IncomingDataFalt = -10006
    }
    #endregion

    #region Structures
    public struct IOLocations
    {
        #region "Pages"
        public const string Error_Page = "~/GenericError.aspx";
        public const string Profile_Page = "~/profile.aspx";
        public const string Category_Page = "~/category.aspx";
        public const string Forum_Page = "~/forum.aspx";
        public const string Default_Page = "~/Default.aspx";
        #endregion

        #region "Dirs"
        internal const string AppData = "~/App_Data/";
        internal const string LangResources = AppData + "res/";
        internal const string HTRules = AppData + "htrules/";
        internal const string Mems = AppData + "members/";
        internal const string Administration = AppData + "adm/";
        public const string VKLs = AppData + "vkls/";
        public const string UCodes = AppData + "ucodes/";
                #endregion

        #region "Data Files"
        internal const string Log = Administration + "errdetails.log";
        internal const string Feedback = Administration + "feedback.log";
        internal const string Act = AppData + "act.nwxd";
        internal const string USettingTemplate = Mems + "settings.nwxd";
        internal const string VKLKTemplate = VKLs + "key.vklt";
        internal const string VKLCTemplate = VKLs + "char.vklt";
        #endregion
    }
    #endregion

    public class LibBase
    {

        public static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }

    }

    public static class Common
    {

        #region fields
        public static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
                            }
        }

        public static Results PrevActResult;
        #endregion

        #region Constants
        public const int LangGeneric = 1;
        public const int ForAll = 1;
        internal const string ResKey = "ProcessResult";
        internal const string ErrKey = "ErrNum";

internal const string  S_UID = "UserId";
        public const string C_ErrorHandled = "errorHandled";
        #endregion

        #region Enums
        public enum PickOptions
        {
            Forward,
            Reverse,
            Left,
            Right
        }
        #endregion

        #region Logs
        public static void LogError(string describtion, string Location)
        {
            LogError(describtion, Location, "", 0);
        }

        public static void LogError(string describtion, string Location, int ErrNumber)
        {
            LogError(describtion, Location, "", ErrNumber);
        }

        public static void LogError(string describtion, string Location, string CustomMsg)
        {
            LogError(describtion, Location, CustomMsg, 0);
        }

        public static void LogError(string describtion, string Location, Priorities Priority)
        {
        }

        public static void LogError(string describtion, string Location, string CustomMsg, int ErrNumber)
        {
            int UId = 0;
            string IP = String.Empty;
            if ((Context == null))
            {
                return;
            }
            IP = Context.Request.UserHostAddress;
            if (!(Context.Session[S_UID] == null))
            {
                UId = (int)Context.Session[S_UID];
            }

            // StringBuilder text = new StringBuilder(200);
            //text.Append("<log describtion=\"" + describtion + "\" ");
            //text.Append(" location=\"" + Location + "\" ");
            //text.Append(" customMsg=\"" + CustomMsg + "\" ");
            //text.Append(" errNumber=\"" + ErrNumber + "\" ");
            //text.Append(" on=\"" + DateTime.UtcNow.ToString() + "\" ");
            //text.Append(" ip=\"" + IP + "\" ");
            //text.Append(" userId=\"" + UId + "\" ");
            //// Add more above.
            //text.Append(" />");

            XElement xel = new XElement("log",
    new XAttribute("description", describtion),
new XAttribute("location", Location),
new XAttribute("customMsg", CustomMsg),
new XAttribute("errNumber", ErrNumber),
new XAttribute("on", DateTime.UtcNow.ToString()),
new XAttribute("ip", IP),
new XAttribute("userId", UId)
);

            StreamWriter logger;
            try
            {
                logger = new StreamWriter(PhysicalPath(IOLocations.Log), true);
                logger.WriteLine(xel.ToString(SaveOptions.DisableFormatting));
                logger.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Feedback
        public static bool Feedback(string type, string subject, string describtion, string name, string email)
        {
            int UId = 0;
            string IP = String.Empty;

            IP = Context.Request.UserHostAddress;
            if (!(Context.Session[S_UID] == null))
            {
                UId = (int)Context.Session[S_UID];
            }

           XElement xel = new XElement("feedback",
                   new XAttribute("type", type),
               new XAttribute("subject", subject),
    new XAttribute("description", describtion),
    new XAttribute("name", name),
    new XAttribute("email", email),
new XAttribute("on", DateTime.UtcNow.ToString()),
new XAttribute("ip", IP),
new XAttribute("userId", UId)
);

            StreamWriter logger;
            try
            {
                logger = new StreamWriter(PhysicalPath(IOLocations.Feedback), true);
                logger.WriteLine(xel.ToString(SaveOptions.DisableFormatting));
                logger.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogError("Error logging user feedback {" + ex.Message + "}", "Common.Feedback", (int)Results.ResourceError);
            }
            return false;
        }
        #endregion

        #region Extensibility
        public static string strPick(string strData, string strDelimiter)
        {
            return strPick(strData, strDelimiter, PickOptions.Left, PickOptions.Reverse, 1, true);
        }

        public static string strPick(string strData, string strDelimiter, PickOptions pickBegins)
        {
            return strPick(strData, strDelimiter, pickBegins, PickOptions.Reverse, 1, true);
        }

        public static string strPick(string strData, string strDelimiter, PickOptions pickBegins, PickOptions pickDirection)
        {
            return strPick(strData, strDelimiter, pickBegins, pickDirection, 1, true);
        }

        public static string strPick(string strData, string strDelimiter, PickOptions pickBegins, PickOptions pickDirection, int DelimiterInstance)
        {
            return strPick(strData, strDelimiter, pickBegins, pickDirection, DelimiterInstance, true);
        }

        public static string strPick(string strData, string strDelimiter, PickOptions pickBegins, PickOptions pickDirection, int DelimiterInstance, bool includeFollowers)
        {
            if (!strData.Contains(strDelimiter)) return string.Empty;
            string[] tmStr = strData.Split(new string[] { strDelimiter }, StringSplitOptions.None);
            if (DelimiterInstance <= 0 || DelimiterInstance >= tmStr.Length) return string.Empty;

            string strReturn = "";
            int LIndex = 0;
            int UIndex = 0;

            if (pickBegins == PickOptions.Left && pickDirection == PickOptions.Reverse)
            {
                UIndex = DelimiterInstance - 1;
                LIndex = (int)(!(includeFollowers) ? UIndex : 0);
            }
            else if (pickBegins == PickOptions.Right && pickDirection == PickOptions.Reverse)
            {
                LIndex = (tmStr.Length - DelimiterInstance);
                UIndex = (int)(!(includeFollowers) ? LIndex : tmStr.Length - 1);
            }
            else if (pickBegins == PickOptions.Left && pickDirection == PickOptions.Forward)
            {
                LIndex = DelimiterInstance;
                UIndex = (int)(!(includeFollowers) ? LIndex : tmStr.Length - 1);
            }
            else
            {
                UIndex = (tmStr.Length - DelimiterInstance) - 1;
                LIndex = (int)(!(includeFollowers) ? UIndex : 0);
            }

            for (int i = LIndex; i <= UIndex; i++)
            {
                strReturn += tmStr[i] + strDelimiter;
            }

            return strReturn.Remove(strReturn.Length - 1);
        }

        public static bool ItemExists(DropDownList DDList, string Item)
        {
            return ItemExists(DDList, Item, false);
        }

        public static bool ItemExists(DropDownList DDList, string Item, bool CaseSensitive)
        {
            foreach (ListItem DDItem in DDList.Items)
            {
                if (CaseSensitive)
                {
                    if (DDItem.Text == Item) return true;
                }
                else
                {
                    if (DDItem.Text.ToLower() == Item.ToLower()) return true;
                }
            }
            return false;
        }
        #endregion

        #region IO Methods
        public static string PhysicalPath(string VirtualPath)
        {
            if ((Context == null))
            {
                return String.Empty;
            }
            try
            {
                return Context.Server.MapPath(VirtualPath);
            }
            catch (System.Exception)
            {
                return String.Empty;
            }
        }

        public static Dictionary<string, object> ReadXMLRecord(string xFile, string ByKey, object WithValue)
        {
            return ReadXMLRecord(xFile, ByKey, WithValue, true);
        }

        public static Dictionary<string, object> ReadXMLRecord(string xFile, string ByKey, object WithValue, bool logExp)
        {
            Dictionary<string, object> res = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            res.Add(ResKey, false);
            res.Add(ErrKey, 0);
            try
            {
                XElement xTable = XElement.Load(xFile);
                try
                {
                    var RequestedRow = (from row in xTable.Descendants() where row.Attribute(ByKey).Value == WithValue.ToString() select row);
                    switch (RequestedRow.Count())
                    {
                        case 0:
                            res[ErrKey] = 2;
                            return res;
                        case 1:
                            res[ErrKey] = 3;
                            break;
                        //  Case Else
                        //  res.Remove(ErrKey)
                    }

                    foreach (var attr in RequestedRow.First().Attributes())
                    {
                        res.Add(attr.Name.LocalName, attr.Value);
                    }
                    res[ResKey] = true;
                }
                catch (Exception ex)
                {
                    if (logExp)
                    {
                        LogError(ex.Message, "xml.read", string.Format("{0}|{1}|{2}", xFile, ByKey, WithValue));
                    }
                    res[ErrKey] = 1;
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "xml.read", xFile);
            }
            return res;
        }
        #endregion

        #region Web Methods
        public static string ClientIP()
        {
            if ((Context != null))
            {
                return Context.Request.UserHostAddress;
            }
            return string.Empty;
        }
        #endregion

        #region Reflection Methods
        public static string GetMethodName(string StackTrace)
        {
            int InPos = 0;
            int AtPos = 0;
            try
            {
                foreach (string Str in StackTrace.Split('\n'))
                {
                    AtPos = Str.IndexOf(" at ");
                    InPos = Str.IndexOf(" in ");

                    if (AtPos >= 0 && InPos > 0)
                    {
                        return strPick(strPick(Str.Substring(AtPos), "("), ".", PickOptions.Right, PickOptions.Reverse, 2, true);
                    }
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        public static string GetMethodName(MethodBase MB)
        {
            try
            {
                return MB.DeclaringType.Name + "." + MB.Name;
            }
            catch
            {
            }
            return string.Empty;
        }
        #endregion

        public static string ConnectionString
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings["AkshConnectionString"].ConnectionString;
            }
        }

    }





    public static class NWExtensions
    {
        public static string ToCamelCase(this string s)
        {
            char[] cs = s.ToCharArray();
            if (s[0] >= 65 && s[0] <= 90)
            {
                cs[0] = char.ToLower(cs[0]);
                return new string(cs);
            }
            else return s;
        }

public static string ToUnicodeValue(this string s, string delimiter)
{
if (string.IsNullOrEmpty(s)) return s;

// Optimized by allocating at once the required size.
StringBuilder res = new StringBuilder((s.Length * 4) + (s.Length *delimiter.Length -delimiter.Length));
foreach (char c in s.ToCharArray())
res.Append(((int)c) + delimiter);

res.Length = res.Length - delimiter.Length;
return res.ToString();
}

public static bool ToBoolean(this string s)
{
try
{
    bool b = false;
if (!string.IsNullOrEmpty(s) && (s.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase) || s.Equals("on", StringComparison.InvariantCultureIgnoreCase) || Convert.ToInt32(s) != 0))
return true;
else
return false;
}
catch (Exception)
{
return false; 
}
}

    }

public static class Configuration
{

public static readonly bool Debugging = true;

}
}
