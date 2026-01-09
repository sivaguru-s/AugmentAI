<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserListPrintablePage.aspx.vb" Inherits="EPay.UserListPrintablePage" Trace="false" ValidateRequest="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>EPay User List Report</title>
</head>
<body>
    <form id="frmUserList" runat="server">
        <center>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="lblCompany" runat="server" Text="Ashley Furniture Industries, Inc." Font-Size="16" Font-Bold="true" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblReportTitle" runat="server" Text="lblReportTitle" Font-Size="16" Font-Bold="true" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <br />
                        <table>
                            <tr>
                                <td style="text-decoration:underline;">
                                Acct #:
                                </td>
                                <td>
                                &nbsp;<asp:Label ID="lblSearchedAccountNumber" runat="server" Text="Acct #" />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-decoration:underline;">
                                    Acct Name:
                                </td>
                                <td>
                                    &nbsp;<asp:Label ID="lblSearchedAccountName" runat="server" Text="Acct Name" />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-decoration:underline;">
                                    Bill-To-State:
                                </td>
                                <td>
                                    &nbsp;<asp:Label ID="lblSearchedBillToState" runat="server" Text="Bill-To-State" />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-decoration:underline;">
                                    Territory:
                                </td>
                                <td>
                                    &nbsp;<asp:Label ID="lblSearchedTerritory" runat="server" Text="Territory" />
                                </td>
                            </tr>
                            <tr>
                                <td style="text-decoration:underline;">
                                    Terms Code:
                                </td>
                                <td>
                                    &nbsp;<asp:Label ID="lblSearchedTermsCode" runat="server" Text="Terms Code" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <input id="btnPrint" type="button" value="Print" onclick="window.print()" style="width: 100px;" />
                        <br />
                        <asp:GridView ID="grdUsers" runat="server" AutoGenerateColumns="False">
                            <Columns>                    
                                <asp:BoundField DataField="Acct #" HeaderText="Acct #">
                                    <ItemStyle  HorizontalAlign="Center" Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Acct Name" HeaderText="Acct Name" >
                                    <ItemStyle HorizontalAlign="Center" Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Bill-To-State" HeaderText="Bill-To-State">
                                    <ItemStyle HorizontalAlign="Center" Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Territory" HeaderText="Territory" >
                                    <ItemStyle HorizontalAlign="Center" Wrap="False" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Terms Code" HeaderText="Terms Code" >
                                    <ItemStyle HorizontalAlign="Center" Wrap="False" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </center>
    </form>
</body>
</html>
