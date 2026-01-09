<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Confirmation.aspx.vb" Inherits="EPay.Confirmation" Trace="false" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>EPay Confirmation</title> 
    <asp:PlaceHolder ID="phStylesAndScripts" runat="server" /> 
    <script type="text/javascript">
        function verifyCancel()
        {
            return confirm("Press a button");
      
        }
    </script>
    <script type="text/javascript"  src="/Scripts/DateValidation.js"></script>
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>
      <script type="text/javascript">
      
          
          $(document).ready(function () {
              var totalRows = $('#<%=grvEPayInvoices.ClientID%> tr').length;
               if (totalRows > 5)
                 
                   gridviewScroll();

           });

           function gridviewScroll() {

               $('#grvEPayInvoices').gridviewScroll({
                   width: 410,
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

               var Timg = document.getElementById('grvEPayInvoicesVertical_TIMG');
               var Bimg = document.getElementById('grvEPayInvoicesVertical_BIMG');
               var HBar = document.getElementById('grvEPayInvoicesVerticalRail');
               var VBar = document.getElementById('grvEPayInvoicesHorizontalRail');
               var Limg = document.getElementById('grvEPayInvoicesHorizontal_LIMG');
               var Rimg = document.getElementById('grvEPayInvoicesHorizontal_RIMG');

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
    <form id="form1" runat="server">
  
    <awr:ResponsiveADHeader ID="ResponsiveADHeader" runat="server" MenuCollapsed="true" HideQuickSearch="true" HomeLinkClosesPage="true" HideMenu="true" SelectedAccountReadOnly="true" />
    
   <br class="BlankLine" />
          
    <div style="text-align:center">
        
    <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" EnableViewState="False" ></asp:Label>

      <div style="width:415px;margin:0 auto">

         <p style="text-align: right"><a href ="documents/USBankHelp.pdf" target="_blank">Help</a> </p>
   
         <div class="ContentBackground"> 
            <div class="ContentHeader" style="float:none;text-align:left;height:15px">E-Payment
             </div> 
           <div style="text-align:left;">
            <p>Thank you for choosing to pay the following Invoices thru Ashley's Epay with US Bank. 
            Please validate the payment amounts and click ok to proceed to U.S. Bank's E-Payment Service.</p>
            </div>   
         </div>

         <br class="BlankLine" />
               
         <div class="ContentBackground">                
            <div class="ContentHeader" style="float:none;text-align:left;height:15px">Customer Invoices For Reference Number 
              <asp:Label ID="lblReferenceNumber" runat="server" Text="lblReferenceNumber"></asp:Label>
            </div>
               <asp:GridView ID="grvEPayInvoices" runat="server" Width="100%" CssClass="GridViewCSSScrollable"  AutoGenerateColumns="false" 
                            GridLines="Both" AllowSorting="true" EmptyDataText="No Data To Display" >
                        <EmptyDataRowStyle CssClass="ContentBackground" HorizontalAlign="Center" VerticalAlign="Top" Font-Italic="true" BorderWidth="0" />
                        <HeaderStyle CssClass="GridHeaderNative GridHeaderLock" BackColor="WhiteSmoke" ForeColor="Black" Height="25" />
                        <RowStyle CssClass="GridMainRow" />
                     <AlternatingRowStyle CssClass="GridAlternateRow" />
                      <PagerStyle CssClass="GridHeaderNative" BackColor="WhiteSmoke" HorizontalAlign="Center" />
                    <PagerSettings NextPageText="Next" PreviousPageText="Prev" Mode="Numeric" Position="Top" Visible="true" />
                       <Columns>
                         <asp:TemplateField>	
                             <HeaderTemplate>                                
                                 <asp:LinkButton ID="lbtnInvoicenoHeader" runat="server" Text ="Invoice #" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <HeaderStyle HorizontalAlign ="Left" />
                             <ItemTemplate>
                                 <asp:Label ID="lblInvoiceno" runat="server" Text='<%#Eval("epyInvoiceNumber")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField>
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnDateHeader" runat="server" Text ="Date" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <HeaderStyle HorizontalAlign ="Left" />
                             <ItemTemplate>
                                 <asp:Label ID="lblDate" runat="server" Text='<%#Eval("opiDagedt")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField >
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnGrossAmountHeader" runat="server" Text ="Original Amount" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <HeaderStyle HorizontalAlign ="Right" />
                             <ItemTemplate>
                                 <asp:Label ID="lblGrossAmount" runat="server" Text='<%#Eval("epyGrossAmount","{0:c}")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Right" />
                         </asp:TemplateField>
                         <asp:TemplateField>
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnDiscountHeader" runat="server" Text ="Discount" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <HeaderStyle HorizontalAlign ="Right" />
                             <ItemTemplate>
                                 <asp:Label ID="lblDiscount" runat="server" Text='<%#Eval("Discount","{0:c}")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Right" />
                         </asp:TemplateField>
                              <asp:TemplateField>
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnAmountHeader" runat="server" Text ="Amount" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                                  <HeaderStyle HorizontalAlign ="Right" />
                             <ItemTemplate>
                                 <asp:Label ID="lblAmount" runat="server" Text='<%#Eval("epyAmountCharged","{0:c}")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Right" />
                         </asp:TemplateField>
                     </Columns>
                     </asp:GridView>          
     
            <br class="BlankLine" />
            <div style="text-align:right;">
                <asp:Label ID="lblTotal" runat="server" Text="Total:" Font-Bold="True" ></asp:Label>
                <asp:Label ID="lblTotalAmt" runat="server" Text="00.00" Font-Bold="True"></asp:Label>
                <asp:Label ID="lblSpaces" runat="server" Text="" Width="50"></asp:Label>
            </div>
            <br class="BlankLine" />
            <asp:Button ID="cmdOK" runat="server" Width="60" Text="OK" />
            <asp:Button ID="cmdCancel" runat="server" Width="60"  Text="Cancel" />
         </div>
          <br class="BlankLine" />
          <asp:Label ID="lblPrintText" runat="server" Text="Please print for your records."></asp:Label>
          <asp:ImageButton ID="cmdPrint" runat="server" ImageUrl="/images/print.png" /> 
      </div>
    </div>        
    <awr:ResponsiveADFooter ID="ResponsiveADFooter" runat="server" />

    </form>
</body>
</html>
