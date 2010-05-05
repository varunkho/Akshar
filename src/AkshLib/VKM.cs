using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Akshar.Lib.Data;

namespace Akshar.Lib
{

    public enum VKLTypes { Key = 1, Character = 2};

    public enum VKLVisibility { Personal = 0, Public, Default };

    public  class VKM : LibBase
    {

        public static Results CreateVKL(VKL vkl)
        {
            try
            {
if (VKLExists(vkl.Name, vkl.Type))
{
Common.LogError("Atempt to create a vkl that already exists.", "Vkm.CreateVKL", (int)Results.TamperingAttempt);
return Results.AlreadyExists;
}

using (DataAccess da = new DataAccess  ())
{
    vkl.Id = Convert.ToInt32(da.ExecuteSPForScalar("New_VKL", new SqlParameter[] { new SqlParameter("@langCode", vkl.LangCode), new SqlParameter("@name", vkl.Name), new SqlParameter("@userId", vkl.UserId), new SqlParameter("@type", (byte)vkl.Type), new SqlParameter("@visibility", (byte)vkl.Visibility) }));
}
vkl.Data.Save(VKLFilePath(vkl.Name, vkl.Type));
return Results.Ok;
            }
            catch (Exception ex)
            {
                Common.LogError("Error creating VKL: " + ex.Message, "VKM.CreateVKL", (int)Results.ResourceError);
            }
return Results.SomeError;
}

/// <summary>
/// Only saves the Data field (other details are read-only once a VKL created). Creates if VKL does not exist already.
/// </summary>
/// <param name="vkl"></param>
public static Results SaveVKL(VKL vkl)
{
try 
	{	        
if (vkl.Id <= 0) // Is it New VKL?
{
return CreateVKL(vkl);
}

		vkl.Data.Save(VKLFilePath(vkl.Name, vkl.Type));
return Results.Ok;
	}
	catch (Exception ex)
	{
        Common.LogError("Error saving VKL: " + ex.Message, "VKM.SaveVKL", (int)Results.ResourceError);
			}
return Results.SomeError;
}

public static XElement LoadVKLData(string name, VKLTypes type)
{
    try
    {
return XElement.Load(VKLFilePath(name, type));
    }
    catch (Exception ex)
    {
        Common.LogError("Error loading VKL: " + ex.Message, "VKM.LoadVKL", (int)Results.ResourceError);
        throw;
    }
return null;
}

public static string LoadRawVKLData(string name, VKLTypes type)
{
    try
    {
        return File.ReadAllText(VKLFilePath(name, type));
    }
    catch (Exception ex)
    {
Common.LogError("Error loading VKL: " + ex.Message, "VKM.LoadRawVKL", (int)Results.ResourceError);
    }
    return string.Empty;
}

public static XElement LoadTemplate(VKLTypes type)
{
Common.PrevActResult = Results.Ok;
    try
    {
return XElement.Load(Common.PhysicalPath((type == VKLTypes.Key ? IOLocations.VKLKTemplate : IOLocations.VKLCTemplate)));
    }
    catch (Exception ex)
    {
        Common.LogError("Error loading template '" + type.ToString() + "': " + ex.Message, "VKM.LoadTemplate", (int)Results.ResourceError);
    }
Common.PrevActResult = Results.SomeError;
return null;
}

        public static bool VKLExists(string VKLName, VKLTypes VKLType)
        {
            if (File.Exists(VKLFilePath(VKLName, VKLType)))
                return true;
            else
                return false;
        }

        private static string VKLFilePath(string Name, VKLTypes type)
        {
            return Common.PhysicalPath(IOLocations.VKLs + (type == VKLTypes.Key ? Name : Name + "_c") + ".vkl");
        }

        public static bool InscriptExists(string inscript)
        {
            string ucPath = Common.PhysicalPath(IOLocations.UCodes + inscript + ".htm");
            if (File.Exists(ucPath))
return true;
else
return false;
        }

        public static string InscriptEntries(string inscript)
        {
            string ucPath = Common.PhysicalPath(IOLocations.UCodes + inscript + ".htm");
            if (File.Exists(ucPath))
                return File.ReadAllText(ucPath);
            else
                return string.Empty;
        }

public static   VKL GetVKL(int vklId)
{
try {
using ( DataAccess da = new DataAccess())
{
var dt =  da.ExecuteQuerySPForDataTable("Sel_VKL", new SqlParameter[] {new SqlParameter("@id", vklId)});
if (dt.Rows.Count > 0)
{
var dr = dt.Rows[0];
var vkl =  new VKL {Id = (int)dr["VKLId"], LangCode = (string)dr["LangCode"], 
Name=(string)dr["Name"], UserId=(int)dr["UserId"], 
Type=(VKLTypes) int.Parse(dr["Type"].ToString()),
 Visibility = (VKLVisibility)int.Parse(dr["Visibility"].ToString())} ;

// vkl.Data = XElement.Load(VKLFilePath(vkl.Name, vkl.Type));
return vkl;
} else
Common.LogError("The VKL with id " + vklId + " couldn't be found.", Common.GetMethodName(MethodBase.GetCurrentMethod()), (int)Results.NotFound);
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
return null;
}

public static VKL GetVKL(string vklName, VKLTypes type)
{
try {
    using (DataAccess da = new DataAccess())
    {
        var dt = da.ExecuteQuerySPForDataTable("Sel_VKL", new SqlParameter[] { new SqlParameter("@name", vklName), new SqlParameter("@type", (int)type) });
        if (dt.Rows.Count > 0)
        {
            var dr = dt.Rows[0];
            var vkl = new VKL
            {
                Id = (int)dr["VKLId"],
                LangCode = (string)dr["LangCode"],
                Name = (string)dr["Name"],
                UserId = (int)dr["UserId"],
                Type = (VKLTypes)int.Parse(dr["Type"].ToString()),
                Visibility = (VKLVisibility)int.Parse(dr["Visibility"].ToString())
            };

            vkl.Data = XElement.Load(VKLFilePath(vkl.Name, vkl.Type));
            return vkl;
        }
        else
            Common.LogError("The VKL " + vklName + " (" + type.ToString() + ") couldn't be found.", Common.GetMethodName(MethodBase.GetCurrentMethod()), (int)Results.NotFound);
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
    return null;
}

    }

[Serializable]
public class VKL
{

public int Id { get; set; }
public string LangCode { get; set; }
public string Name { get; set; }
public int UserId { get; set; }
public VKLTypes Type { get; set; }
public VKLVisibility Visibility { get; set; }

[NonSerialized]
private XElement _data;
public XElement Data
{ get { return _data; } set { _data = value; }}

}

}
