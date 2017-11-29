<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Button.ascx.cs" Inherits="RockWeb.Plugins.org_secc.Avalanche.Button" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <Rock:BootstrapButton runat="server" ID="btnButton" OnClick="btnButton_Click" CssClass="btn btn-default"></Rock:BootstrapButton>
    </ContentTemplate>
</asp:UpdatePanel>
