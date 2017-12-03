<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ImageBlock.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.ImageBlock" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:Image ID="iImage" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
