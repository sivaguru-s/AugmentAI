<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AdminMaintenance.aspx.vb" Inherits="EPay.AdminMaintenance" Trace="false" ValidateRequest="false"%>

<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server" >
    <title>EPay Admin Maintenance</title>   
    <asp:PlaceHolder ID="phStylesAndScripts" runat="server" /> 
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
    <script type="text/javascript">
        function Invoice(Cusno, Shpno, Invno, Invdate, Ponum, Ordno, Ordate) {
            var URL = "../../DealerProfilesNET/InvoiceDetail.aspx?MenuReadOnly=1&txtshpno=" + Shpno + "&txtinvno=" + Invno + "&txtinvdate=" + Invdate + "&txtordno=" + Ordno + "&txtorddt=" + Ordate
            window.open(URL, 'InvoiceDetail', 'toolbar=no,titlebar=no,menubar=no,directories=no,location=no,status=no,resizable=yes,copyhistory=no,scrollbars=yes,top=50,left=1,width=700,height=500,fullscreen=no');
        }
    </script> 
    
    <script type="text/javascript">
        $(document).ready(function () {
            var totalRows = $('#<%=gvInvoices.ClientID%> tr').length;
            var pageNo = Number('<%=gvInvoices.PageIndex%>')
            if (totalRows > 5)
            if (totalRows <= 500 &&  pageNo == 0) {
	                gridviewSinglerowScroll();
	            }

	            else {
	                gridviewDoublerowScroll();
	            }
        });
	    


        function gridviewSinglerowScroll() {

            $('#gvInvoices').gridviewScroll({
                width: 980,
                height: 400,
                headerrowcount: 1,
                arrowsize: 30,
                railsize: 15,
                barsize: 10,
                varrowtopimg: "/Images/arrowvt.png",
                varrowbottomimg: "/Images/arrowvb.png",
                harrowleftimg: "/Images/arrowhl.png",
                harrowrightimg: "/Images/arrowhr.png"

            });

            gvStyles();
        }

        function gridviewDoublerowScroll() {

            $('#gvInvoices').gridviewScroll({
                width: 980,
                height: 400,
                headerrowcount: 2,
                arrowsize: 30,
                railsize: 15,
                barsize: 10,
                varrowtopimg: "/Images/arrowvt.png",
                varrowbottomimg: "/Images/arrowvb.png",
                harrowleftimg: "/Images/arrowhl.png",
                harrowrightimg: "/Images/arrowhr.png"

            });

            gvStyles();
        }



	    function gvStyles() {

	        var Timg = document.getElementById('gvInvoicesVertical_TIMG');
	        var Bimg = document.getElementById('gvInvoicesVertical_BIMG');
	        var HBar = document.getElementById('gvInvoicesVerticalRail');
	        var VBar = document.getElementById('gvInvoicesHorizontalRail');
	        var Limg = document.getElementById('gvInvoicesHorizontal_LIMG');
	        var Rimg = document.getElementById('gvInvoicesHorizontal_RIMG');

	        if (HBar) {
	            var BarHeight = parseInt(HBar.clientHeight) + parseInt(30);
	            var Himgtop = parseInt(HBar.clientHeight) + parseInt(60);


	            if (HBar.style.display != "none") {
	                if (Timg) {
	                    Timg.style.cssText = "height: 30px; position: absolute; z-index: 0; top: 0px; right: 0px;background-color: #F0F0F0";
	                }



	                if (Bimg) {
	                    Bimg.style.cssText = "height: 30px; position: absolute; z-index: 0; right: 0px; top:" + parseInt(BarHeight) + "px;background-color: #F0F0F0";


	                }
	            }
	        }
	        if (VBar) {
	            if (VBar.style.display != "none") {
	                if (Limg) {
	                    Limg.style.cssText = "width: 30px; position: absolute; top: " + parseInt(Rimg.style.top) + "px; z-index: 0; left: 0px;";
	                }
	            }
	        }
	    }

	</script>
	
           
</head>
<body>
<form id="form1" runat="server" >
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <awr:ResponsiveADHeader ID="ResponsiveADHeader" runat="server" MenuCollapsed="true" HideQuickSearch="true" HomeLinkClosesPage="true" HideMenu="true" SelectedAccountReadOnly="true" />
    <div class="BodyIndent" style="width:980px; margin: 0 auto;">
        <div>
            <h1 style="text-align: center">E-Payment Admin Maintenance</h1>
            <custom:nav ID="Navigation" runat="server" />
        </div>
        <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" EnableViewState="False" ></asp:Label>
        <br class="BlankLine" />
        <div class="ContentHeader" style="text-align: left;">Search Criteria</div>
        <div class="ContentBackground" style="text-align: left;">
            <asp:Table ID="tblSearchUsers" runat="server">
                <asp:TableRow style="text-align:left;">
                    <asp:TableCell style="text-align:left; width:100px;">&nbsp;</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:75px;">&nbsp;</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:160px;"><asp:RadioButton ID="rdbAllCustomers" runat="server" GroupName="CustomerSelection" Text="All Customers" /></asp:TableCell>             
                </asp:TableRow>
                <asp:TableRow style="text-align:left;">             
                    <asp:TableCell style="text-align:left; width:100px;"><asp:Label ID="lblCreditTerritory" runat="server" Text="Credit Territory:"></asp:Label></asp:TableCell>
                    <asp:TableCell style="text-align:left; width:75px;"><asp:DropDownList ID="lstCreditTerritory" runat="server" Width="45px"></asp:DropDownList></asp:TableCell>
                    <asp:TableCell style="text-align:left; width:160px;"><asp:RadioButton ID="rdbSelectedCustomer" runat="server" GroupName="CustomerSelection" Text="Selected Customer" /> </asp:TableCell>
                    <asp:TableCell style="text-align:left; width:87px;">
                        <asp:Button ID="cmdSearch" Text="Search" runat="server" Width="80px" Height="25px" /></asp:TableCell>
                </asp:TableRow>                
            </asp:Table>
        </div>
        <br class="BlankLine" />     
        <div class="ContentBackground" style="width: 100%;">
            <table class="ContentHeader" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr class="ContentHeader">
                    <td class="ContentHeader" style="width: 500px;">
                        <asp:Label ID="lblPaymentsForSpecifiedCustomer" runat="server" Text="Unconfirmed payments for Acct #:"></asp:Label>
                    </td>
                    <td class="ContentHeader" style="width: 150px;">
                        <asp:Button ID="cmdDelete" runat="server" Text="Delete" Width="100px" Height="21px" />
                    </td>
                    <td class="ContentHeader" align="left" style="width: 600px;vertical-align:middle;">
                        <asp:ImageButton ID="cmdExportToExcel" runat="server" ImageUrl="/images/ExportToExcel.gif" Height="19px" Width="100px" />
                    </td>
                </tr>
            </table>
         <div style="height:400px">
                <asp:GridView ID="gvInvoices" runat="server" 
                        CssClass="GroupContentBackground" AllowSorting="true" AllowPaging="true" Width="100%"
                        EmptyDataText="No Invoices To Display" AutoGenerateColumns="false" PageSize="500">
                    <EmptyDataRowStyle CssClass="ContentBackground" HorizontalAlign="Center" VerticalAlign="Top" />
                    <HeaderStyle CssClass="GridHeaderNative GridHeaderLock" BackColor="WhiteSmoke" ForeColor="Black" />
                    <RowStyle CssClass="GridMainRow" />
                    <AlternatingRowStyle CssClass="GridAlternateRow" />
                    <PagerStyle  BackColor="WhiteSmoke" HorizontalAlign="Center" />
                    <PagerSettings NextPageText="Next" PreviousPageText="Prev" Mode="Numeric" Position="Top" Visible="true" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderStyle Height="20px" />
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelectAll" runat="server" OnCheckedChanged="SelectAllCheckboxes" AutoPostBack="true" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="overflow:hidden;height:18px;">
                                    <asp:CheckBox ID="chkSelect" runat="server" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="epyStatus">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridStatusHeader" runat="server" Text="Status" CommandName="Sort" CommandArgument="epyStatus" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Hyperlink ID="hplGridStatus" runat="server" Text='<%#Eval("epyStatus")%>'  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblGridRefNo" runat="server" Text='<%#Eval("epyReferenceNumber")%>' Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="epyCustomerNo">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridCustomerNumberHeader" runat="server" Text="Customer #" CommandName="Sort" CommandArgument="epyCustomerNo" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridCusNo" runat="server" Text='<%#Eval("epyCustomerNo")%>'  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="epyInvoiceNumber">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridInvoiceNumberHeader" runat="server" Text="Invoice #" CommandName="Sort" CommandArgument="epyInvoiceNumber" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Hyperlink ID="hplGridInvoiceNumber" runat="server" Text='<%#Eval("epyInvoiceNumber")%>'  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="opicrmnr">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridCreditNumberHeader" runat="server" Text="Credit #" CommandName="Sort" CommandArgument="opicrmnr" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Hyperlink ID="hplGridCreditNumber" runat="server" Text='<%#Eval("opicrmnr")%>'  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="epyShpNo">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridShipToHeader" runat="server" Text="ShipTo" CommandName="Sort" CommandArgument="epyShpNo" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridShipTo" runat="server" Text='<%#Eval("epyShpNo")%>'  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="inhDagedt">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridInvoiceDateHeader" runat="server" Text="Inv Date" CommandName="Sort" CommandArgument="inhDagedt" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridInvDate" runat="server" Text='<%#Eval("inhDagedt", "{0:M/dd/yyyy}") %>'  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="inhOrdno">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridOrderNumberHeader" runat="server" Text="Order #" CommandName="Sort" CommandArgument="inhOrdno" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridOrderNumber" runat="server" Text='<%#Eval("inhOrdno")%>'  />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblGridOrderDate" runat="server" Text='<%#Eval("inhdordda", "{0:M/dd/yyyy}")%>' Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="inhTripNo">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridTripNumberHeader" runat="server" Text="Trip #" CommandName="Sort" CommandArgument="inhTripNo" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridTripNumber" runat="server" Text='<%#Eval("inhTripNo")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="inhShipInstructions">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridRPPNumberHeader" runat="server" Text="RPP #" CommandName="Sort" CommandArgument="inhShipInstructions" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridRPPNumber" runat="server" Text='<%#Eval("inhShipInstructions")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="inhPoNum">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridPONumberHeader" runat="server" Text="PO #" CommandName="Sort" CommandArgument="inhPoNum" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridPONumber" runat="server" Text='<%#Eval("inhPoNum")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="inhInvAm" ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridInvoiceAmountHeader" runat="server" Text="Inv Amt" CommandName="Sort" CommandArgument="inhInvAm" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridInvoiceAmount" runat="server" Text='<%#Eval("inhInvAm")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="opiTtlcr" ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridAmountPaidHeader" runat="server" Text="Amt Pd" CommandName="Sort" CommandArgument="opiTtlcr" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridAmountPaid" runat="server" Text='<%#Eval("opiTtlcr")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="opiOpamt" ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridBalanceHeader" runat="server" Text="Balance" CommandName="Sort" CommandArgument="opiOpamt" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridBalance" runat="server" Text='<%#Eval("opiOpamt")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="opicatcd" ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridCodeHeader" runat="server" Text="Code" CommandName="Sort" CommandArgument="opicatcd" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridCode" runat="server" Text='<%#Eval("opicatcd")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField SortExpression="Days" ItemStyle-HorizontalAlign="Right">
                            <HeaderTemplate>
                                <asp:LinkButton ID="lbnGridDaysHeader" runat="server" Text="Days" CommandName="Sort" CommandArgument="Days" ForeColor="Black" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblGridDays" runat="server" Text='<%#Eval("Days")%>'/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
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
        </div>
        </div>
    <awr:ResponsiveADFooter ID="ResponsiveADFooter" runat="server" />
</form>
</body>
</html>
