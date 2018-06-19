<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MobileWorkflow.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.MobileWorkflow" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
Mobile Workflow
        <Rock:NotificationBox runat="server" ID="nbError" />
    </ContentTemplate>
</asp:UpdatePanel>
