<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MobileListViewLava.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.MobileListViewLava
    " %>

<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <div class="row">
            <div class="col-sm-6">
                <pre><asp:Literal runat="server" ID="lLava" /></pre>
            </div>
            <div class="col-sm-6 well">
                <asp:Literal runat="server" ID="lLavaRendered" />
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
