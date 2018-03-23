<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VideoPlayerBlock.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.VideoPlayerBlock" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="well">
            <asp:Literal ID="lLava" runat="server" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
