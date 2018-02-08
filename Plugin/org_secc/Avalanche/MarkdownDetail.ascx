<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarkdownDetail.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.MarkdownDetail" %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <pre><asp:label ID="lbMarkdown" runat="server" /></pre>
    </ContentTemplate>
</asp:UpdatePanel>