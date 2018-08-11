<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TextOverImage.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.TextOverImage" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="well">
        <asp:Image ID="imgImage" runat="server" CssClass="img-responsive" />
            <br />
            <asp:Literal ID="lLava" runat="server" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
