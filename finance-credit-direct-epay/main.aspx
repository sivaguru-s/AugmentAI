<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="main.aspx.vb" Inherits="EPay.main" Trace="false" ValidateRequest="false"%>

<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>EPay</title>
    <asp:PlaceHolder ID="phStylesAndScripts" runat="server" /> 
    <script type="text/javascript"  src="/Scripts/DateValidation.js"></script>
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
    <script type="text/javascript" src="Scripts/Main.js?20161011"></script>
    <script type="text/javascript" src="Scripts/InvoiceSelection.js?20250306"></script>
	<style type="text/css">
	    .ui-datepicker-trigger {
            cursor: pointer!important;
            margin-top:-1px!important;
	        margin-left:-17px !important;
            vertical-align:middle!important;
	        height:19px !important;
        }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <awr:ResponsiveADHeader ID="ResponsiveADHeader" runat="server" MenuCollapsed="true" HideQuickSearch="true" HomeLinkClosesPage="true" HideMenu="true" SelectedAccountReadOnly="true" />
    <div class="BodyIndent" style="width:775px; margin: 0 auto;">
        <h1 style="text-align: center">E-Payment</h1>
        <custom:nav id="Navigation" runat="server"/> <%--AppAuthorization=<%=AppAuthorization %>--%>

        <asp:Label ID="lblNoAccountSelected" Text="Please select an account" runat="server" Visible="false" ForeColor="red"></asp:Label>
        <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" EnableViewState="False" ></asp:Label>        
        <div class="ContentBackground">
            <div class="ContentHeader">Search Invoices </div>
            <asp:Table ID="tblSearchInvoices" runat="server">
                <asp:TableRow style="text-align:left;">
                <asp:TableCell style="text-align:right;">Invoice #:&nbsp;</asp:TableCell>
                                                                                    <asp:TableCell style="text-align:left;"><asp:TextBox ID="txtInvoiceNumber" runat="server" Width="90px" CssClass="input-xs"></asp:TextBox>
                    </asp:TableCell>                
                
                <asp:TableCell>Credit #:&nbsp;</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="txtCreditNumber" runat="server" Width="90px" CssClass="input-xs"></asp:TextBox></asp:TableCell>                
                <asp:TableCell>PO #:&nbsp;</asp:TableCell>
                <asp:TableCell><asp:TextBox ID="txtPONumber" runat="server" Width="150px" CssClass="input-xs"></asp:TextBox></asp:TableCell>         
                </asp:TableRow>                
                <asp:TableRow>
                <asp:TableCell> &nbsp;</asp:TableCell>
                <asp:TableCell ColumnSpan="2" style="text-align:right;">Invoice Date:&nbsp;</asp:TableCell>
                <asp:TableCell  style="text-align:left;"><asp:TextBox ID="txtFromDate" runat="server" Width="120px" oncopy="return false" MaxLength="10" oncut="return false" onpaste="return   false" CssClass="input-xs"></asp:TextBox></asp:TableCell>              
                <asp:TableCell style="text-align:center;">to &nbsp;</asp:TableCell>
                <asp:TableCell style="text-align:left;"> <asp:TextBox ID="txtToDate" runat="server" Width="112px"  oncopy="return false" MaxLength="10" oncut="return false" onpaste="return false" CssClass="input-xs"></asp:TextBox></asp:TableCell>
                
                <asp:TableCell ColumnSpan="2" style="text-align:right;">
                                <asp:Button ID="cmdSearch" Text="Search" runat="server" /></asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>        
        <br class="BlankLine" />
        <div class="ContentBackground" style="width: 900px;margin:0 auto;display :table">
            <table class="ContentHeader" cellpadding="0" cellspacing="0">
                <tr class="ContentHeader">
                    <td class="ContentHeader" style="width: 500px;">
                        Customer Invoices From&nbsp;
                        <asp:Label ID="lblInvoiceDate" runat="server" Text="lblInvoiceDate"></asp:Label>
                        To&nbsp;
                        <asp:Label ID="lblToDate" runat="server" Text="lblToDate"></asp:Label> 
                    </td>
                    <td class="ContentHeader" style="width:150px;">
                        <asp:Button ID="cmdPayment" runat="server" Text="Make Payment" />
                    </td>
                    <td class="ContentHeader" style="width:250px;">
                        <asp:CheckBox ID="chkShowAllInvoices" runat="server" AutoPostBack="true" Text="Show All Invoices" />
                    </td>
                    <td Class="ContentHeader" style="width:250px;">
                        <asp:ImageButton ID = "cmdExportToExcel" runat="server" ImageUrl="/images/ExportToExcel.gif" Height="19px" />
                    </td>
                </tr>
            </table>
            <div id = "pgritm" style="text-align:center;">
                <asp:Repeater ID = "rptPager" runat="server">
                    <ItemTemplate>
                                                                                                                    <asp:LinkButton ID = "lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>' Enabled='<%# Eval("Enabled") %>' 
                            Font-Bold="true"    ForeColor='<%# IIf(Eval("Enabled"), System.Drawing.Color.SteelBlue, System.Drawing.Color.Black)%>'  OnClick = "Page_Changed" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div id="grview" runat="server" >
                <asp:GridView ID="gvInvoices" runat="server" Width="100%" AllowSorting="true"
                        GridLines="both" EmptyDataText="No Data To Display" AutoGenerateColumns="false"
                        EnableViewState="false">
                    <EmptyDataRowStyle CssClass="ContentBackground" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle CssClass="GridHeaderNative,GridHeaderLock" BackColor="WhiteSmoke" ForeColor="Black" />
                    <RowStyle CssClass="GridMainRow" />
                    <AlternatingRowStyle CssClass="GridAlternateRow" />
                   <PagerStyle BackColor="WhiteSmoke" HorizontalAlign="Center" />
                    <PagerSettings  Position="Top" Visible="true" />
                    <Columns>
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <HeaderStyle Height="20px" />
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelectAll" runat="server" OnCheckedChanged="SelectAllCheckboxes" AutoPostBack="true" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="overflow:hidden;height:18px;">
                                    <asp:CheckBox ID="chkSelect" runat="server" />
                                </div><asp:HiddenField ID="hdnGridPayInfo" runat="server" Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridStatusHeader" runat="server" CommandName="Sort" CommandArgument="EpayStatus" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Hyperlink ID="hplGridStatus" runat="server"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridInvoiceNumberHeader" runat="server" CommandName="Sort" CommandArgument="opiinvno" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Hyperlink ID="hplGridInvoiceNumber" runat="server"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridCreditNumberHeader" runat="server" CommandName="Sort" CommandArgument="opicrmnr" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Hyperlink ID="hplGridCreditNumber" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridShipToHeader" runat="server" CommandName="Sort" CommandArgument="opishpno" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridShipTo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridInvoiceDateHeader" runat="server" CommandName="Sort" CommandArgument="opiDagedt" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridInvDate" runat="server"  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridOrderNumberHeader" runat="server" CommandName="Sort" CommandArgument="opiOrdno" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridOrderNumber" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridTripNumberHeader" runat="server" CommandName="Sort" CommandArgument="inhTripNo" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridTripNumber" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridRPPNumberHeader" runat="server" CommandName="Sort" CommandArgument="[RPP #]" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridRPPNumber" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridPONumberHeader" runat="server" CommandName="Sort" CommandArgument="opiponum" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridPONumber" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridInvoiceAmountHeader" runat="server" CommandName="Sort" CommandArgument="opiInvam" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridInvoiceAmount" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridAmountPaidHeader" runat="server" CommandName="Sort" CommandArgument="opiTtlcr" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridAmountPaid" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridBalanceHeader" runat="server" CommandName="Sort" CommandArgument="opiOpamt" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridBalance" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridCodeHeader" runat="server" CommandName="Sort" CommandArgument="opicatcd" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridCode" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lkbGridDaysHeader" runat="server" CommandName="Sort" CommandArgument="Days" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridDays" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>                      
        <!-- <asp:HiddenField ID="hdnCustomerNumber" runat="server" Visible="False" />
        <asp:HiddenField ID="hdnShiptoNumber" runat="server" Visible="False" />
        <asp:HiddenField ID="hdnAllShiptos" runat="server" Visible="False" />
        <asp:HiddenField ID="hdnSecurityMHS" runat="server" Visible="False" />
        <asp:HiddenField ID="hdnShowCredits" runat="server" Visible="False" />
        <asp:HiddenField ID="hdnShowOldCredits" runat="server" Visible="False" />
        <asp:HiddenField ID="hdnShowPricing" runat="server" Visible="False" /> 
         -->
        <asp:HiddenField ID="hdnSortColumn" runat="server" Visible="False" />
        <asp:HiddenField ID="hdnSortAscending" runat="server" Visible="False" />

        <!-- Hidden field to store selected invoice data (replaces ViewState for checkbox selections) -->
        <asp:HiddenField ID="hdnSelectedInvoices" runat="server" />

        <!-- Initialize JavaScript invoice selection manager -->
        <script type="text/javascript">
            $(document).ready(function () {
                // Initialize the invoice selection manager
                InvoiceSelection.init(
                    '<%= hdnSelectedInvoices.ClientID %>',
                    '<%= gvInvoices.ClientID %>'
                );
            });
        </script>
        </div>
    <awr:ResponsiveADFooter ID="ResponsiveADFooter" runat="server" />
</form>
</body>
</html>
