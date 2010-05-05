
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Akshar.aspx.cs" Inherits="Akshar.Akshar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Akshar</title>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>
    <script src="GENERIC.js" type="text/javascript"></script>
<script src="vkm-0.4.8.js" type="text/javascript" ></script>
<script src="ui.core.min.js" type="text/javascript" ></script>
<script src="ui.dialog.min.js" type="text/javascript" ></script>
<link href="themes/ui-lightness/ui.all.css" rel="Stylesheet" />
<script src="akshar-0.2.5.js" type="text/javascript" ></script>
<style type="text/css">
.submenu, .menu, .c-menu {
background-color:#00ffcc;
border:black solid 1px;
font-family:Verdana;
font-size:13pt;
}

.menu:hover, .menuitem:hover,.toolitem a:hover {
background-color:#adff2f;
}
.c-menu a:hover {
cursor:pointer;
}

.m-separator {}

.toolbar {
display:none;
}

.toolitem {
margin-left:0px;
}


.toolitem a {
color:Black;
padding-left:4px;
padding-right:4px;
}

.submenu {
padding:1px;
padding-bottom:3px;
padding-right:3px;
display:inline;
visibility:hidden;
position:absolute;
margin-top:5px;
}

.menuitem {
display:block;
text-align:left;
text-decoration: none;
margin-bottom:3px;
padding-right:25px;
}

div.menuitem a {
margin-left:2px;
color:Black;
}
a {
cursor:default;
}

disabledMI 
{
color:Gray;
}

.menubar {
margin-left:100px;
width:500px;
}

.menu {
padding:1px;
padding-left:5px;
padding-right:5px;
margin-left:2px;
}

.lpane {
float:left;
width:53%;
height:350px;
overflow:scroll;
}
.rpane {
float:right;
width:45%;
height:350px;
}

.fileLV, .dirTrV
{
    list-style-type:none;
}

.fileLV li a,.dirTrV li a {
width:155px;
color:black;
}

.c-menu 
{
    position:absolute;
    display:inline;
    width:120px;
        padding:1px;
padding-bottom:3px;
}
.c-menu a
{
color:Black;
    display:block;
    text-align:left;
    text-decoration:none;
margin-bottom:3px;
font-size:15px;
font-family:Verdana;
}

.fileSelected 
{
background-color:#00ccff;
font-weight:bold;
}

#dlgOpen img,#dlgSave img {
height:20px;
margin-right:20px;
}

</style>
</head>
<body>
<form id="form1" runat="server">
<div>
<img src="images/checked.png" id="marked" alt="Checked" style="display:none;width:15px;height:15px;" />
<h1 role="banner" style="text-align:center;margin-top:-10px;font-size:14pt;">Akshar is an accessible, embeddable, highly extensible, multilingual online word processor for all.</h1>
<br />
<a onclick="" tabindex=0 style="float:right;" id="aview"></a>
<br />
<center>
<h2 id="doctitle"></h2>
</center>
<div role="navigation" id="aksMenubar" class="menubar">
<span class="menu">
<a role="button" aria-haspopup="1" onclick="" tabindex=0 id="mfile" initial="mf">File</a>
</span>
<span class="menu">
<a role="button" aria-haspopup="1" onclick="" tabindex=0 id="mview" initial="mv">View</a>
</span>
<span class="menu">
<a role="button" aria-haspopup="1" onclick="" tabindex=0 id="mlanguage" initial="ml">Language</a>
</span>
<span class="menu">
<a role="button" aria-haspopup="1" onclick="" tabindex=0 id="mdocument" initial="md">Document</a>
</span>
<span class="menu">
<a role="button" aria-haspopup="1" onclick="" tabindex=0 id="mtools" initial="mt">Tools</a>
</span>
<span class="menu">
<a role="button" aria-haspopup="1" onclick="" tabindex=0 id="mhelp" initial="mh">Help</a>
</span>
<br />
<div class="submenu" id="mfPop">
<div class="menuitem">
<a onclick="" tabindex=0 id="mfNew">New</a>
</div>
<div class="menuitem">
<a onclick="" tabindex=0 id="mfOpen">Open ...</a>
</div>
<hr class="m-separator" />
<div class="menuitem">
<a onclick="" tabindex=0 id="mfSave">Save</a>
</div>
<div class="menuitem">
<a onclick="" tabindex=0 id="mfSaveAs">Save as ...</a>
</div>
<hr class="m-separator" />
<div class="menuitem">
<a onclick="" tabindex=0  id="mfClose">Close</a>
</div>
</div>
<div class="submenu" id="mvPop" tabindex=-1>
<div class="menuitem">
<a onclick="" tabindex=0 id="mvFormat">Formatting Bar</a>
</div>
</div>
<div class="submenu" id="mlPop">
<div class="menuitem">
<a onclick="" tabindex=0 id="mlDef">Default (Your System's Current)</a>
</div>
<hr class="m-separator" />
<div class="menuitem">
<a onclick="" tabindex=0 id="mlAdd">Add ...</a>
</div>
</div>
<div class="submenu" id="mdPop">

</div>
<div class="submenu" id="mtPop">
<div class="menuitem">
<a href="keyboard.aspx" target="_blank" tabindex=0 id="mtVkl">New VKL</a>
</div>
<div class="menuitem">
<a href="account.aspx" target="_blank" tabindex=0 id="mtAccount">My Account</a>
</div>
</div>
<div class="submenu" id="mhPop">
<div class="menuitem">
<a href="index.html" target="_blank" tabindex=0 id="mtHome">Home</a>
</div>
<div class="menuitem">
<a href="accessibility.html" target="_blank" tabindex=0 id="mhAcc">Accessibility</a>
</div>
<div class="menuitem">
<a href="feedback.aspx" target="_blank" tabindex=0 id="mhFeed">Feedback</a>
</div>
</div>
</div>
<div id="tbFormat" class="toolbar" style="margin-top:5px;">
<span class="toolitem">
<a tabindex=0 title="Bold" unselectable="on" onclick="" cmd="b" style="font-weight:bold;">B</a>
<a title="Italic" tabindex=0 unselectable="on" onclick="" cmd="i" style="font-style:italic;">I</a>
<a title="Underlined" tabindex=0 unselectable="on" onclick="" cmd="u" style="text-decoration:underline;">U</a>
</span>
<span class="toolitem">
<a title="Decrease Size" tabindex=0 unselectable="on" onclick="" cmd="fs-1">&lt;</a>
<a title="Increase Size" tabindex=0 unselectable="on" onclick="" cmd="fs+1">&gt;</a>
</span>
<span class="toolitem">
<a title="Ordered List" tabindex=0 unselectable="on" onclick="" cmd="ol">OL</a>
<a title="Unordered List" tabindex=0 unselectable="on" onclick="" cmd="ul">UL</a>
</span>
<span class="toolitem">
<a title="Superscript" tabindex=0 unselectable="on" onclick="" cmd="sup">X<sup>2</sup></a>
<a title="Subscript" tabindex=0 unselectable="on" onclick="" cmd="sub">X<sub>2</sub></a>
</span>
<span class="toolitem">
<select title="Text Color" id="tfFor"></select>
</span>
<span class="toolitem">
<select title="Background Color" id="tfBack"></select>
</span>
<span class="toolitem">
<a title="Justify Left" tabindex=0 unselectable="on" onclick="" cmd="jl">L</a>
<a title="Justify Center" tabindex=0 unselectable="on" onclick="" cmd="jc">C</a>
<a title="Justify Right" tabindex=0 unselectable="on" onclick="" cmd="jr">R</a>
</span>
<span class="toolitem">
<a title="Clear Formats" unselectable="on" onclick="" tabindex=0 cmd="cls">CLS</a>
</span>
</div>
<div role="main">
<iframe id="aksharContainer" style="margin-top:10px;border: 2px solid black; height: 390px; width: 99.7%; overflow-x: auto; overflow-y: scroll;" spellcheck="false" frameborder="0"></iframe>
<div style="width:100%;text-align:center;font-size:20pt;height:20pt;border:black solid 1px;background-color:Silver;" id="statusPane"></div>
</div>
</div>
</form>

<div id="dlgOpen" style="display:none;width:100%;font-size:10pt;">
<div tabindex=0 class="lpane"></div>
<div tabindex=0 class="rpane"></div>
<div style="display:block;width:100%;">
<label for="txPath">Look in:</label><input type="text" readonly id="txPath" style="width:150px;" />
<label for="txFile">Document:</label><input type="text" readonly id="txFile" style="width:150px;" />
</div>
</div>
<div id="dlgSave" style="display:none;width:100%;font-size:10pt;">
<div tabindex=0 class="lpane"></div>
<div tabindex=0 class="rpane"></div>
<div style="display:block;width:100%;">
<label for="txPath">Save in:</label><input type="text" readonly id="txPath" style="width:150px" />
<label for="txFile">Name:</label><input type="text" id="txFile" style="width:150px;" />
</div>
</div>

<asp:Literal runat="server" ID="lsettings"></asp:Literal>
</body>
</html>
