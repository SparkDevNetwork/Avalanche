<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WebViewBlock.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.WebViewBlock" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:label ID="lbHtml" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>