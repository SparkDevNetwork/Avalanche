<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ImageBlock.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.ImageBlock" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="well">
        <asp:Image ID="imgImage" runat="server" />
            <asp:Literal ID="lLava" runat="server" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
