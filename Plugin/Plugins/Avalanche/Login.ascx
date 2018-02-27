<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Login.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.Login" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <Rock:NotificationBox runat="server" Text="Login Block" NotificationBoxType="Info"></Rock:NotificationBox>
    </ContentTemplate>
</asp:UpdatePanel>
