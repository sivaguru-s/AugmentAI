<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TestRTPConf.aspx.vb" Inherits="EPay.RTPConf" Trace="true"
 %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </div>
        <br />
        <asp:RadioButtonList ID="envRadio" runat="server">
            <asp:ListItem Selected="True">Dev</asp:ListItem>
            <asp:ListItem>Stage</asp:ListItem>
            <asp:ListItem>Prod</asp:ListItem>
        </asp:RadioButtonList>
        Ref: <asp:TextBox ID="txtRefNum" runat="server"></asp:TextBox><br />
        Conf: <asp:TextBox ID="txtConf" runat="server"></asp:TextBox>
        <asp:Button ID="Sumbit" runat="server" Text="Sumbit" />
    </form>
</body>
</html>
