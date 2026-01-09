<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AnalystReport.aspx.vb" Inherits="EPay.AnalystReport" trace="false"%>

   
<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Epay Analyst Report</title>
    <asp:PlaceHolder ID="phStylesAndScripts" runat="server" /> 
    <script type="text/javascript"  src="/Scripts/DateValidation.js"></script>
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
    <script type="text/javascript">
          $(function () {
             $("#<%=txtDate.ClientID%>").datepicker({
                 buttonImage: '/images/downArrow.gif', buttonImageOnly: true, changeMonth: true, changeYear: true,
                 showOn: 'both', defaultDate: '0D',
                 changeMonth: true, changeYear: true, showOtherMonths: true, selectOtherMonths: true, showButtonPanel: true,
                 onSelect: function (dateText, inst) { $("#<%=txtDate.ClientID%>").val(dateText); $(this).blur(); }
             }).keyup(function (e) { datePicker_keyUp(this, e); })
         });
     </script>
		<style type="text/css">
	.ui-datepicker-trigger {
    cursor: pointer!important;
    margin-top:-1px!important;
	margin-left:-17px !important;
    vertical-align:middle!important;
	height:19px !important;
	
    }
	</style>  

       <script type="text/javascript">
	   function isNumeric(evt){
			evt = (evt) ? evt : window.event;
			charcode = (evt.which) ? evt.which : evt.keyCode;		
			if (charcode > 31 && (charcode < 48 || charcode >57 ))
			return false;
			else
			return true;
	   }
	   function isAlphaNumeric(evt) {

	       evt = (evt) ? evt : window.event;
	       charcode = (evt.which) ? evt.which : evt.keyCode;
	       if ((charcode >= 48 && charcode <= 57) || (charcode >= 65 && charcode <= 90) || (charcode >= 97 && charcode <= 122) || (charcode == 8) || (charcode == 46) || charcode == 13)
	           return true;
	       else
	           return false;
	   }
			</script>
			<script type="text/javascript">
           $(document).ready(function () {
               var totalRows = $('#<%=grvReport.ClientID%> tr').length;
               var pageNo = Number('<%=grvReport.PageIndex%>')
               if (totalRows > 5)
                   if (totalRows <= 500 && pageNo == 0) {
                       gridviewSinglerowScroll();
                   }

                   else {
                       gridviewDoublerowScroll();
                   }
        });

           function gridviewSinglerowScroll() {

            $('#grvReport').gridviewScroll({
                width: 800,
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

               $('#grvReport').gridviewScroll({
                   width: 800,
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

            var Timg = document.getElementById('grvReportVertical_TIMG');
            var Bimg = document.getElementById('grvReportVertical_BIMG');
            var HBar = document.getElementById('grvReportVerticalRail');
            var VBar = document.getElementById('grvReportHorizontalRail');
            var Limg = document.getElementById('grvReportHorizontal_LIMG');
            var Rimg = document.getElementById('grvReportHorizontal_RIMG');

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
<form id="AnalystForm" runat="server">
    <awr:ResponsiveADHeader ID="ResponsiveADHeader" runat="server" MenuCollapsed="true" HideQuickSearch="true" HomeLinkClosesPage="true" HideMenu="true" SelectedAccountReadOnly="true" />
    <div class="BodyIndent" style="width:800px; margin:0 auto;">
        <div>
            <h1 style="text-align: center">E-Payment Report</h1>
            <custom:nav id="Navigation" runat="server"/>
        </div>
        <div style="text-align:center;">
            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" EnableViewState="False" ></asp:Label>
        </div>
        <div class="ContentBackground">
            <div class="ContentHeader" style="text-align: left">Enter Search Criteria and Click Search</div>
            <asp:Table ID="tblSearchInvoices" runat="server" >
                <asp:TableRow style="text-align:left;">
                  <asp:TableCell>Territory:&nbsp;</asp:TableCell>
                  <asp:TableCell><asp:DropDownList ID="lstTerritory" runat="server"></asp:DropDownList>
                          </asp:TableCell>                  
                  <asp:TableCell>Customer #:&nbsp;</asp:TableCell>
                  <asp:TableCell><asp:TextBox ID="txtCustomerNumber" runat="server" Width="70px" onKeypress="return isAlphaNumeric(event)" onpaste="return false" CssClass="input-xs"></asp:TextBox>
                          </asp:TableCell>
                  <asp:TableCell>Check #:&nbsp;</asp:TableCell>
                  <asp:TableCell><asp:TextBox ID="txtConfirmationNo" runat="server" Width="75px" onKeypress="return isAlphaNumeric(event)" onpaste="return false" CssClass="input-xs"></asp:TextBox></asp:TableCell>
                  <asp:TableCell>Ref #:&nbsp;</asp:TableCell>
                  <asp:TableCell><asp:TextBox ID="txtReferenceNo" runat="server" Width="75px" onKeypress="return isNumeric(event)" onpaste="return false" MaxLength ="9" CssClass="input-xs"></asp:TextBox></asp:TableCell>                
                  <asp:TableCell style="text-align:left;"> <asp:TextBox ID="txtDate" runat="server" Width="120px" oncopy="return false" MaxLength="10" oncut="return false" onpaste="return false" CssClass="input-xs"></asp:TextBox></asp:TableCell>
                                    
                  <asp:TableCell style="text-align:right;">
                         &nbsp;<asp:Button ID="cmdSearch" Text="Search" runat="server" />
                  </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
        <div>  
            <br class="BlankLine" />    
            <div style="height:400px">

                <asp:GridView ID="grvReport" runat="server" CssClass="GridViewCSSScrollable" Width="100%" AutoGenerateColumns="false"  GridLines="Both" AllowPaging="true" PageSize="500"  AllowSorting="true" EmptyDataText="No Data To Display" 
                    OnPageIndexChanging="grvReport_PageIndexChanging" OnSorting="grvReport_Sorting">
                        <EmptyDataRowStyle CssClass="ContentBackground" HorizontalAlign="Center" VerticalAlign="Top" Font-Italic="true" BorderWidth="0" />
                        <HeaderStyle CssClass="GridHeaderNative GridHeaderLock" BackColor="WhiteSmoke" ForeColor="Black" Height="25" />
                        <RowStyle CssClass="GridMainRow" />
                     <AlternatingRowStyle CssClass="GridAlternateRow" />
                      <PagerStyle BackColor="WhiteSmoke" HorizontalAlign="Center" />
                    <PagerSettings NextPageText="Next" PreviousPageText="Prev" Mode="Numeric" Position="Top" Visible="true" />
                    <Columns>
                         <asp:TemplateField SortExpression ="dtea">	
                             <HeaderTemplate>                                
                                 <asp:LinkButton ID="lbtnDateEnteredHeader" runat="server" Text ="Date Entered" CommandName ="Sort" CommandArgument ="dtea" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblDateEntered" runat="server" Text='<%#Eval("dtea","{0:M/dd/yyyy}")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="CustomerNumber">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnCustomerNumberHeader" runat="server" Text ="Customer" CommandName ="Sort" CommandArgument ="CustomerNumber" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblCustomerNumber" runat="server" Text='<%#Eval("CustomerNumber")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="Shipno">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnShipno" runat="server" Text ="Shipno" CommandName ="Sort" CommandArgument ="Shipno" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblShipno" runat="server" Text='<%#Eval("Shipno")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="EpayStatus">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnStatusHeader" runat="server" Text ="Status" CommandName ="Sort" CommandArgument ="EpayStatus" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblStatus" runat="server" Text='<%#Eval("EpayStatus")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="fileType">
                             <HeaderTemplate>                               
                                 <asp:LinkButton ID="lbtnfileTypeHeader" runat="server" Text ="File" CommandName ="Sort" CommandArgument ="fileType" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <ItemTemplate>
                                   <asp:Label ID="lblfileType" runat="server" Text='<%#Eval("fileType")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>


                           <asp:TemplateField SortExpression ="GrossAmount">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnShipnoHeader" runat="server" Text ="Gross Amount" CommandName ="Sort" CommandArgument ="GrossAmount" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblShipno" runat="server" Text='<%#Eval("GrossAmount","{0:c}")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Right" />
                         </asp:TemplateField>

                           <asp:TemplateField SortExpression ="discount">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnDiscountHeader" runat="server" Text ="Discount" CommandName ="Sort" CommandArgument ="Shipno" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblDiscount" runat="server" Text='<%#Eval("discount","{0:c}")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Right" />
                         </asp:TemplateField>

                           <asp:TemplateField SortExpression ="AmountCharged">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnAmtPaidHeader" runat="server" Text ="Amount Paid" CommandName ="Sort" CommandArgument ="AmountCharged" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblShipno" runat="server" Text='<%#Eval("AmountCharged","{0:c}")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Right" />
                         </asp:TemplateField>

                           <asp:TemplateField SortExpression ="ConfirmationNumber">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnConfnoHeader" runat="server" Text ="Conf #" CommandName ="Sort" CommandArgument ="ConfirmationNumber" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblConfno" runat="server" Text='<%#Eval("ConfirmationNumber")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>

                           <asp:TemplateField SortExpression ="ReferenceNumber">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnRefnoHeader" runat="server" Text ="Ref #" CommandName ="Sort" CommandArgument ="ReferenceNumber" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblRefno" runat="server" Text='<%#Eval("ReferenceNumber")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>

                           <asp:TemplateField SortExpression ="CreditTerritory">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnTerrHeader" runat="server" Text ="Terr" CommandName ="Sort" CommandArgument ="CreditTerritory" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblTerr" runat="server" Text='<%#Eval("CreditTerritory")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>

                     </Columns>
                   
                    </asp:GridView>
                </div>
        
        </div>  
    </div>
    <awr:ResponsiveADFooter ID="ResponsiveADFooter" runat="server" />
    <asp:HiddenField ID="hdnSortColumn" runat="server" Visible="False" />
    <asp:HiddenField ID="hdnSortAscending" runat="server" Visible="False" />
</form>
</body>
</html>
