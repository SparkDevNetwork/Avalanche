<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupListBlock.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.GroupListBlock" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="well">
            <h2>Group List Parent Groups</h2>
            <asp:Label ID="lbGroups" runat="server" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
