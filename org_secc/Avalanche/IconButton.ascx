<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IconButton.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.IconButton" %>
<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <Rock:BootstrapButton runat="server" CssClass="btn btn-default" ID="btnButton"></Rock:BootstrapButton>
    </ContentTemplate>
</asp:UpdatePanel>
