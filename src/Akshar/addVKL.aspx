<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addVKL.aspx.cs" Inherits="Akshar.addVKL" EnableViewState="false" %>
<%@ Import Namespace="Akshar.Lib" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add VKL to the List</title>
<script type="text/javascript">
function _(id) { return document.getElementById(id); }
    function bindEvent(ctrls, event, fn) {
if (!(ctrls instanceof Array)) { ctrls = [ctrls]; }
                for (var c in ctrls) {
                if (ctrls[c].addEventListener) ctrls[c].addEventListener(event, fn, false);
            else if (ctrls[c].attachEvent) ctrls[c].attachEvent('on' + event, fn);
        }}

var vklTypes={Key: <%= (int)VKLTypes.Key%>, Character: <%= (int)VKLTypes.Character %>},
vklStatus={Personal: <%= (int)VKLVisibility.Personal %>,Public: <%= (int)VKLVisibility.Public %>,Default: <%= (int)VKLVisibility.Default %>},
args = null;
    var cmbLang, cmbVkl, cmbChosen;
    
window.onload = function()
{
cmbLang = _('cmbLang');
cmbVkl = _('cmbVkl');
cmbChosen = _('cmbChosen');

bindEvent(cmbLang, "change", onLanguageChanged);
bindEvent(_("add"), "click", addClicked);
bindEvent(_("remove"), "click", removeClicked);
bindEvent(_("ok"), "click", okClicked);

if (window.dialogArguments)
args = window.dialogArguments;
else if (window.arguments)
args = window.arguments;

if (vkls)
{
for (var v in vkls)
{
vkls[v].typeText = vkls[v].type == vklTypes.Character ? "CHARACTER" : "KEY";
switch (vkls[v].status)
{
case vklStatus.Personal:
vkls[v].displayName = vkls[v].name + ' (Personal)';
break;
case vklStatus.Public:
vkls[v].displayName = vkls[v].name + ' (Public)';
break;
case vklStatus.Default:
vkls[v].displayName = vkls[v].name + ' (Default)';
break;
}
}
}
if (args.vkls)
{
for (var i in args.vkls)
{
var item = document.createElement("option");
        item.text = args.vkls[i].name + " (" + args.vkls[i].type + ")";
        item.vkl = args.vkls[i];
        cmbChosen.options.add(item);
}
}
}

var cselType = vklTypes.Key;
var radios = document.getElementsByName("selType");
function radioClicked()
{
var st = vklTypes.Character;
if (radios[0].checked)
st = vklTypes.Key;

if (st != cselType)
{
cselType = st;
onLanguageChanged()
}
}

function onLanguageChanged() {
    cmbVkl.innerHTML = "";

var item = document.createElement("OPTION");
item.value = "";item.text = "--Select--";
cmbVkl.options.add(item);

if (cmbLang.selectedIndex > 0)
{
var selLang = cmbLang.options[cmbLang.selectedIndex].value.toUpperCase();
    for (var i in vkls) {
            if ((vkls[i].lcode == selLang || vkls[i].lcode.toUpperCase() == selLang) && vkls[i].type == cselType)
{
            item = document.createElement("option");
        item.text = vkls[i].displayName;
        item.value = vkls[i].id;
        item.vkl = vkls[i];
        cmbVkl.options.add(item);
    }
}
}
    }

function addClicked()
{
if (cmbVkl.selectedIndex == 0 || typeof (cmbVkl.options[cmbVkl.selectedIndex].vkl) !== "object")
{
alert("You haven't selected a Virtual Keyboard Layout from the list.");
return;
}

var svkl = cmbVkl.options[cmbVkl.selectedIndex].vkl;
for (var i=0;i<cmbChosen.options.length;i++)
{
var vkl = cmbChosen.options[i].vkl;
if (vkl.name.toUpperCase() == svkl.name.toUpperCase() && vkl.type.toUpperCase() == svkl.typeText)
{
alert("The VKL (" + svkl.name + ", type: " + svkl.typeText + ") is already chosen.");
return;
}
}

var item = document.createElement("option");
        item.text = svkl.name + " (" + svkl.typeText + ")";
        item.vkl = {name: svkl.name, type:svkl.typeText};
cmbChosen.options.add(item);
}

function removeClicked() {
if (cmbChosen.selectedIndex >= 0)
cmbChosen.options.remove(cmbChosen.selectedIndex);
}

function okClicked() {
var selVkls = new Array(cmbChosen.options.length);
for (var i=0;i<cmbChosen.options.length;i++)
{
var vkl = cmbChosen.options[i].vkl;
selVkls[vkl.name.toUpperCase() + (vkl.type == "CHARACTER" ? "_C" : "")] = vkl;
}
args.fn(selVkls);
window.close();
}
</script>
</head>
<body>
<form id="form1" runat="server">
    <div style="margin-top:20px;margin-left:10px;">
<input type="radio" name="selType" value=<%= (int)VKLTypes.Key %> checked onclick="radioClicked()" />Key Based
<input type="radio" name="selType" value=<%= (int)VKLTypes.Character %> onclick="radioClicked()" />Character Based
<br />
<label style="position:relative; top:4px; margin-left:95px;" for="cmbLang">Language:</label>
<asp:DropDownList style="position:relative; top:4px;" ID="cmbLang" runat="server" DataTextField="language"             DataValueField="langId" DataSourceID="dsLanguage" OnDataBound="cmbLang_DataBound" />
<br />
<label style="position:relative; top:8px;" for="cmbVkl">Virtual Keyboard Layout:</label>
<select style="position:relative; top:8px; width:160px;" id="cmbVkl">
<option value="">--Select--</option>
</select>
<br />
<center>
<input type="button" style="position:relative; top:14px;" id="add" accesskey="A" value="Add" />
<select style="position:relative; top:14px; width:160px;" id="cmbChosen"></select>
<input type="button" style="position:relative; top:14px;" id="remove" accesskey="R" value="Remove" />
</center>
<br />
<div style="text-align:right;margin-right:20px;">
<input type="button" style="position:relative; top:14px;" id="ok" accesskey="O" value="Ok" />
<input type="button" style="position:relative; top:14px;" accesskey="c" onclick="window.close();" value="Cancel" />
</div>
    </div>
<asp:SqlDataSource ID="dsLanguage" runat="server" 
            ConnectionString="<%$ ConnectionStrings:AkshConnectionString %>" 
            SelectCommand="SELECT rtrim(langId) as langId, language FROM [Languages] ORDER BY [Language]">
        </asp:SqlDataSource>
    </form>
    <asp:Literal ID="dsVkl" runat="server"></asp:Literal>
</body>
</html>
