<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewPayment.aspx.vb" Inherits="EPay.ViewPayment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Epay Payment</title>
    <asp:PlaceHolder ID="phStylesAndScripts" runat="server" /> 
    <script type="text/javascript" src="Scripts/Common.js"></script>
    <script type='text/javascript' src='Scripts/x.js'></script>
    <script type='text/javascript' src='Scripts/xtableheaderfixed.js'></script> 
    <script type='text/javascript'>
        xAddEventListener(window, 'load',
            function () {
                new xTableHeaderFixed
            ('GridViewCSSScrollable', 'divGridContainer', 0);
            }, false);
    </script>
    <style type="text/css">
       #divGridContainer {
          position: relative;
          height: 350px;
          overflow: auto;
        }

        GridViewCSSScrollable {
          text-align: center;
        }
    </style>
</head>
<body>
    <form id="frmViewPayment" runat="server">
        <awr:ResponsiveADHeader ID="ResponsiveADHeader" runat="server" HideMenu="true" HidePageHeader="true" />
        <div style="width:425px;margin:0 auto;float:none;">
            <div style="text-align: center;">
                <asp:Label ID="lblMessage" runat="server" CssClass="AlertText" Visible="false" />
            </div>
            <div class="ContentBackground" style="padding: 2px;">
                <div class="ContentHeader" style="padding: 2px;">
                    <div style="float:left;">
                        Reference Number:&nbsp;
                        <asp:Label ID="lblReferenceNumber" runat="server" />&nbsp;/
                        <asp:Label ID="lblType" runat="server" />
                    </div>
                    <div style="text-align:right;"><asp:Button ID="btnExportToExcel" runat="server" Text="Export To Excel" /></div>
                </div>
                <div id="divGridContainer" runat="server" style="width:100%;margin-top:5px">
                    <asp:GridView ID="grvViewPayment" runat="server" AutoGenerateColumns="false" CssClass="GridViewCSSScrollable" 
                            Width="99%" GridLines="None" BorderWidth="0" AllowSorting="true">
                        <EmptyDataRowStyle CssClass="ContentBackground" HorizontalAlign="Center" VerticalAlign="Top" Font-Italic="true" />
                        <HeaderStyle CssClass="GridHeaderNative,GridHeaderLock" BackColor="WhiteSmoke" ForeColor="Black" Height="25" />
                        <RowStyle CssClass="GridMainRow" />
                        <AlternatingRowStyle CssClass="GridAlternateRow" />
                        <Columns>
                            <asp:BoundField DataField="InvoiceNumber" SortExpression="InvoiceNumber" HeaderText="Invoice">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="GrossAmount" SortExpression="GrossAmount" HeaderText="Gross">
                                <HeaderStyle HorizontalAlign="Right" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Discount" SortExpression="Discount" HeaderText="Discount">
                                <HeaderStyle HorizontalAlign="Right" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="AmountCharged" SortExpression="AmountCharged" HeaderText="Net">
                                <HeaderStyle HorizontalAlign="Right" />
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
                <br />
                <div style="text-align:center;">
                    <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="window.close();" />
                </div>
            </div>
        </div>
        <awr:ResponsiveADFooter ID="ResponsiveADFooter" runat="server" />
    </form>
</body>
</html>
