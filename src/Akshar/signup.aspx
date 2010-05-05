<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="signup.aspx.cs" Inherits="Akshar.signup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Sign up</title>
<script type="text/javascript" src="jquery-1.3.2-vsdoc2.js">    // src="http://ajax.googleapis.com/ajax/libs/jquery/1.2.6/jquery.min.js"></script>
<script type="text/javascript" src="GENERIC.js">    // src="http://ajax.googleapis.com/ajax/libs/jquery/1.2.6/jquery.min.js"></script>
<link rel="Stylesheet" type="text/css" href="/layout.css" />
<script type="text/javascript">
    contextHelp = true;
function validateInputRange(oSrc, args)
{
switch (oSrc.id) {
case '<% = UserNameRange.ClientID %>':
if (args.Value.length < 5||args.Value.length >15) {
args.IsValid = false;
return;
}
break;
case '<% = PasswordRange.ClientID %>':
if (args.Value.length < 6||args.Value.length >20) {
args.IsValid = false;
return;
}
break;
case '<% = NameRange.ClientID %>':
if (args.Value.length < 4||args.Value.length >50) {
args.IsValid = false;
return;
}
break;
}
return true;
}

</script>
<style type="text/css">
        .style1
        {
                        width: 721px;
        }
        .style2
        {
            width: 283px;
        }
        #chAll { cursor:pointer;}
.cs-chbut { cursor:pointer; height:20px;}
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
    <form id="newMember" runat="server">
<a tabindex=0 onclick="" id="chAll" tog=1>Collapse All</a>
<h1 style="text-align:center;"> Sign up for Your New Account</h1>
<asp:ValidationSummary ID="allvalSummary" runat="server" ValidationGroup="allval" 
        style="position: relative; top: 0px; left: 600px; width: 280px"/>
                                                                                    <center><asp:label ID="ErrorProvider" runat="server" ForeColor="Red" /></center>
                                                                                    <center>
                                                                            <table class="style1">
                        <tr>
                            <th align=center colspan=2 style="font-weight:bold">
                            <h2>All fields of this section are required.</h2>
                            </th>
                            </tr>
                        <tr>
                            <td align="right" class="style2" >  
 <asp:Label ID="Label1" AssociatedControlID="UserName" runat="server" Text="User Name:"/>
</td>
                            <td align=left>
<asp:TextBox ID="UserName" runat="server" MaxLength="15" Width="180px"></asp:TextBox>
<asp:RequiredFieldValidator ID="UserNameRequired" runat="server"
Display="Dynamic" ControlToValidate="UserName" ErrorMessage="User Name is required."
                                    ValidationGroup="allval">*</asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="UserNameRange" runat="server" ClientValidationFunction="validateInputRange" OnServerValidate="UserNameRange_ServerValidate"
                                    ControlToValidate="UserName" ErrorMessage="User Name length is out of specified range." Display="Dynamic" ValidationGroup="allval">Invalid Range</asp:CustomValidator>
                                                                    <asp:RegularExpressionValidator ID="UserNameRegex" runat="server" 
                                    ControlToValidate="UserName" Display="Dynamic" 
                                    ErrorMessage="User Name contains invalid character(s)." 
                                    ValidationExpression="[a-zA-Z0-9._]*" ValidationGroup="allval">Invalid Input</asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                        <tr>
                            <td align="right" class="style2">
                                                                                    <img onclick="" class="cs-chbut" src="images/collapse.gif" title="Collapse" alt="Collapse" /><br />
<ul class="chelp"><li>Case Insensitive</li><li>Must be between 5-15 characters.</li><li>
    Only alphanumeric characters (a-z, 0-9), dot (.) and underscore (_) are allowed.</li></ul>
                            </td>
                            <td>

 </td>
                        </tr>
                        <tr>
                            <td align="right" class="style2">
<asp:Label ID="Label2" runat="server" Text="Password:" AssociatedControlID="Password"/>
                            </td>
                            <td align=left>
<asp:TextBox ID="Password" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
<asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
Display="Dynamic" ControlToValidate="Password" ErrorMessage="Password is required." 
                                    ValidationGroup="allval">*</asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="PasswordRange" OnServerValidate="PasswordRange_ServerValidate"
                                    runat="server" ClientValidationFunction="validateInputRange" 
                                    ControlToValidate="Password" ErrorMessage="Password 
length is out of specified range." Display="Dynamic" ValidationGroup="allval">Invalid Range</asp:CustomValidator>
                                                                    </td>
                                                    </tr>
                        <tr>
                            <td align="right" class="style2">
<asp:Label ID="Label3" runat="server" Text="Confirm Password:" AssociatedControlID="ConfirmPassword"/>
</td>
                            <td align=left>
<asp:TextBox ID="ConfirmPassword" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
<asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server"
Display="Dynamic" ControlToValidate="ConfirmPassword" ErrorMessage="Confirm Password is required." 
                                    ValidationGroup="allval">*</asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="ConfirmPasswordComparer" runat="server" 
                                    ErrorMessage="Password confirmation doesn't match." 
                                    ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                                    Display="Dynamic" ValidationGroup="allval">Unmatch Confirmation</asp:CompareValidator>
                                    </td>
                                                    </tr>
                         <tr>
<td align=right class="style2">
                                                                                                                <img onclick="" class="cs-chbut" src="images/collapse.gif" title="Collapse" alt="Collapse" /><br />
                                <ul class="chelp"><li>Case Sensitive</li><li>Must be between 6-20 characters.</li></ul>
</td>
<td></td>
</tr>
                        <tr>
                            <td align="right" class="style2" >
<asp:Label ID="Label4" AssociatedControlID="Email" runat="server" Text="Email:"/>
</td>
                            <td align=left>
<asp:TextBox ID="Email" runat="server" style="width: 180px"></asp:TextBox>
<asp:RequiredFieldValidator ID="EmailRequired" runat="server"
Display="Dynamic" ControlToValidate="Email" ErrorMessage="Email is required." 
                                    ValidationGroup="allval">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="EmailRegex" runat="server" 
                                    ControlToValidate="Email" Display="Dynamic" 
                                    ErrorMessage="Email is not valid." ValidationGroup="allval" 
                                    Text="Invalid Input" 
                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                        <tr>
                            <td align=right class="style2">
<asp:Label ID="Label6" runat="server" Text="Name:" AssociatedControlID="Name" />
</td>
                            <td align=left>
<asp:TextBox ID="Name" runat="server" MaxLength="50" 
                                    style="width: 180px;"></asp:TextBox>
<asp:RequiredFieldValidator ID="NameRequired" runat="server"
Display="Dynamic" ControlToValidate="Name" ErrorMessage="Name is required." 
                                    ValidationGroup="allval">*</asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="NameRange" 
                                    runat="server" ClientValidationFunction="validateInputRange" OnServerValidate="NameRange_ServerValidate"
                                    ControlToValidate="Name" 
                                    ErrorMessage="Name length is out of specified range." Display="Dynamic" 
                                    ValidationGroup="allval">Invalid Range</asp:CustomValidator>
</td>
                                                    </tr>
                         <tr>
                         <td colspan="2" style="padding-top:10px;text-align:center;">
                         <asp:CheckBox ID="RememberMe" runat="server" Text="In Future, sign me in Automatically" /> 
                    <asp:Button ID="Submit" runat="server" Text="Create My Account" ValidationGroup="allval" OnClick="submit_Click" />
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
