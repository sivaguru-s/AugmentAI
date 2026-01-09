<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="History.aspx.vb" Inherits="EPay.History"  Trace="false" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>History</title>
    <asp:PlaceHolder ID="phStylesAndScripts" runat="server" /> 
    <script type="text/javascript" src="Scripts/Common.js"></script>
    <script type='text/javascript'>
      			function isNumber(evt){
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
			    if ((charcode >= 48 && charcode <= 57) || (charcode >= 65 && charcode <= 90) || (charcode >= 97 && charcode <= 122) || (charcode == 8) || (charcode == 46))
			        return true;
			    else
			        return false;
			}
    </script>
  
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
    		<script type="text/javascript">
    		    $(document).ready(function () {
    		        var totalRows = $('#<%=grvEpayHistory.ClientID%> tr').length;
               if (totalRows > 5)
                 
                       gridviewScroll();
                 
           });

    		    function gridviewScroll() {

               $('#grvEpayHistory').gridviewScroll({
                   width: 750,
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

               function gvStyles() {

               var Timg = document.getElementById('grvEpayHistoryVertical_TIMG');
               var Bimg = document.getElementById('grvEpayHistoryVertical_BIMG');
               var HBar = document.getElementById('grvEpayHistoryVerticalRail');
               var VBar = document.getElementById('grvEpayHistoryHorizontalRail');
               var Limg = document.getElementById('grvEpayHistoryHorizontal_LIMG');
               var Rimg = document.getElementById('grvEpayHistoryHorizontal_RIMG');

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
        <awr:ResponsiveADHeader ID="ResponsiveADHeader" runat="server" MenuCollapsed="true" HideQuickSearch="true" HomeLinkClosesPage="true" HideMenu="true" SelectedAccountReadOnly="true" />
        <div class="NormalText">
           <h1 style="text-align: center">E-Payment History</h1>
        </div>
        <div style="text-align:center;">
            <asp:Label ID="lblNoAccountSelected" Text="Please select an account" runat="server" Visible="false" ForeColor="red"></asp:Label>
            <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" EnableViewState="False" ></asp:Label>
        </div>
        <div style="text-align:center;width:750px;margin: 0 auto;">
            <custom:nav id="Navigation" runat="server"/>
<%--            <p style="text-align: right"><a href ="documents/USBankHelp.pdf" target="_blank">Help</a> </p>--%>
            <div class="ContentBackground">
                <div class="ContentHeader" style="text-align: left">Search Epay History </div>
                <asp:Table ID="tblSearchInvoices" runat="server" >
                    <asp:TableRow style="text-align:left;">
                      <asp:TableCell>Invoice #:&nbsp;</asp:TableCell>
                      <asp:TableCell><asp:TextBox ID="txtInvoiceNumber" runat="server" Width="80px" onKeypress ="return isAlphaNumeric(event);" onPaste="return false;" CssClass="input-xs"></asp:TextBox>
                              </asp:TableCell>
                      <asp:TableCell>Check #:&nbsp;</asp:TableCell>
                      <asp:TableCell><asp:TextBox ID="txtConfirmationNo" runat="server" Width="75px" onKeypress ="return isAlphaNumeric(event);" onPaste="return false;" CssClass="input-xs"></asp:TextBox></asp:TableCell>
                      <asp:TableCell>Ref #:&nbsp;</asp:TableCell>
                      <asp:TableCell><asp:TextBox ID="txtReferenceNo" runat="server" Width="75px" onKeypress="return isNumber(event);"  onPaste="return false;" CssClass="input-xs"></asp:TextBox></asp:TableCell>
                      <asp:TableCell style="text-align:left;"> <asp:TextBox ID="txtDate" runat="server" Width="120px"  oncopy="return false" MaxLength="10" oncut="return false" onpaste="return   false" CssClass="input-xs"></asp:TextBox>&nbsp;</asp:TableCell>
                                    
                      <asp:TableCell style="text-align:right;">
                             <asp:Button ID="cmdSearch" Text="Search" runat="server" />&nbsp;
                      </asp:TableCell>
                      <asp:TableCell>
                          <asp:Button ID="btnExportToExcel" runat="server" Text="Export To Excel" />
                      </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
            <div class="ContentBackground">  
                <br class="BlankLine" />    
                <asp:Repeater ID="rptPager" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text")%>' CommandArgument='<%# Eval("Value")%>' Enabled='<%# Eval("Enabled")%>' 
                            Font-Bold="true" OnClick = "Page_Changed" />
                    </ItemTemplate>
                </asp:Repeater>
                <div  runat="server" style="width:100%;">
                    <asp:GridView ID="grvEpayHistory" runat="server"  Width="100%" AutoGenerateColumns="false" 
                            GridLines="None" AllowPaging="false" AllowSorting="true" EmptyDataText="No Payments To Display">
                        <EmptyDataRowStyle CssClass="ContentBackground" HorizontalAlign="Center" VerticalAlign="Top" Font-Italic="true" BorderWidth="0" />
                        <HeaderStyle CssClass="GridHeaderNative GridHeaderLock" BackColor="WhiteSmoke" ForeColor="Black" Height="25" />
                        <RowStyle CssClass="GridMainRow" />
                        <AlternatingRowStyle CssClass="GridAlternateRow" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbDateAdded" runat="server" CommandName="Sort" CommandArgument="DateAdded" ForeColor="Black" Text="Date Added" />
                                </HeaderTemplate>
                                <ItemStyle Width="100" HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbConfirmationNumber" runat="server" CommandName="Sort" CommandArgument="ConfirmationNumber" ForeColor="Black" Text="Check #" />
                                </HeaderTemplate>
                                <ItemStyle Width="75" HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbStatus" runat="server" CommandName="Sort" CommandArgument="Status" ForeColor="Black" Text="Status" />
                                </HeaderTemplate>
                                <ItemStyle Width="125" HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbReferenceNumber" runat="server" CommandName="Sort" CommandArgument="ReferenceNumber" ForeColor="Black" Text="Ref #" />
                                </HeaderTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                                <ItemStyle Width="75" />
                                <ItemTemplate>
                                    <asp:HyperLink ID="hplReferenceNumber" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbType" runat="server" CommandName="Sort" CommandArgument="Type" ForeColor="Black" Text="File" />
                                </HeaderTemplate>
                                <ItemStyle Width="75" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdnTypeCode" runat="server" />
                                    <asp:Label ID="lblType" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbGrossAmount" runat="server" CommandName="Sort" CommandArgument="GrossAmount" ForeColor="Black" Text="Gross" />
                                </HeaderTemplate>
                                <ItemStyle Width="75" HorizontalAlign="Right" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbDiscount" runat="server" CommandName="Sort" CommandArgument="Discount" ForeColor="Black" Text="Discount" />
                                </HeaderTemplate>
                                <ItemStyle Width="75" HorizontalAlign="Right" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:LinkButton ID="lkbAmountCharged" runat="server" CommandName="Sort" CommandArgument="AmountCharged" ForeColor="Black" Text="Net" />
                                </HeaderTemplate>
                                <ItemStyle Width="75" HorizontalAlign="Right" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>                   
            </div> 
        </div>
        <awr:ResponsiveADFooter ID="ResponsiveADFooter" runat="server" />
    </form>
</body>
</html>
