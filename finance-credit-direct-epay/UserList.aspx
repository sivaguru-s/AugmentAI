<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserList.aspx.vb" Inherits="EPay.UserList" Trace="false" ValidateRequest="false"%>

<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server" >
    <title>EPay User List</title>
    <asp:PlaceHolder ID="phStylesAndScripts" runat="server" /> 
    <script type="text/javascript" src="/Scripts/gridviewScroll.min.js"></script>   

    
       <script type="text/javascript">
           $(document).ready(function () {
               var totalRows = $('#<%=grvUsers.ClientID%> tr').length;
               if (totalRows > 5)
                   {
                       gridviewScroll();
                   }

                 
           });

           function gridviewScroll() {


               $('#grvUsers').gridviewScroll({
                   width: 600,
                   height: 300,
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

               var Timg = document.getElementById('grvUsersVertical_TIMG');
               var Bimg = document.getElementById('grvUsersVertical_BIMG');
               var HBar = document.getElementById('grvUsersVerticalRail');
               var VBar = document.getElementById('grvUsersHorizontalRail');
               var Limg = document.getElementById('grvUsersHorizontal_LIMG');
               var Rimg = document.getElementById('grvUsersHorizontal_RIMG');

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
    <div class="BodyIndent" style="width:600px;margin:0 auto">
        <h1 style="text-align: center">E-Payment User List</h1>
        <p style="text-align: right; width: 606px;">
            &nbsp; 
            <custom:nav id="Navigation" runat="server"/>
        </p>
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red" EnableViewState="False" ></asp:Label>
                </td>
            </tr>
        </table>
        <div class="ContentHeader" style="text-align: left; width: 606px;">Search User</div>
        <div class="ContentBackground" style="text-align: left; width: 600px;">
            <asp:Table ID="tblSearchUsers" runat="server" Width="600px">
                <asp:TableRow style="text-align:left;">
                    <asp:TableCell style="text-align:left; width:62px;">Acct #:</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:100px;"><asp:TextBox ID="txtAccountNumber" runat="server" Width="90px" CssClass="input-xs"></asp:TextBox></asp:TableCell>                
                    <asp:TableCell style="text-align:left; width:92px;">Acct Name :</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:160px;"><asp:TextBox ID="txtAccountName" runat="server" Width="150px" CssClass="input-xs"></asp:TextBox></asp:TableCell>                   
                    <asp:TableCell style="text-align:left; width:87px;">Bill-To-State :</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:60px;"><asp:DropDownList ID="lstBillToState" runat="server" Width="50px"></asp:DropDownList></asp:TableCell>
                    <asp:TableCell style="text-align:left; width:30px;">&nbsp;</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow style="text-align:left;">
                    <asp:TableCell style="text-align:left; width:62px;">Territory :</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:100px;"><asp:DropDownList ID="lstTerritory" runat="server" Width="45px"></asp:DropDownList></asp:TableCell>
                    <asp:TableCell style="text-align:left; width:92px;">Terms Code :</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:160px;"><asp:DropDownList ID="lstTermsCode" runat="server" Width="45px"></asp:DropDownList>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnReset" Text="Reset" runat="server" Width="50px" Height="22px" /></asp:TableCell>
                    <asp:TableCell style="text-align:left; width:87px;">
                        <asp:Button ID="cmdSearch" Text="Search" runat="server" Width="80px" Height="25px" /></asp:TableCell>
                    <asp:TableCell style="text-align:left; width:60px;">&nbsp;</asp:TableCell>
                    <asp:TableCell style="text-align:left; width:30px;">&nbsp;</asp:TableCell>
                </asp:TableRow>                
            </asp:Table>
        </div>
        <br class="BlankLine" />
        <div class="ContentBackground" style="width: 600px;">
            <table class="ContentHeader" cellpadding="0" cellspacing="0" style="width: 600px;">
                <tr class="ContentHeader">
                    <td class="ContentHeader" style="width: 250px;">
                        E-Payment users as of 
                        <asp:Label ID="lblDate" runat="server" Text="lblDate"></asp:Label>
                    </td>
                     <td class="ContentHeader" style="width: 150px;">
                        <asp:Button ID="btnPrintablePage" runat="server" Text="Printable Page" />
                     </td>
                        
                    <td class="ContentHeader" style="width: 90px;">
                        
                    </td>
                    <td>
                        <asp:ImageButton ID="cmdExportToExcel" runat="server" ImageUrl="/images/ExportToExcel.gif" Height="19px" />
                    </td>
                    
                </tr>
            </table>                    
            <asp:HiddenField ID="Hdnpageno" runat="server" />
              <div style="text-align:center;">
                <asp:Repeater ID="rptPager" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>' Enabled='<%# Eval("Enabled") %>' 
                            Font-Bold="true"    ForeColor='<%# IIf(Eval("Enabled"), System.Drawing.Color.SteelBlue, System.Drawing.Color.Black)%>'  OnClick = "Page_Changed" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
               <div >
                 <asp:GridView ID="grvUsers" runat="server" CssClass="GridViewCSSScrollable" Width="100%" AutoGenerateColumns="false"
                      GridLines="Both" AllowSorting="true" EmptyDataText="No Users To Display" OnSorting="grvUsers_Sorting">
                        <EmptyDataRowStyle CssClass="ContentBackground" HorizontalAlign="Center" VerticalAlign="Top" Font-Italic="true" BorderWidth="0" />
                        <HeaderStyle CssClass="GridHeaderLock" BackColor="WhiteSmoke" ForeColor="Black" Height="25" />
                        <RowStyle CssClass="GridMainRow" />
                     <AlternatingRowStyle CssClass="GridAlternateRow" />                  
                         <Columns>
                         <asp:TemplateField SortExpression ="Acct #">	
                             <HeaderTemplate>                                
                                 <asp:LinkButton ID="lbtnAccnoHeader" runat="server" Text ="Acct #" CommandName ="Sort" CommandArgument ="Acct #" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblAccno" runat="server" Text='<%#Eval("Acct #")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="Acct Name">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnAccnameHeader" runat="server" Text ="Acct Name" CommandName ="Sort" CommandArgument ="Acct Name" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblAccname" runat="server" Text='<%#Eval("Acct Name")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Left" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="Bill-To-State">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnbilltostageHeader" runat="server" Text ="Bill-To-State" CommandName ="Sort" CommandArgument ="Bill-To-State" ForeColor ="Black"></asp:LinkButton>
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblbilltostage" runat="server" Text='<%#Eval("Bill-To-State")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Center" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="Territory">
                             <HeaderTemplate>
                                 <asp:LinkButton ID="lbtnterritoryHeader" runat="server" Text ="Territory" CommandName ="Sort" CommandArgument ="Territory" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <ItemTemplate>
                                 <asp:Label ID="lblterritory" runat="server" Text='<%#Eval("Territory")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Center" />
                         </asp:TemplateField>
                         <asp:TemplateField SortExpression ="Terms Code">
                             <HeaderTemplate>                               
                                 <asp:LinkButton ID="lbtntermscodeHeader" runat="server" Text ="Terms Code" CommandName ="Sort" CommandArgument ="Terms Code" ForeColor ="Black"></asp:LinkButton>                                 
                             </HeaderTemplate>
                             <ItemTemplate>
                                   <asp:Label ID="lbltermscode" runat="server" Text='<%#Eval("Terms Code")%>'></asp:Label>
                             </ItemTemplate>
                             <ItemStyle HorizontalAlign ="Center" />
                         </asp:TemplateField>
                     </Columns>
                     </asp:GridView>
                </div>
         </div>              
        <br />
        &nbsp;  
                           
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
    <awr:ResponsiveADFooter ID="ResponsiveADFooter" runat="server" />
</form>
</body>
</html>
