using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;



namespace Akshar.Lib
{
    public class JS
    {

        public static string  CreateObject(SqlDataReader sdr, string objectName, bool camelCase)
        {
            if (sdr == null || !sdr.HasRows) return string.Empty;

            System.Text.StringBuilder js = new System.Text.StringBuilder("var " + objectName + " = [");
            string[] names = JSOMembers(sdr, camelCase);

            while (sdr.Read())
            {
                js.Append("\n{");
                for (int i = 0; i < sdr.FieldCount; i++)
                {
                    // Response.Write(sdr.GetFieldType(i).Name.ToLower() + "\n");
                    switch (sdr.GetFieldType(i).Name.ToLower())
                    {
                        case "int":
                        case "byte":
                        case "long":
                        case "short":
                        case "decimal":
                        case "double":
                            js.Append(names[i] + ": " + sdr[i].ToString());
                            break;
                        case "boolean":
                            js.Append(names[i] + ": " + sdr.GetBoolean(i).ToString().ToLower());
                            break;
                        default:
                            js.Append(names[i] + ": '" + sdr[i].ToString() + "'");
                            break;
                    }
                    if ((i + 1) < sdr.FieldCount)
                        js.Append(',');
                }
                js.Append("},");
            }
            js[js.Length - 1] = ']';
            js.Append(';');
            return js.ToString();
        }

        private static string[] JSOMembers(SqlDataReader sdr, bool camelCase)
        {
            string[] names = new string[sdr.FieldCount];
            for (int i = 0; i < sdr.FieldCount; i++)
            {
                names[i] = (camelCase ? sdr.GetName(i).ToCamelCase() : sdr.GetName(i));
            }
            return names;
                    }


    }
}
