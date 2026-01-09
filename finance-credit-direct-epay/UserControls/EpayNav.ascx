<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EpayNav.ascx.vb" Inherits="EPay.EpayNav" %>

<style type="text/css">
    div#ePayContainer {
        text-align: right;
    }

    ul.ePayNav {
        list-style: none;
    }

    li.ePayNav:before {
        content: "|";
    }

    li.ePayNav {
        display: inline;
        padding-left: 2px;
    }

    ul.ePayNav li:first-child:before {
        content: "";
    }
</style>

<div id="ePayContainer">
    <ul class="ePayNav">
        <li class="ePayNav" id="liEpayHome" runat="server">
            <asp:HyperLink ID="hlEpayHome" CssClass="ePayNav" runat="server" NavigateUrl="~/main.aspx" Text="E-Pay Home" />
        </li>
        <li class="ePayNav" id="liUserList" runat="server">
            <asp:HyperLink ID="hlUserList" CssClass="ePayNav" runat="server" NavigateUrl="~/UserList.aspx" Text="User List" />
        </li>
        <li class="ePayNav" id="liAdminMaintenance" runat="server">
            <asp:HyperLink ID="hlAdminMaintenance" CssClass="ePayNav" runat="server" NavigateUrl="~/AdminMaintenance.aspx" Text="Admin Maintenance" />
        </li>
        <li class="ePayNav" id="liReport" runat="server">
            <asp:HyperLink ID="hlReport" CssClass="ePayNav" runat="server" NavigateUrl="~/AnalystReport.aspx" Text="Analyst Report" />
        </li>
        <li class="ePayNav">
            <asp:HyperLink ID="hlHistory" CssClass="ePayNav" runat="server" NavigateUrl="~/History.aspx" Text="History" />
        </li>
        <li class="ePayNav">
            <asp:HyperLink ID="hlHelp" CssClass="ePayNav" runat="server" NavigateUrl="~/documents/USBankHelp.pdf" Text="Help" Target="_blank" />
        </li>
    </ul>
</div>
