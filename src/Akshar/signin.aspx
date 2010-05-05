<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="signin.aspx.cs" Inherits="Akshar.signin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Sign in</title>
    <link rel="Stylesheet" type="text/css" href="/layout.css" />
</head>
<body>
<div id="header" role="navigation">
<ul id="navBar">
<li><a href="index.html">Home</a></li>
<li><a href="Akshar.aspx">Akshar</a></li>
<li><a href="accessibility.html">Accessibility</a></li>
<li><a href="embed.html">Embed</a></li>
<li><a href="vkl-faq.html">VKL FAQ</a></li>
<li><a href="signup.aspx">Register</a></li>
<li><a href="account.aspx">My Account</a></li>
</ul>
<br class="spacer" />
</div>
<div id="content" role="main">
    <form id="form1" runat="server">
    <center><asp:label ID="ErrorProvider" runat="server" ForeColor="Red" /></center></center>
    <center>
        <table>
                                                <tr>
                            <td align="right" >
<asp:Label ID="Label1" AssociatedControlID="UserName" runat="server" Text="User Name:"/>
</td>
                            <td>
<asp:TextBox ID="UserName" runat="server" MaxLength="15" style="width: 128px;"></asp:TextBox>
<asp:RequiredFieldValidator ID="UserNameRequired" runat="server"
Display="Dynamic" ControlToValidate="UserName" ErrorMessage="User Name is required." 
                                    ValidationGroup="allval">*</asp:RequiredFieldValidator>
                                                                                                        </td>
                                </tr>
                        <tr>
                            <td align="right">
<asp:Label ID="Label2" runat="server" Text="Password:" AssociatedControlID="Password"/>
                                                                                                                                            </td>
                            <td>
<asp:TextBox ID="Password" runat="server" MaxLength="20" TextMode="Password" style="width: 128px;"></asp:TextBox>
<asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
Display="Dynamic" ControlToValidate="Password" ErrorMessage="Password is required." 
                                    ValidationGroup="allval">*</asp:RequiredFieldValidator>
                                                                    </td>
                                                    </tr>
                                                    <tr>
                                                    <td colspan="2" style="padding: 10px 0 0 10px;">
                                                    <asp:CheckBox ID="RememberMe" runat="server" Text="In Future, sign me in Automatically" />        
                                                    </td>
                                                    </tr>
                                                    <tr>
                                                    <td colspan="2" style="padding: 10px 0 0 10px;">
                                                           <asp:Button ID="Submit" runat="server" Text="Sign in" ValidationGroup="allval" OnClick="submit_Click" /> or <a href="signup.aspx">Register</a>
                                                    </td>
                                                    </tr>
                                                    </table>
         </center>
                 </form>
        </div>
                <div id="footer" role="contentinfo">
Copyright 2010-2011 Varun Khosla | <a href="feedback.aspx">Feedback</a> | <a href="mailto:varunkhosla@outlook.com">Contact</a>
</div>
</body>
</html>
