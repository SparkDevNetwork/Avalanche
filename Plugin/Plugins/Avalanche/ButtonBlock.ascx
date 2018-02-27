<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ButtonBlock.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.ButtonBlock" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <Rock:BootstrapButton runat="server" ID="btnButton" OnClick="btnButton_Click" CssClass="btn btn-default"></Rock:BootstrapButton>
    </ContentTemplate>
</asp:UpdatePanel>
