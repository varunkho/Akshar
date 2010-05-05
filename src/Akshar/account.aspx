<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="account.aspx.cs" Inherits="Akshar.account" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>My Account</title>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>
    <link rel="Stylesheet" type="text/css" href="/layout.css" />
    <script type="text/javascript">
        $(document).ready(function() {

            $("input:not(:disabled)[id=chkPublic]").bind("click", function() {
                if (this.checked) {
                    if (confirm("Once a VKL is made public, it cannot be made private again. Are you sure you want to proceed?")) {
                        $.post("account.aspx?id=" + this.parentNode.getAttribute("vid") + "&val=1&act=vis");
                        $(this.parentNode).html("Yes");
                    } else this.checked = false;
                }
            })

        })

    </script>
<style>
h1 {
text-align:center;
}
    .style1
    {
        width: 326px;
    }
</style>
</head>
<body>
<div id="header" role="navigation">
<ul id="navBar">
<li><a href="index.html">Home</a></li>
<li><a href="Akshar.aspx">Akshar</a></li>
<li><a href="accessibility.html">Accessibility</a></li>
<li><a href="embed.html">Embed</a></li>
<li><a href="vkl-faq.html">VKL FAQ</a></li>
<li><a href="account.aspx">My Account</a></li>
</ul>
<br class="spacer" />
</div>
<div id="content" role="main">
    <form id="form1" runat="server">
    <div style="text-align:right;">
<asp:LoginView ID="LoginView1" runat="server">
    <LoggedInTemplate>Welcome <asp:LoginName ID="LoginName1" runat="server" /> | </LoggedInTemplate>
        </asp:LoginView>
<asp:LoginStatus ID="LoginStatus1" runat="server" />
</div>
<h1>My Virtual Keyboard Layouts</h1>                                      
<a href="keyboard.aspx">Create New</a>
<table style="border:solid 1px black;margin-top:10px; width: 500px;" cellpadding="5px" 
            cellspacing="5px">
<tr>
<th class="style1">Virtual Keyboard Layout</th><th align=left>Visible to Others</th>
</tr>
<asp:Repeater runat="server" ID="VKLLists" EnableViewState=false>
<ItemTemplate>
<tr>
<td>
<a href="<%# VKLLink(Eval("Visibility"), Eval("VKLId")) %>"><%#string.Format("{0} ({1})", Eval("Name"), (int.Parse(Eval("Type").ToString()) == (int)Akshar.Lib.VKLTypes.Character ? "Character" : "Key"))  %></a>   
</td>
<td vid='<%# Eval("VKLId") %>' >
<asp:Literal ID="lstate" runat="server">
<input type="checkbox" id="chkPublic" />
</asp:Literal>
</td>
</tr>
</ItemTemplate>
</asp:Repeater>
</table>

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" OnSelecting="SqlDataSource1_Selecting" 
            ConnectionString="<%$ ConnectionStrings:AkshConnectionString %>" 
            SelectCommand="Sel_UserVKLs" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:Parameter Name="userId" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    </form>
    </div>
    <div id="footer" role="contentinfo">
Copyright 2010-2011 Varun Khosla | <a href="feedback.aspx">Feedback</a> | <a href="mailto:varunkhosla@outlook.com">Contact</a>
</div>
</body>
</html>
