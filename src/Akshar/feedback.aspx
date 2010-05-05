<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="feedback.aspx.cs" Inherits="Akshar.feedback" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Feedback</title>
<script type="text/javascript">
function validateInputRange(oSrc, args) {
switch (oSrc.id) {
case '<%= NameRange.ClientID %>':
if (args.Value.length > 50) {
args.IsValid = false;
return;
}
break;
case '<%= SubjectRange.ClientID %>':
    if (args.Value.length > 100) {
        args.IsValid = false;
        return;
    }
    break;
case '<%= DescriptionRange.ClientID %>':
    if (args.Value.length > 1000) {
        args.IsValid = false;
        return;
    }
    break;
}
return true;
}
</script>
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
<li><a href="account.aspx">My Account</a></li>
</ul>
<br class="spacer" />
</div>
<div id="content" role="main">
    <form id="form1" runat="server">
    <h1 style="text-align:left;font-weight:normal;font-size: 17px;">Have a suggestion? Encountered some bug? Want to have a feature added? Tell us, we would love to hear !</h1>
    <center><asp:label ID="ErrorProvider" runat="server" ForeColor="Red" EnableViewState=false /></center>
    <span enableviewstate=false runat=server id="msg" style="text-align:center;font-size:14px;"></span>
    <br />
<center>
<table id="tbFeed" runat=server>
<tr>
<td align=right><label for="Type">Type:</label></td>
<td align=left>
<asp:DropDownList ID="Type" runat="server">
<asp:ListItem Text="Issue" Value="Issue" Selected="True"></asp:ListItem>
<asp:ListItem Text="Suggestion" Value="Suggestion" ></asp:ListItem>
<asp:ListItem Text="Enhancement" Value="Enhancement" ></asp:ListItem>
</asp:DropDownList>
</td>
</tr>
<tr id="trEmail" runat=server>
<td align=right><label for="Email">Email:</label></td>
<td align=left>
<asp:TextBox ID="Email" runat="server" style="width: 280px"></asp:TextBox>
<asp:RequiredFieldValidator ID="EmailRequired" runat="server"
Display="Dynamic" ControlToValidate="Email" ErrorMessage="Email is required." 
                                    ValidationGroup="allval">Email is required.</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator ID="EmailRegex" runat="server" 
                                    ControlToValidate="Email" Display="Dynamic" 
                                    ErrorMessage="Email is not valid." ValidationGroup="allval" 
                                    Text="Email is not valid" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
</td>
</tr>
<tr id="trEmlDesc" runat=server>
<td align=center colspan=2>In case we are not able to comprehend the specific issue/suggestion you are submitting, we would like to reach to you. (for anonymous feedback submition only)</td>
</tr>
<tr id="trName" runat=server>
<td align=right><label for="Name">Your Name:</label><br /><span>At most 50 characters</span></td>
<td align=left>
<asp:TextBox ID="Name" runat="server" MaxLength="50" 
                                    style="width:280px;"></asp:TextBox>
<asp:RequiredFieldValidator ID="NameRequired" runat="server"
Display="Dynamic" ControlToValidate="Name" ErrorMessage="Name is required." 
                                    ValidationGroup="allval">Name is required.</asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="NameRange" 
                                    runat="server" ClientValidationFunction="validateInputRange" OnServerValidate="NameRange_ServerValidate"
                                    ControlToValidate="Name" 
                                    ErrorMessage="Name's length cannot exceed 50 characters." Display="Dynamic" 
                                    ValidationGroup="allval">Name's length cannot exceed 50 characters.</asp:CustomValidator>
</td>
</tr>
<tr>
<td align=right><label for="Subject">Subject:</label><br /><span>At most 100 characters</span></td>
<td align=left>
<asp:TextBox ID="Subject" runat="server" MaxLength="100" 
                                    style="width:350px;"></asp:TextBox>
<asp:RequiredFieldValidator ID="SubjectRequired" runat="server"
Display="Dynamic" ControlToValidate="Subject" ErrorMessage="Subject is required." 
                                    ValidationGroup="allval">Subject is required.</asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="SubjectRange" runat="server" ClientValidationFunction="validateInputRange" OnServerValidate="SubjectRange_ServerValidate"
                                    ControlToValidate="Subject" ErrorMessage="Subject's length cannot exceed 100 characters." Display="Dynamic" 
                                    ValidationGroup="allval">Subject's length cannot exceed 100 characters.</asp:CustomValidator>
</td>
</tr>
<tr>
<td align=center colspan=2>A brief title about the specific issue/suggestion you are submitting.</td>
</tr>
<tr>
<td align=right><label for="Description">Description:</label><br /><span>At most 1000 characters</span></td>
<td align=left>
<asp:TextBox ID="Description" runat="server" MaxLength="100" style="width:350px;" TextMode="MultiLine" Rows=5></asp:TextBox>
<asp:RequiredFieldValidator ID="DescriptionRequired" runat="server"
Display="Dynamic" ControlToValidate="Description" ErrorMessage="Description is required." 
                                    ValidationGroup="allval">Description is required.</asp:RequiredFieldValidator>
                                                                    <asp:CustomValidator ID="DescriptionRange" runat="server" ClientValidationFunction="validateInputRange" OnServerValidate="DescriptionRange_ServerValidate"
                                    ControlToValidate="Description" ErrorMessage="Description's length cannot exceed 1000 characters." Display="Dynamic" 
                                    ValidationGroup="allval">Description's length cannot exceed 1000 characters.</asp:CustomValidator>
</td>
</tr>
<tr>
<td align=center colspan=2>Detailed description of the issue/suggestion.</td>
</tr>
<tr>
<td align="center" colspan="2" style="padding: 15px 0 0 0;">
<asp:Button ID="Submit" runat="server" Text="Submit" ValidationGroup="allval" OnClick="submit_Click" />
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
