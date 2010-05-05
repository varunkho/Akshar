<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="keyboard.aspx.cs" Inherits="Akshar.keyboardProps" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Develop a Virtual Keyboard Layout for Your Language</title>
<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>
<script type="text/javascript" src="GENERIC.js"></script>
<% if (Wizard.ActiveViewIndex == 0) { %>
<script type="text/javascript">
    contextHelp = true;
var vklTypes={Key: <%= (int)Akshar.Lib.VKLTypes.Key%>, Character: <%= (int)Akshar.Lib.VKLTypes.Character %>};
var l,s,t,cselType=vklTypes.Key,radios,vklName;
    $(document).ready(function() {
l = _("vklLang");
s = _("vklScript");
vklName=_("vklName");
t=_("vklTmpl");
radios = document.getElementsByName("vklType");

if (typeof vkls !== "undefined") {
for (var i in vkls)
vkls[i].lcode = $.trim(vkls[i].lcode.toUpperCase());
}

        $(l).change(listChange);
$(s).change(listChange);
radioClicked(true);
        if (!(l.options.length && s.options.length && l.options[l.selectedIndex].value && s.options[s.selectedIndex].value))
            listChange();
populateVkl();
    })

function listChange() {
if (l.options.length && s.options.length && l.options[l.selectedIndex].value && s.options[s.selectedIndex].value)
vklName.value = l.options[l.selectedIndex].text + "-" + s.options[s.selectedIndex].text;
else
vklName.value = "";
if (this === l)
populateVkl();
        }

    function radioClicked(loading) {
        var st = vklTypes.Character;
        if (radios[0].checked)
            st = vklTypes.Key;

        if (st != cselType) {
            cselType = st;
            if (!loading)
            populateVkl();
        }
    }

    function populateVkl() {
        t.innerHTML = "";
        var item = document.createElement("OPTION");
        item.value = ""; item.text = "No thanks (I'll start from scrach.)";
        t.options.add(item);
t.selectedIndex = 0;
        if (typeof vkls !== "undefined" && l.selectedIndex > 0) {
            var selLang =$.trim(l.options[l.selectedIndex].value.toUpperCase());
            for (var i in vkls) {
                if (vkls[i].lcode == selLang && vkls[i].type == cselType) {
                    item = document.createElement("option");
                    item.value = item.text = vkls[i].name;
                                        t.options.add(item);
                }
            }
        }
    }

</script>
<style>
#chAll { cursor:pointer;}
.cs-chbut { cursor:pointer; height:20px;}
</style>
<% } else if (Wizard.ActiveViewIndex == 1) { %>
<%--
For Sequence:
1. The parent row's arSeq must have an attribute "tog" defined (without regard to its value).
2. The following row to the parent row contains the nested table.
3. the nested table must have two attributes defined:
1) ni, next positional index for generating unique field names.
2) pref, prefix (usually the parent's key name) that must be appended to every field of the nested table for uniqueness as well as identification of the hierarchy when the data is posted on the server.
For Rule:
1. The containing row's arRule must have an attribute "tog" defined (without regard to its value).
2. The char column contains the nested table.
3. the nested table must have two attributes defined:
1) ni, next positional index for generating unique field names.
2) pref, prefix (usually the container's key name) that must be appended to every field of the nested table for uniqueness as well as identification of the hierarchy when the data is posted on the server.

--%>

<script type="text/javascript">
    var editingtr, editingCtrl, scode = null, counttr, vklType = '<%= VKLType %>'.toUpperCase(),
    vklMaps, rexp, mexp, mtemplate, rtemplate, vrules, vrTemplate, vmshall,
    rtt;

    $(document).ready(function() {
        counttr = parseInt(_("scounttr").value), vmshall = _('vmshall');
        vklMaps = _("vklmaps"), makeVisible((rexp = _('rexpt').cloneNode(true)), true), makeVisible((mexp = _('mexpt').cloneNode(true)), true);
        vrules = _('vrules'); vrTemplate = $(vrules).clone()[0]; vrTemplate.deleteRow(1);
        mtemplate = _("mapTemplate"); rtemplate = vrules.rows[1];
        mexp.setAttribute("id", 'mexp'); rexp.setAttribute("id", 'rexp');

        $("input,select", "#vklmaps tr:gt(1)").bind("focus", map_FocusChanged);

        $("#vklmaps").bind("click", function(e) {
            var id, li = e.target.id.lastIndexOf("_");
            if (li > 0) id = e.target.id.substr(li + 1);
            else id = e.target.id;

            switch (id) {
                case "arSeq":
                    var curTr = $(e.target).parents("tr:first")[0];
                    arSeqClicked(e.target, curTr);
                    break;
                case "arRule":
                    var curTr = $(e.target).parents("tr:first")[0];
                    arRuleClicked(e.target, curTr);
                    break;
                case "btnMAdd":
                    var tb = $(e.target).parents("table:first")[0];
                    if (tb.id == vklMaps.id)
                        addRow("");
                    else {
                        var ni = tb.getAttribute("ni");
                        $("#arSeq", addRow("", false, tb, tb.getAttribute("pref"), ni)).remove();
                        tb.setAttribute("ni", ni + 1);
                    }
                    break;
                case "btnRAdd":
                    var tb = $(e.target).parents("table:first")[0];
                    var ni = tb.getAttribute("ni");
                    addRule(tb, tb.getAttribute("pref"), ni);
                    tb.setAttribute("ni", ni + 1);
                    break;
                case "mexp":
                    var curTr = $(e.target).parents("tr:first")[0];
                    mexpClicked(e.target, curTr);
                    break;
                case "rexp":
                    var curTr = $(e.target).parents("tr:first")[0];
                    rexpClicked(e.target, curTr);
                    break;
            }
        }) // click

        $("#ulst tr").each(function(id) {
            if (id == 0)
                $(this).children("th:first").after("<th style='position:relative;left:5px;'>Code</th>");
            else {
                var chrel = $(this).children("td:first");
                var ustr = $(chrel).text();
                if (ustr.length > 1)
                    $(this).children("td:first").after("<td>" + ustr.charCodeAt(1) + "</td>");
                else
                    $(this).children("td:first").after("<td>" + ustr.charCodeAt(0) + "</td>");

                $(chrel).wrapInner("<a onclick=''></a>").click(function() {
                    addRow((ustr.length > 1 ? ustr.charAt(1) : ustr.charAt(0)));
                })
            }
        }) // each

        $("img[id=mexp],img[id=rexp]").each(function() {
            if (this.id == "mexp")
                this.parentNode.parentNode.mexp = this;
            else
                this.parentNode.parentNode.rexp = this;
        }) // r/mexp

        bindEvents(_("vklmaps"), "keydown keypress  keyup", function(eve) {
            var e = window.event || eve, target = e.srcElement || e.originalTarget;
                        if (target.tagName.toLowerCase() != "input")
                return;

            var id = target.id.toLowerCase();
            switch (e.type.toLowerCase()) {
                case "keypress":
                    if (id != "key" || e.charCode == 0)
                        return;
                    if (vklType == 'KEY') {
                        $(target).val(scode);
                        // The auto detect shift feature is not available for character based VKL to make such VKLs purely based on the typed character (although shift checkbox is still provided in case the user needs to detect its presence.)
                        if (e.shiftKey)
                            $("#chkshift:first", $(target).parents("tr:first")).attr("checked", true);
                        else
                            $("#chkshift", $(target).parents("tr:first"))[0].checked = false;
                    } else
                        $(target).val(String.fromCharCode(e.which || e.keyCode));

                    // JQuery is not able to cancel an event when the handler is not directly associated with the element that raises the event.
                    return cancelEvent(e, true);
                case "keydown":
                    scode = e.which || e.keyCode;
                    break;
                case "keyup":
                    scode = null;
                    if (id == "char")
                        charEnter(target);
                    break;
            }
        });

        $("#genUchar").click(function() {
            var uValue = parseInt($("#uval").val());
            if (isNaN(uValue)) { alert("Doesn't seem to be a number."); return; }
            if (editingtr == null) { alert("No row is selected."); return; }
            if (editingtr[0].getAttribute("rtype")) {
                if (editingCtrl[0].type.toUpperCase() == "TEXT")
                    editingCtrl.val(editingCtrl.val() + String.fromCharCode(uValue));
                else {
                    var txChar = $("#rthen", editingtr[0]);
                    txChar.val(txChar.val() + String.fromCharCode(uValue));
                }
            } else {
                var txChar = $("#char", editingtr[0]);
                txChar.val(txChar.val() + String.fromCharCode(uValue));
                charEnter(txChar);
            }
        }) // genUchar click


        $("#genAll").click(function() {
            rowNum = 1; addRows();
        }) // genAll click

        //        $("#addNew").bind("click", function() {
        //            addRow("");
        //        }) // addNew
    }) // ready event

    function allShowHide(show) {
show = (show === undefined ? !(vmshall.tog) : show);
        if (show) {
            vmshall.tog=true;
            setText(vmshall, "Collapse All");
        } else {
        vmshall.tog=false;
            setText(vmshall, "Expand All");
        }

        $("table[pref]").each(function() {
            if (this.getAttribute("type") == "rule") {
                if (isVisible(this) != show)
                    rexpClicked(this.parentNode.parentNode.rexp, this.parentNode.parentNode);
            } else {
                if (isVisible(this.parentNode.parentNode) != show) {
var ri=this.parentNode.parentNode.rowIndex, ptb=this.parentNode.parentNode.parentNode;
mexpClicked(ptb.rows[ri - 1].mexp, ptb.rows[ri - 1]);
                }
            }
        })
    }

    var rowNum = 1;
    function addRows() {
        var tb = _("ulst");
        for (; rowNum < (rowNum + 5) && rowNum < tb.rows.length; rowNum++) {
            var ustr = $(tb.rows[rowNum].cells[0]).text();
            addRow((ustr.length > 1 ? ustr.charAt(1) : ustr.charAt(0)), true);
        }
        if (rowNum < tb.rows.length)
            window.setTimeout(addRows, 500);
    }

    function addRow(uchar, suppressive, tb, pref, suf) {
        var newTR = $(mtemplate).clone();
        newTR.attr("id", "");newTR.show();
        var txchar = $("#char", newTR), txkey = $("#key", newTR), chshift = $("#chkshift", newTR);

        if (!tb) {
            $(vklMaps.rows[vklMaps.rows.length - 1]).before(newTR);
            txchar.val(uchar).attr("name", "char" + counttr.toString()); txkey.attr("name", "key" + counttr.toString()); chshift.attr("name", "chkshift" + counttr.toString());
                        counttr++;
        } else {
        $(tb.rows[tb.rows.length - 1]).before(newTR);
            txchar.val(uchar).attr("name", pref + "_char" + suf.toString()); txkey.attr("name", pref + "_key" + suf.toString()); chshift.attr("name", pref + "_chkshift" + suf.toString());
        }
if (uchar.length > 0)
        $("#dcode", newTR).text(uchar.toUnicode(" "));

        $("input", newTR).bind("focus", map_FocusChanged);

        if (!suppressive)
            txkey.trigger("focus");

        return newTR[0];
    }

    function map_FocusChanged(e) {
        // try {
        editingCtrl = $(e.target);
        var tmp, newTR = $(e.target).parents("tr:first");
                        if (editingtr == null || newTR[0] !== editingtr[0]) {
                            newTR.addClass("edittr");
                            // if (newTR[0].getAttribute("rtype"))
                                // alert("!!!");
                if (editingtr != null) {
                    editingtr.removeClass("edittr");
                    // if (editingtr[0].parentNode !== newTR[0].parentNode && (tmp = editingtr.parents("table:first")[0]).id != vklMaps.id && (tmp = tmp.parentNode.parentNode.previousSibling) !== newTR[0]) {
                        // mexpClicked(tmp.mexp, tmp);
                    // }
                }
                editingtr = newTR;
            }
        // } catch (exception) { alert(exception); }
        }

    function charEnter(oed) {
        var ustr = $(oed).val();
        $("#dcode", $(oed).parents("tr:first")).text(ustr.toUnicode(" "));
    }

    function vmTemplate() {
        var vmt = document.createElement("table");
$(vmt).append(vklMaps.rows[0].cloneNode(true));
        $(vmt).append(vklMaps.rows[vklMaps.rows.length - 1].cloneNode(true));
        // alert(vmt.innerHTML);
        return vmt;
    }

    function mexpClicked(mexp, tr) {
        if (isVisible(vklMaps.rows[tr.rowIndex + 1])) {
            makeVisible(vklMaps.rows[tr.rowIndex + 1], false);
            setColExp(mexp, "Expand", "images/expand.gif");
        } else {
        makeVisible(vklMaps.rows[tr.rowIndex + 1], true);
        setColExp(mexp, "Collapse", "images/collapse.gif");
        }
    }
    function arSeqClicked(arSeq, tr) {
        if (arSeq.getAttribute("tog")) {
            if (confirm("Really want to delete the sequence?")) {
                arSeq.removeAttribute("tog");
                vklMaps.deleteRow(tr.rowIndex + 1);
                $("#mexp", tr.cells[0]).remove();
                tr.mexp = null;
                $("img[id$=arRule]:first", tr.cells[1]).show(); $("#char:first", tr.cells[1])[0].readOnly = false;
                arSeq.setAttribute("src", "images/s.gif"); arSeq.setAttribute("title", "Add Sequence"); arSeq.setAttribute("alt", "Add Sequence");
                // setText(arSeq, "Add Sequence");
            }
        } else {
            var newTr = vklMaps.insertRow(tr.rowIndex + 1), tb = vmTemplate(),
                pref = $(tr.cells[0]).children("input")[0].name, newCell = newTr.insertCell(0), exp = mexp.cloneNode(true);
            newCell.colSpan = 4; newCell.appendChild(tb);
            $("#arSeq", addRow("", false, tb, pref, 1)).remove();
            arSeq.setAttribute("tog", true); tb.setAttribute("pref", pref); tb.setAttribute("ni", 2);
            $(tr.cells[0]).append(exp); tr.mexp = exp;
            $("img[id$=arRule]:first", tr.cells[1]).hide(); $("#char:first", tr.cells[1])[0].readOnly = true;
            arSeq.setAttribute("src", "images/sd.gif"); arSeq.setAttribute("title", "Remove Sequence"); arSeq.setAttribute("alt", "Remove Sequence");
            // setText(arSeq, "Remove Sequence");
        }
    }

    function rexpClicked(rexp, tr) {
var tb = $("table:first", tr.cells[1])[0];
        if (isVisible(tb)) {
            makeVisible(tb, false);
            tr.cells[1].colSpan = 1;
            makeVisible(tr.cells[0], true);makeVisible(tr.cells[2], true);makeVisible(tr.cells[3], true);
            $("#char", tr.cells[1])[0].style.marginLeft=0;
setColExp(rexp, "Expand", "images/expand.gif");
} else {
var cleft = tr.cells[1].offsetLeft;
            makeVisible(tb, true);
            makeVisible(tr.cells[0], false);makeVisible(tr.cells[2], false);makeVisible(tr.cells[3], false);
            tr.cells[1].colSpan = 4;
            $("#char", tr.cells[1])[0].style.marginLeft=cleft-25;
setColExp(rexp, "Collapse", "images/collapse.gif");
        }
    }

    function arRuleClicked(arRule, tr) {
        if (arRule.getAttribute("tog")) {
            if (confirm("Really want to delete all the rule(s)?")) {
            var tb = $("table:first", tr.cells[1]);
            if (isVisible(tb[0]))
                rexpClicked(tr.rexp, tr);

tb.remove();arRule.removeAttribute("tog");
                $(tr.rexp).remove();tr.rexp = null;
                $("img[id$=arSeq]", tr.cells[0]).show();
                // setText(arRule, "Add Rule");
                arRule.setAttribute("src", "images/r.gif");arRule.setAttribute("title", "Add Rule");arRule.setAttribute("alt", "Add Rule");
            }
        } else {
            var tb = $(vrTemplate).clone()[0],
                pref = $(tr.cells[0]).children("input")[0].name, exp = rexp.cloneNode(true);
                $(arRule).after(tb);
                addRule(tb, pref, 1);
            arRule.setAttribute("tog", true); tb.setAttribute("pref", pref); tb.setAttribute("ni", 2);
            $(arRule).after(exp); tr.rexp = exp;
            $("img[id$=arSeq]", tr.cells[0]).hide();
            // setText(arRule, "Remove Rule");
            arRule.setAttribute("src", "images/rd.gif"); arRule.setAttribute("title", "Remove Rule"); arRule.setAttribute("alt", "Remove Rule");
            rexpClicked(exp, tr);
        }
    }

function addRule(tb, pref, suf) {
        var newTR = $(rtemplate).clone();
        $(tb.rows[tb.rows.length - 1]).before(newTR);
var rtype = $("#rtype", newTR), rdir = $("#rdir", newTR), rif = $("#rif", newTR), rthen = $("#rthen", newTR);
rtype.attr("name", pref + "_rtype" + suf.toString()); rdir.attr("name", pref + "_rdir" + suf.toString()); rif.attr("name", pref + "_rif" + suf.toString()); rthen.attr("name", pref + "_rthen" + suf.toString());
newTR.attr("rtype", true);
// newTR.bind("focus", map_FocusChanged);
$("input,select", newTR).bind("focus", map_FocusChanged);
rtype.trigger("focus");
return newTR[0];
}
</script>

<script id="Clicks">

    var validRif = /^(?:[^\[\]\s ]|\[[^\]\[\s ]+\]){1,3}$/;
    //        for (;;)
    //        {
    //        s = prompt("tyep");
    //        alert(validRif.test(s));
    //        if (!confirm("Continue?"))
    //        break;
    //        }

    function doneClicked() {
        var retVal = true;
                $("#vklmaps,table[type=map]").each(function() {
            var keys = [];
            for (i = 1; i < (this.rows.length - 1); i++) {
                if (this.rows[i].cells.length < 4) continue; // submap container row
                var txkey = $("#key", this.rows[i].cells[0]), key = txkey.val(), shift = $("#chkshift", this.rows[i].cells[3])[0].checked;

                if (String.isNullOrEmpty(key))
                    continue;

                // if (key == 74) {
                // alert(this.rows[i].cells[3].innerHTML);
                //                     alert(shift + " " + i);
                // }
                if (typeof keys[key] === "object") {
                    if (shift && !keys[key].down)
                        keys[key].down = true;
                    else if (!shift && !keys[key].up)
                        keys[key].up = true;
                    else {
                        alert("You have already chosen this key combination before! " + key);
                        allShowHide(true);
                        txkey.trigger("focus");
                        return (retVal = false);
                    }
                } else {
                    if (shift)
                        keys[key] = { down: true, up: false };
                    else
                        keys[key] = { down: false, up: true };
                }
            }
        })

        if (!retVal)
            return retVal;

        $("tr[rtype]").each(function() {
            var rtype = $("#rtype", this.cells[0])[0], rif=$("#rif", this.cells[2])[0];
            if (rtype.selectedIndex == 0) {
                if (rif.value.length > 3) {
                    allShowHide(true);
                    alert("If Contained cannot have more than 3 characters.");
                    rif.focus();
                    return (retVal = false);
                }
            } else {
                if (!validRif.test(rif.value)) {
                    allShowHide(true);
                    alert("If Contained does not have a valid pattern or have a pattern that intents to match more than 3 characters.");
                    rif.focus();
                    return (retVal = false);
                }
            }
        }) // each
        return retVal;
    } // done click
</script>
<style type="text/css">
.edittr > td {
border:solid 2px #00ffcc;
font-weight:bold;
font-size:14;
}

#vklmaps {float:left;padding-left:20px;width:49%;}
#vklmaps th {border:black solid 1px;}
#ulst {float:right; width:40%;}

#ulst a {
font-size:20px;
cursor:pointer;
}

#ulst td {
text-align:center;
position:relative;
left:5px;
}

img {
height:20px;
cursor:pointer;
}

.cs-add {
cursor:pointer;
color:blue;
}

.tdcode {
width:100px;
overflow:visible;
font-size:10pt;
}

.tdkey {
width:145px;
}

.tbrule {
margin-bottom:5px;
margin-top:5px;
}

        .spacer { clear: both; line-height: 0; height: 0; padding: 0; margin: 0;}
</style>
<% } %>

    </head>
<body>
<center><asp:label ID="ErrorProvider" runat="server" ForeColor="Red" EnableViewState=false /></center>
<form id="form1" runat="server" defaultbutton="">
<asp:MultiView ID="Wizard" runat=server ActiveViewIndex="-1">
<asp:View ID="step1" runat=server>
<a tabindex=0 onclick="" id="chAll" tog=1>Collapse All</a>
<h2 style="position:relative; text-align :center ; margin-bottom: 44px;">Step 1 - 
    Specify the name, language, script and more about Your new Virtual Keyboard 
    Layout.</h2>
<center>
<div style="<%--p   osition :relative;  left: 400px; height: 100px; width: 1052px; --%>margin:5px;">
<label style="margin-left:-120px;" for="vklLang">Language:</label>
<asp:DropDownList ID="vklLang" runat="server" DataTextField="Language"
            DataValueField="LangId" OnSelectedIndexChanged="vklLang_SelectedIndexChanged" AutoPostBack="true" />
<asp:RequiredFieldValidator ID="LangRequired" runat="server"
        ControlToValidate="vklLang" ErrorMessage="Language is required." 
        ValidationGroup="allval">Language is required.</asp:RequiredFieldValidator>
<br />
<label for="vklScript" style ="position: relative ; margin-bottom :10px;">Input 
    Script:</label>
<asp:DropDownList ID="vklScript" runat="server" DataTextField="Script" 
            DataValueField="ScriptId" style="width:130px;"/>
<asp:CheckBox ID="ShowAll" runat="server" Text="Show All" 
            oncheckedchanged="ShowAll_CheckedChanged" AutoPostBack="true" style ="position: relative;"/>
<asp:RequiredFieldValidator ID="InscriptRequired" runat="server" ValidationGroup="allval"
 ControlToValidate="vklScript" ErrorMessage="Input Script is required." style ="position: relative ;width: 130px;">Input Script is required.</asp:RequiredFieldValidator>
<br />
<label for="vklName" style ="margin-left:-55px;">Name:</label>
<asp:TextBox ID="vklName" runat="server" MaxLength="25" style ="position: relative ;"/>
<asp:RequiredFieldValidator ID="NameRequired" runat="server" 
        ControlToValidate="vklName" ErrorMessage="Name is required." ValidationGroup="allval" style ="position: relative ; width: 130px;">Name is required.</asp:RequiredFieldValidator>
<asp:RegularExpressionValidator ID="NameRegex" runat="server" 
                                    ControlToValidate="vklName" Display="Dynamic" 
                                    ErrorMessage="Name contains invalid character(s)." 
                                    ValidationExpression="[a-zA-Z-\(\) ]*" ValidationGroup="allval" style ="position: relative ;">Invalid Input</asp:RegularExpressionValidator>
<div style="margin-left:-330px;">
<img onclick="" class="cs-chbut" src="images/collapse.gif" title="Collapse" alt="Collapse" /><br />
<ul class="chelp"><li>Case Insensitive</li><li>Up to 25 characters.</li><li>Only 
    alphabets (a-z) and punctuations (-()) are allowed.</li></ul>
</div>
<asp:RadioButton ID="Key" runat="server" GroupName="vklType" Checked=true Text="Key Based" style ="margin-left:-70px;" onclick="radioClicked();" />
<asp:RadioButton ID="Character" runat="server" GroupName="vklType" Text="Character Based" onclick="radioClicked();" />
<br />
<label for="vklTmpl" style="margin-left:-200px;">Existing VKL as Template:</label>
<select ID="vklTmpl" runat="server"></select>
<br /><br />
<asp:Button ID="Next" runat="server" Text="Next" AccessKey="n" onclick="Next_Click" ValidationGroup="allval" />
<input type="button" style="margin-left:3px;" accesskey="c" onclick="javascript:window.location.href='account.aspx'" value="Cancel" />
<br /><br />
 </div>
 </center>
</asp:View>
<asp:View ID="step2" runat="server" EnableViewState=false>
<a style="cursor:pointer;" id=vmshall onclick="allShowHide()">Expand All</a>
<a href="vkl-faq.html" style="float:right;" target="_blank">Get Help with VKL 
    Creation</a>
<h2 style="text-align:center;" >Step 2 - Create key/Character Mappings</h2>
<br />
<label for="uval" style ="font-weight : bold">Unicode Value (decimal, octal or hex): </label><input type="text" id="uval" />
<input type="button" id="genUchar" accesskey="g" style = "position: relative; left:10px;"value="Generate UNICODE" />
<input type="button" id="genAll" style = "position: relative; left:20px;" value="Get Me Quickly" />
<br />
    <div style="width:100%;margin-top:10px;">
        <table ID="vklmaps" cellspacing="5px">
            <tr>
                <th>Key</th>
                <th>Character</th>
                <th>Code</th>
                <th>Shift</th>
            </tr>
            <tr runat=server ID="mapTemplate" style="display:none;">
                <td class="tdkey">
                    <input style="width:30px;" ID="key" title="Key" type="text" value="" />
<img onclick="" tabindex=0 src="images/s.gif" title="Add Sequence" alt="Add Sequence" id="arSeq" />
</td>
                <td>
                    <input style="width:70px;" ID="char" title="Character" type="text" value="" maxlength=10 />
<img onclick="" tabindex=0 src="images/r.gif" title="Add Rule" alt="Add Rule" id="arRule" />
                                        </td>
                <td align=center class="tdcode">
                                    <span ID="dcode"></span>
                </td>
                <td align="center">
                    <input ID="chkshift" title="Shift" type="checkbox" />
                    </td>
            </tr>
<asp:Repeater ID="MRows" runat="server" EnableViewState=false>
                <ItemTemplate>
                    <tr>
<td class="tdkey">
                            <input style="width:30px;" ID="key" name='<%# Eval("KeyName") %>' title="Key" type="text" 
                                value='<%# Eval("Key") %>' />
                                 <img onclick="" tabindex=0 src="images/s.gif" title="Add Sequence" alt="Add Sequence" id="arSeq" runat=server />
                                 </td>
<td>
                            <input style="width:70px;" maxlength=10 ID="char" name='<%# Eval("CharName") %>' title="Character" type="text" 
                                value='<%# Eval("Character") %>' <%# (((Akshar.Map)Container.DataItem).HasMaps ? "readonly=true" : "") %> />
                                <img onclick="" tabindex=0 src="images/r.gif" title="Add Rule" alt="Add Rule" id="arRule" runat=server />
                                <asp:Repeater ID="RRows" runat="server" EnableViewState=false>
                                        <HeaderTemplate>
                    <table cellspacing="5px" class="tbrule" type=rule style="display:none;" ni=<%# ((Akshar.Map)Container.DataItem).NextRulePos %> pref='<%# ((Akshar.Map)Container.DataItem).KeyName %>'>
<tr><th>Type</th><th>Direction</th><th>If Contain</th><th>Then Type</th></tr>
</HeaderTemplate>
<ItemTemplate>
<tr rtype=true>
<td>
<select id="rtype" name='<%# ((Akshar.Rule)Container.DataItem).TypeName %>'>
<option value="1" <%# (((Akshar.Rule)Container.DataItem).Type == Akshar.RuleTypes.TEXT ? "selected" : "") %> >
    Text</option>
<option value="2" <%# (((Akshar.Rule)Container.DataItem).Type == Akshar.RuleTypes.REGX ? "selected" : "") %>>
    Range</option>
</select>
</td>
<td>
<select id="rdir" name='<%# ((Akshar.Rule)Container.DataItem).DirName %>'>
<option value="1" <%# (((Akshar.Rule)Container.DataItem).Direction == Akshar.RuleDirections.RIGHT ? "selected" : "") %>>
    Right</option>
<option value="2" <%# (((Akshar.Rule)Container.DataItem).Direction == Akshar.RuleDirections.LEFT ? "selected" : "") %>>
    Left</option>
</select>
</td>
<td>
                    <input ID="rif" title="Text to be matched" type="text" name='<%# ((Akshar.Rule)Container.DataItem).IfName %>' value='<%# ((Akshar.Rule)Container.DataItem).Pattern %>' /><span id="rifErr" style="color:Red;"><%# ((Akshar.Rule)Container.DataItem).PatternError %></span>
                                        </td>
<td>
                    <input ID="rthen" maxlength=10 title="Text to be typed if match found" type="text" name='<%# ((Akshar.Rule)Container.DataItem).ThenName %>' value='<%# ((Akshar.Rule)Container.DataItem).Replacement %>' />
                                        </td>
</tr>
</ItemTemplate>
<FooterTemplate>
<tr>
<td colspan=4 align=center>
<a class="cs-add" tabindex=0 onclick="" role="button" id="btnRAdd">Add New</a>
</td>
</tr>
                    </table>
                    </FooterTemplate>
                    </asp:Repeater>
                                </td>
<td align=center class="tdcode"><span ID="dcode"><%# Eval("Code") %></span></td>
<td align="center">
                            <input ID="chkshift" <%# ((bool) Eval("Shift") ? "checked" : "") %> name='<%# Eval("ShiftName") %>' 
                                title="Shift" type="checkbox" /></td>
                    </tr>
                    <asp:Repeater ID="subMRows" runat="server" EnableViewState=false>
                    <HeaderTemplate>
                    <tr style="display:none;">
                    <td colspan=4>
<table cellspacing="5px" type=map ni=<%# ((Akshar.Map)Container.DataItem).NextMapPos %> pref='<%# ((Akshar.Map)Container.DataItem).KeyName %>'>
            <tr>
                <th>Key</th>
                <th>Character</th>
                <th>Code</th>
                <th>Shift</th>
            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
<tr>
                        <td class="tdkey">
                            <input style="width:30px;" ID="key" name='<%# ((Akshar.Map)Container.DataItem).KeyName %>' title="Key" type="text" 
                                value='<%# ((Akshar.Map)Container.DataItem).Key %>' /></td>
                        <td>
                            <input style="width:70px;" maxlength=10 ID="char" name='<%# ((Akshar.Map)Container.DataItem).CharName %>' title="Character" type="text" 
                                value='<%# ((Akshar.Map)Container.DataItem).Character %>' <%# (((Akshar.Map)Container.DataItem).HasMaps ? "readonly=true" : "") %>  />
                                <img onclick="" tabindex=0 src="images/r.gif" title="Add Rule" alt="Add Rule" id="arRule" runat=server />
<asp:Repeater ID="RRows" runat="server" EnableViewState=false>
                                        <HeaderTemplate>
                    <table cellspacing="5px" class="tbrule" type=rule style="display:none;" ni=<%# ((Akshar.Map)Container.DataItem).NextRulePos %> pref='<%# ((Akshar.Map)Container.DataItem).KeyName %>'>
<tr><th>Type</th><th>Direction</th><th>If Contain</th><th>Then Type</th></tr>
</HeaderTemplate>
<ItemTemplate>
<tr rtype=true>
<td>
<select id="rtype" name='<%# ((Akshar.Rule)Container.DataItem).TypeName %>'>
<option value="1" <%# (((Akshar.Rule)Container.DataItem).Type == Akshar.RuleTypes.TEXT ? "selected" : "") %> >
    Text</option>
<option value="2" <%# (((Akshar.Rule)Container.DataItem).Type == Akshar.RuleTypes.REGX ? "selected" : "") %>>
    Range</option>
</select>
</td>
<td>
<select id="rdir" name='<%# ((Akshar.Rule)Container.DataItem).DirName %>'>
<option value="1" <%# (((Akshar.Rule)Container.DataItem).Direction == Akshar.RuleDirections.RIGHT ? "selected" : "") %>>
    Right</option>
<option value="2" <%# (((Akshar.Rule)Container.DataItem).Direction == Akshar.RuleDirections.LEFT ? "selected" : "") %>>
    Left</option>
</select>
</td>
<td>
                    <input ID="rif" title="Text to be matched" type="text" name='<%# ((Akshar.Rule)Container.DataItem).IfName %>' value='<%# ((Akshar.Rule)Container.DataItem).Pattern %>' /><span id="rifErr" style="color:Red;"><%# ((Akshar.Rule)Container.DataItem).PatternError %></span>
                                        </td>
<td>
                    <input ID="rthen" maxlength=10 title="Text to be typed if match found" type="text" name='<%# ((Akshar.Rule)Container.DataItem).ThenName %>' value='<%# ((Akshar.Rule)Container.DataItem).Replacement %>' />
                                        </td>
</tr>
</ItemTemplate>
<FooterTemplate>
<tr>
<td colspan=4 align=center>
<a class="cs-add" tabindex=0 onclick="" role="button" id="btnRAdd">Add New</a>
</td>
</tr>
                    </table>
                    </FooterTemplate>
                    </asp:Repeater>
                                </td>
                        <td align=center class="tdcode">
                            <span ID="dcode"><%# ((Akshar.Map)Container.DataItem).Code %></span>
                            </td>
                        <td align="center">
                            <input ID="chkshift" <%# (((Akshar.Map)Container.DataItem).Shift ? "checked" : "") %> name='<%# ((Akshar.Map)Container.DataItem).ShiftName %>' 
                                title="Shift" type="checkbox" /></td>
                    </tr>
 </ItemTemplate>
 <FooterTemplate>
 <tr>
<td colspan=4 align=center>
<a tabindex=0 onclick="" role="button" class="cs-add" id="btnMAdd">Add New</a>
</td>
</tr>
 </table>
 </td>
 </tr>
 </FooterTemplate>
                    </asp:Repeater>
                                                        </ItemTemplate>
            </asp:Repeater>
<tr>
<td colspan=4 align=center>
<a class="cs-add" tabindex=0 onclick="" role="button" id="btnMAdd">Add New</a>
</td>
</tr>
        </table>

        <asp:Literal ID="ulstContainer" runat="server"></asp:Literal>
    </div>
    <br class="spacer" />
    <div style="float:left;width:100%;padding-left: 200px;">
<asp:Button ID="Done" runat="server" Text="I'm Done!" AccessKey="o" 
        onclick="done_Click" OnClientClick="return doneClicked()" Width="79px" />
    <asp:Button ID="Previous" runat="server" AccessKey="p" onclick="Previous_Click" 
        Text="Previous" />
<input type="button" style="margin-left:3px;" accesskey="c" onclick="javascript:window.location.href='account.aspx'" value="Cancel" />
<br /><br />
</div>


<asp:HiddenField ID="scounttr" runat="server" Value="1" />
<table cellspacing="5px" class="tbrule" style="display:none;" id="vrules" type=rule>
<tr>
                <th>Type</th>
                <th>Direction</th>
                <th>If Contain</th>
                <th>Then Type</th>
            </tr>
<tr>
<td>
<select id="rtype">
<option value="1">Text</option>
<option value="2">Range</option>
</select>
</td>
<td>
<select id="rdir">
<option value="1">Right</option>
<option value="2">Left</option>
</select>
</td>
<td>
                    <input ID="rif" title="Text to be matched" type="text" /><span id="rifErr" style="color:Red;"></span>
                                        </td>
<td>
                    <input ID="rthen" maxlength=10 title="Text to be typed if match found" type="text" />
                                        </td>
</tr>
<tr>
<td colspan=4 align=center>
<a class="cs-add" tabindex=0 onclick="" role="button" id="btnRAdd">Add New</a>
</td>
</tr>
                    </table>
                    <asp:Literal runat="server" ID="lrexp" Visible=false>
 <img onclick="" tabindex=0 id="rexp" src="images/expand.gif" title="Expand" alt="Expand" />
 </asp:Literal>
 <asp:Literal runat="server" ID="lmexp" Visible=false>
  <img onclick="" tabindex=0 id="mexp" src="images/expand.gif" title="Expand" alt="Expand" />
 </asp:Literal>
 <asp:Literal runat="server" ID="lrSeq" Visible=false>
<img onclick="" tabindex=0 src="images/sd.gif" title="Remove Sequence" alt="Remove Sequence" />
 </asp:Literal>
 <asp:Literal runat="server" ID="lrRule" Visible=false>
<img onclick="" tabindex=0 src="images/rd.gif" title="Remove Rule" alt="Remove Rule" />
 </asp:Literal>
 <img onclick="" tabindex=0 id="mexpt" style="display:none;" src="images/collapse.gif" title="Collapse" alt="Collapse" />
 <img onclick="" tabindex=0 id="rexpt" style="display:none;" src="images/collapse.gif" title="Collapse" alt="Collapse" />
</asp:View>
</asp:MultiView>
    </form>
</body>
</html>
