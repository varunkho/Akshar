using System;
using System.Collections;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using Akshar.Lib;
using Akshar.Lib.Data;

namespace Akshar
{
    public partial class keyboardProps : System.Web.UI.Page
    {

        private const string S_RID = "RVKLID";
private const string V_ACTIVITY = "CurActivity";
private string V_VKL = "CurVKL";

#region Props
protected Activities Act
{
get
{
if (Convert.ToInt32(ViewState[V_ACTIVITY]) ==(int) Activities.Editing)
return Activities.Editing;
else
return Activities.Creating;
}
set {
ViewState[V_ACTIVITY] = value;
}
}

protected VKLTypes VKLType
{
get
{
if (Character.Checked)
return VKLTypes.Character;
else
return VKLTypes.Key;
}
}

protected int EditingVKLId {
get
{
return Convert.ToInt32(Session[S_RID]);
}
set
{
    Session[S_RID] = value;
}
}

protected VKL EditingVKL
{
get
{
return (ViewState[V_VKL] as VKL);
}
set
{
ViewState[V_VKL] = value;
}
}
#endregion

#region Page
        protected void Page_Load(object sender, EventArgs e)
        {
if (!Member.SessionBegun)
Response.Redirect("~/signin.aspx");

SqlDataReader sdr;
using (DataAccess da = new DataAccess())
{
        sdr = (SqlDataReader)da.ExecuteSPForDataReader("Sel_VKLs", new SqlParameter[] { Parameters.Int("@userId", Member.CurrentUser) });

    this.ClientScript.RegisterClientScriptBlock(GetType(), "VKLs", "<script>\n" + JS.CreateObject(sdr, "vkls", true) + "</script>", false);
    sdr.Close();
}

if (!IsPostBack)
{
int vid = 0;
if (Request.QueryString["vid"] != null && int.TryParse(Request.QueryString["vid"], out vid))
{
EditVKL(vid);
} else {
DataAccess da = new DataAccess();
vklLang.DataSource =  da.ExecuteSelectForDataTable("SELECT * FROM [Languages] ORDER BY [Language]").DefaultView;
vklLang.DataBind();
vklLang.Items.Insert(0, new ListItem {Value=string.Empty,Text="--Select--"});
PopulateInscripts();
Wizard.SetActiveView(step1);
Act = Activities.Creating;
}
}
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Wizard.ActiveViewChanged += new EventHandler(Wizard_ActiveViewChanged);

            this.MRows.ItemCreated += new RepeaterItemEventHandler(MRows_ItemCreated);
        }

        void MRows_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
        if (e.Item.DataItem == null)
        {
if (e.Item.ItemType == ListItemType.Header)
{
var r = (Repeater)sender;
var parentMap = (r.ID == "subMRows" ? ((Map[])r.DataSource)[0]._parent : ((Rule[])r.DataSource)[0]._parent);
e.Item.DataItem = parentMap;
}
return;
        }

if (((Repeater)sender).ID == "RRows")
return;

                Map m = (Map)e.Item.DataItem;
        int index;
        Literal l;
        HtmlImage a;
if (m.HasMaps)
{
a = (GetControlWithIndex(e.Item, "arSeq", out index) as HtmlImage);
if (a != null)
{
a.Alt = "Remove Sequence";
a.Attributes["title"] = a.Alt;
a.Src = "images/sd.gif";
a.Attributes.Add("tog", "true");

l = new Literal();
l.Text = lmexp.Text;
e.Item.Controls.AddAt(index +1, l);
 a =(HtmlImage) e.Item.FindControl("arRule");
a.Style.Add(HtmlTextWriterStyle.Display, "none");
}

var subMaps =  e.Item.FindControl("subMRows") as Repeater;
if (subMaps != null)
{
    subMaps.ItemCreated += new RepeaterItemEventHandler(MRows_ItemCreated);
subMaps.DataSource = m.Maps;
subMaps.DataBind();
}
} else if (m.HasRules)
{
    a = (HtmlImage)GetControlWithIndex(e.Item, "arRule", out index);
    a.Alt = "Remove Rule";
    a.Attributes["title"] = a.Alt;
    a.Src = "images/rd.gif";
    a.Attributes.Add("tog", "true");

    l = new Literal();
    l.Text = lrexp.Text;
    e.Item.Controls.AddAt(index + 1, l);

    a = e.Item.FindControl("arSeq") as HtmlImage;
    if (a != null)
    a.Style.Add(HtmlTextWriterStyle.Display, "none");

    var rules = (Repeater)e.Item.FindControl("RRows");
    rules.ItemCreated += new RepeaterItemEventHandler(MRows_ItemCreated);
    rules.DataSource = m.Rules;
    rules.DataBind();
}
        }

private Control GetControlWithIndex(Control parent, string childId, out int childIndex)
{
childIndex = -1;
for (int i =0;i<parent.Controls.Count;i++)
{
if (!string.IsNullOrEmpty(parent.Controls[i].ID) && string.Equals(parent.Controls[i].ID, childId, StringComparison.InvariantCultureIgnoreCase))
{
childIndex = i;
return parent.Controls[i];
}
}
return null;
}

        void Wizard_ActiveViewChanged(object sender, EventArgs e)
        {
            if (Wizard.ActiveViewIndex == 0)
                form1.DefaultButton = Next.ClientID;
            else
                form1.DefaultButton = string.Empty;
        }
#endregion

#region Lists
        protected void vklLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vklLang.AutoPostBack)
            { // The event should be processed only when show all option is unchecked, dynamically removing event is not working, so this helps to prevent processing during submit.
                PopulateInscripts();
            }
        }

        protected void ShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowAll.Checked)
            {
                vklLang.AutoPostBack = false;
                PopulateInscripts();
            }
            else
            {
                vklLang.AutoPostBack = true;
                PopulateInscripts();
            }
        }
#endregion

#region Buttons
        protected void Next_Click(object sender, EventArgs e)
        {
if (!Page.IsValid)
{
ErrorProvider.Text = "You must specify all the information within the constraint specified in order to proceed.";
return;
}
if (VKM.VKLExists(vklName.Text, VKLType))
{
ErrorProvider.Text = "A VKL with The same name already exists. Try some other name.";
return;
}

            Wizard.SetActiveView(step2);
LoadInscript(vklScript.SelectedItem.Text);
string vTmpl = Request.Form["vklTmpl"];
if (!string.IsNullOrEmpty(vTmpl) && VKM.VKLExists(vTmpl, VKLType))
{
try {
XElement xvkl = VKM.LoadVKLData(vTmpl, VKLType);
    var mappings = Map.Default();
    TranslateFromXMappings(xvkl.Element("mappings"), mappings);
    MRows.DataSource = mappings.Maps;
    MRows.DataBind();
    scounttr.Value = mappings.NextMapPos.ToString();
} catch (Exception ex) {
Common.LogError("Error creating vkl template {" + ex.Message + "}.", "Keyboard.Next", (int)Results.ResourceError);
ErrorProvider.Text += "  Error loading VKL Template, we are sorry for the inconvenience.";
}
}
        }

protected void Previous_Click(object sender, EventArgs e)
{
Wizard.SetActiveView(step1);
}

protected void done_Click(object sender, EventArgs e)
{
//foreach (var k in Request.Form.AllKeys)
//    if (!k.StartsWith("__"))
//        Response.Write(k + "=" + Request.Form[k]);
//return;

XElement XMappings;
if (!CreateMappings(out XMappings)) {
ErrorProvider.Text = "Errors have been encountered, please correct them and then try again.";
return;
}

VKL vkl;
if (Act == Activities.Creating)
{
vkl = new VKL {LangCode = vklLang.SelectedValue, Name = vklName.Text, Type = this.VKLType, UserId = Member.CurrentUser, Visibility = VKLVisibility.Personal};
     vkl.Data = VKM.LoadTemplate(this.VKLType);
    var Props = vkl.Data.Element("props");
    Props.Element("author").Value = User.Identity.Name;
    Props.Element("name").Value = vklName.Text;
    Props.Element("type").Value = VKLType.ToString();
    Props.Element("inscript").Value = vklScript.SelectedItem.Text;
    var lang = Props.Element("language");
    lang.SetAttributeValue("code", vklLang.SelectedValue);
    lang.Value = vklLang.SelectedItem.Text;
} else {
vkl = EditingVKL;
// Quick security check.
if (vkl == null || vkl.Id != EditingVKLId)
{
Common.LogError("Illegal editing atempt with svklId: " + EditingVKLId + " and vklid: " + (vkl == null ? "null" : vkl.Id.ToString()), "keyboard.done", (int)Results.TamperingAttempt);
ErrorProvider.Text = "You seem to be not authorized to perform this acction. If you think you do, you may try a little while after.";
return;
}

vkl.Data = VKM.LoadVKLData(vkl.Name, vkl.Type);
vkl.Data.Element("mappings").Remove();
}

vkl.Data.Add(XMappings);

if (VKM.SaveVKL(vkl) == Results.Ok)
{
Response.Redirect("~/account.aspx");
}
else
ErrorProvider.Text = "Some error occured while performing this action, please try again.";
}
#endregion

#region Methods
private bool CreateMappings(out XElement XMappings)
{
Map mappings = Map.Default(), curMap = mappings;
Rule curRule = null;
string dest = string.Empty;int pos;

 foreach (var key in Request.Form.AllKeys)
{
string[] hkey = key.ToUpper().Split(new char[] {'_'});

for (int i = 0;i<hkey.Length;i++)
{
if (!ParseKey(hkey[i], out dest, out pos))
goto End; // An invalid key at any point invalidates the meaning of further traversing.

switch (dest)
{
case "KEY": case "CHAR": case "CHKSHIFT":
curMap =  curMap.GetMap(pos);
break;
default:
curRule = curMap.GetRule(pos);
break;
}
}

string value = Request.Form[key];
if (curRule != null)
{
int val;
switch (dest)  
{
case "RTYPE": 
if (!int.TryParse(value, out val)) continue;
curRule.Type = (val == (int)RuleTypes.REGX ? RuleTypes.REGX : RuleTypes.TEXT);
break;
case "RDIR":
if (!int.TryParse(value, out val)) continue;
curRule.Direction = (val == (int)RuleDirections.LEFT ? RuleDirections.LEFT : RuleDirections.RIGHT);
break;  
case "RIF": 
curRule.Pattern = value;
break;
case "RTHEN":
curRule.Replacement = value;
break;
}
} else {
switch (dest)
{
    case "KEY":
    curMap.Key = value;
    break;
    case "CHAR":
curMap.Character = value;
break;
    case "CHKSHIFT":
curMap.Shift = true;
    break;
}
}

End:
curMap = mappings;
curRule = null;
}

 XMappings = new XElement("mappings");
if (TranslateToXMappings(mappings, XMappings))
return true;
else
{
MRows.DataSource = mappings.Maps;
MRows.DataBind();
return false;
}
}

private bool TranslateToXMappings(Map mapp, XElement parent)
{
XElement xel;
bool isValid = true;
foreach (Map m in mapp.Maps)
{
if (string.IsNullOrEmpty(m.Key) || (m.Maps.Length == 0 && string.IsNullOrEmpty(m.Character) ))
continue;
    xel = new XElement("map", new XAttribute("key", m.Key), new XAttribute("shift", m.Shift), new XAttribute("text",( string.IsNullOrEmpty(m.Character) ? string.Empty : m.Character) ));
parent.Add(xel);

if (m.Maps.Length > 0)
{
    xel.SetAttributeValue("text", string.Empty);
    isValid = TranslateToXMappings(m, xel);
}
else
{
int lmax = 0, rmax = 0;
    foreach (Rule r in m.Rules)
    {
        if (string.IsNullOrEmpty(r.Pattern) || string.IsNullOrEmpty(r.Replacement))
            continue;

Match test = null;
        if (r.Type == RuleTypes.TEXT)
        {
            if (r.Pattern.Length > 3)
            {
                r.PatternError = "Out of maximum Length";
                isValid = false;
                continue;
            }
        }
        else
        {
        test = Regex.Match(r.Pattern, Rule.ValidPattern);
        if (!test.Success)
        {
                r.PatternError = "Invalid Pattern";
                isValid = false;
                continue;
            }
        }

var xrule = new XElement("rule", new XAttribute("type", r.Type), new XAttribute("direction", r.Direction), new XAttribute("pattern", r.Pattern), new XAttribute("text", r.Replacement));
        xel.Add(xrule);

int inter = 0;
if (r.Direction == RuleDirections.LEFT)
{
lmax = (test != null ? (inter = test.Groups[1].Captures.Count + test.Groups[2].Captures.Count) > lmax ? inter : lmax : r.Pattern.Length > lmax ? r.Pattern.Length : lmax);
} else {
    rmax = (test != null ? (inter = test.Groups[1].Captures.Count + test.Groups[2].Captures.Count) > rmax ? inter : rmax : r.Pattern.Length > rmax ? r.Pattern.Length : rmax);
}
if (r.Type == RuleTypes.REGX)
xrule.SetAttributeValue("len", inter);
    } // foreach rule

    //if (xel.Elements().Any()) {
    //if (!string.IsNullOrEmpty(m.Character)) xel.SetAttributeValue("default", m.Character);
    //} else
    //xel.Value = m.Character;
if (xel.Elements().Any())
xel.Add(new XAttribute("rleftlen", lmax), new XAttribute("rrightlen", rmax));
}
}
return isValid;
}

private void TranslateFromXMappings(XElement xelem, Map parent)
{
    Map m;
    foreach (var xel in xelem.Elements())
    {
        if (xel.Name == "map")
        {
            m = parent.AddMap(xel.Attribute("key").Value, xel.Attribute("text").Value, bool.Parse(xel.Attribute("shift").Value));
            if (xel.Elements().Any())
                TranslateFromXMappings(xel, m);
        }
        else
        {
            parent.AddRule((RuleTypes)Enum.Parse(typeof(RuleTypes), xel.Attribute("type").Value), (RuleDirections)Enum.Parse(typeof(RuleDirections), xel.Attribute("direction").Value), xel.Attribute("pattern").Value, xel.Attribute("text").Value);
        }
    }
}

        private void EditVKL(int vid)
{
var vkl = VKM.GetVKL(vid);
if (vkl != null && (Member.Roles.IsSupremeAdministrator || (Member.CurrentUser == vkl.UserId && vkl.Visibility != VKLVisibility.Default)))
{
vkl.Data = VKM.LoadVKLData(vkl.Name, vkl.Type);
Wizard.SetActiveView(step2);
Act = Activities.Editing;
EditingVKLId = vkl.Id;
EditingVKL = vkl;
Character.Checked = (vkl.Type == VKLTypes.Character ? true : false);
Previous.Visible = false;
var mappings = Map.Default();
TranslateFromXMappings(vkl.Data.Element("mappings"), mappings);
MRows.DataSource = mappings.Maps;
MRows.DataBind();
scounttr.Value = mappings.NextMapPos.ToString();
LoadInscript(vkl.Data.Element("props").Element("inscript").Value);
}
else
{
Common.LogError("Illegal Attempt with {q=" + Request.Url.Query + "}", "EditVKL", (int)Results.InvalidAction);
ErrorProvider.Text = "The specified Virtual Keyboard Layout does not exist or you do not have permissions to edit it.";
}
}

        private void LoadInscript(string inscript)
        {
            if (VKM.InscriptExists(inscript))
            {
                ulstContainer.Text = VKM.InscriptEntries(inscript);
            }
            else
                ErrorProvider.Text = "No unicode entries found for " + inscript;
        }

        private void PopulateInscripts()
        {
            DataAccess da = new DataAccess();
            if (!ShowAll.Checked)
            {
                if (vklLang.SelectedItem == null || vklLang.SelectedValue == string.Empty)
                    vklScript.Items.Clear();
                else
                vklScript.DataSource = da.ExecuteQuerySPForDataTable("sel_inscripts", new SqlParameter[] { Parameters.String("LangId", vklLang.SelectedItem.Value) }).DefaultView;
}
            else // Show All
                vklScript.DataSource = da.ExecuteQuerySPForDataTable("sel_inscripts", new SqlParameter[] { Parameters.String("LangId", DBNull.Value) }).DefaultView;

            vklScript.DataBind();
vklScript.Items.Insert(0, new ListItem {Value=string.Empty,Text="--Select--"});
        }

        private bool ParseKey(string src, out string dest, out int pos)
        {
            int posIndex = 0;
            dest = string.Empty; pos = 0;
            if (src.Length < 3)
                return false;

            switch (src.Substring(0, 3))
            {
                case "KEY":
                case "RIF":
                    posIndex = 3;
                    break;
                case "CHA":
                case "RDI":
                    posIndex = 4;
                    break;
                case "RTY":
                case "RTH":
                    posIndex = 5;
                    break;
                case "CHK":
                    posIndex = 8;
                    break;
                default:
                    return false;
            }

            if (int.TryParse(src.Substring(posIndex), out pos))
            {
                dest = src.Substring(0, posIndex);
                return true;
            }
            else
                return false;

        }
#endregion

    }


    public class Map
    {

public Map _parent { get; private set; }

#region Values
        public string Key { get; set; }
        public string Character { get; set; }
        public string Code { get {
        return Character.ToUnicodeValue(" ");
        }}
        public bool Shift { get; set; }
        public int pos { get; private set; }
#endregion

#region Id Props
public string KeyName { get; private set; }
public string CharName { get; private set; }
public string ShiftName { get; private set; }

public bool HasMaps
{
get {
if (maps.Count > 0)
return true;
else
return false;
}}

public bool HasRules
{
    get
    {
        if (rules.Count > 0)
            return true;
        else
            return false;
    }
}

public int NextMapPos { get; private set; }

public int NextRulePos { get; private set; }
#endregion

#region Construction
public static Map Default()
{
return new Map();
}

private Map()
{
this.NextMapPos = this.NextRulePos = 1;
}

public Map(Map parent, int pos) : this()
{
this._parent = parent;
this.pos = pos;
if (string.IsNullOrEmpty(this._parent.KeyName))
{
this.KeyName = "key" + pos;
this.CharName = "char" + pos;
this.ShiftName = "chkshift" + pos;
}
else
{
    this.KeyName = this._parent.KeyName+ "_key" + pos;
    this.CharName = this._parent.KeyName + "_char" + pos;
    this.ShiftName = this._parent.KeyName + "_chkshift" + pos;
}
}
#endregion

#region Collections
Dictionary<int, Map> maps = new Dictionary<int, Map>();
public Map[] Maps
{
    get
    {
        var all = maps.Values;
        return all.ToArray();
    }
}

Dictionary<int, Rule> rules = new Dictionary<int, Rule>();
public Rule[] Rules
{
    get
    {
        var all = rules.Values;
        return all.ToArray();
    }
}
#endregion

#region Operations
public Map GetMap(int pos)
{
    if (maps.ContainsKey(pos))
        return maps[pos];
    else
    {
        Map m = new Map(this, pos);
        maps.Add(pos, m);
        if (this.NextMapPos < pos)
            this.NextMapPos = pos + 1;
        return m;
}
}

public Rule GetRule(int pos)
{
    if (rules.ContainsKey(pos))
        return rules[pos];
    else
    {
        Rule r = new Rule(this, pos);
        rules.Add(pos, r);
        if (this.NextRulePos < pos)
            this.NextRulePos = pos + 1;
        return r;
    }
}

public Map AddMap(string key, string character, bool shift)
{
Map m = new Map(this, this.NextMapPos);
maps.Add(this.NextMapPos, m);
m.Key = key;
m.Character = character;
m.Shift = shift;
this.NextMapPos ++;
return m;
}

public Rule AddRule(RuleTypes type, RuleDirections dir, string pattern, string replacement)
{
Rule r = new Rule(this, this.NextRulePos, type, dir, pattern, replacement);
    rules.Add(this.NextRulePos, r);
    this.NextRulePos ++;
        return r;
}

public void RemoveMap(Map m)
{
if (maps.ContainsKey(m.pos))
maps.Remove(m.pos);
}

public void RemoveRule(Rule r)
{
    if (rules.ContainsKey(r.pos))
        rules.Remove(r.pos);
}
#endregion

//public string ToRow()
//{
//StringBuilder tr = new StringBuilder(250);
//tr.Append("<tr><td><input ID=\"key" + pos + "\" title=\"Key\" type=\"text\" value=\"" + Key + "\" />");
//if (maps.Count > 0)
//{
//tr.Append("<a href=\"#\" id=\"arSeq\">Remove Sequence</a></td><td><input ID=\"char" + pos);
// tr.Append("\" title=\"Character\" type=\"text\" value=\"");
//tr.Append(Character + "\"/><a href=\"#\" id=\"arRule\" style=\"display:none;\">Rule</a></td>");
//}
//else if (rules.Count > 0)
//{
//tr.Append("<a href=\"#\" id=\"arSeq\" style=\"display:none;\">Add Sequence</a></td><td><input ID=\"char");
//tr.Append(pos);
// tr.Append("\" title=\"Character\" type=\"text\" value=\"");
//tr.Append(Character + "\"/><a href=\"#\" id=\"arRule\">Add Rule</a></td>");
//}

//                <td>
//                                    <span ID="dcode"></span>
//                </td>
//                <td align="center">
//                    <input ID="chkshift" title="Shift" type="checkbox" />
//                    </td>
//            </tr>
//}


    }

    public enum RuleTypes { TEXT = 1,REGX = 2 }
    public enum RuleDirections { RIGHT = 1, LEFT = 2 }
    public class Rule
    {

        public const string ValidPattern = @"^(?:([^\[\]\s ])|(\[[^\]\[\s ]+\])){1,3}$";

        public RuleTypes Type { get; set; }
        public RuleDirections Direction { get; set; }
                public string Pattern { get; set; }
                public string Replacement { get; set; }
                public string PatternError;


        public int pos { get; private set; }
public Map _parent { get; private set; }

public string TypeName { get; private set; }
public string DirName { get; private set; }
public string IfName { get; private set; }
public string ThenName { get; private set; }

public Rule(Map parent, int pos)
{
this._parent = parent;
this.pos = pos;

this.TypeName = this._parent.KeyName + "_rtype" + pos;
this.DirName = this._parent.KeyName + "_rdir" + pos;
this.IfName = this._parent.KeyName + "_rif" + pos;
this.ThenName = this._parent.KeyName + "_rthen" + pos;
}

public Rule(Map parent, int pos, RuleTypes type, RuleDirections dir, string pattern, string replacement) : this(parent, pos)
{
this.Type = type;
this.Direction = dir;
this.Pattern = pattern;
this.Replacement = replacement;
}

public string ToRow()
{
StringBuilder tr = new StringBuilder(400);
tr.Append("<tr><td><select id=\"rtype\">");
if (Type == RuleTypes.TEXT)
tr.Append("<option value=\"1\" selected>Text</option><option value=\"2\">Range</option>");
else
tr.Append("<option value=\"1\">Text</option><option value=\"2\" selected>Range</option>");

tr.Append("</select></td><td><select id=\"rdir\">");
if (Direction == RuleDirections.RIGHT)
tr.Append("<option value=\"1\" selected>Right</option><option value=\"2\">Left</option>");
else
tr.Append("<option value=\"1\">Right</option><option value=\"2\" selected>Left</option>");

tr.Append("</select></td><td><input ID=\"rif\" title=\"Text to be matched\" type=\"text\" value=\"" + Pattern + "\"/>");
tr.Append("<span id=\"err\">" + PatternError + "</span>");
tr.Append("</td><td><input ID=\"rthen\" title=\"Text to be typed if match found\" type=\"text\" value=\"" + Replacement + "\" /></td></tr>");
return tr.ToString();
}

}


}
