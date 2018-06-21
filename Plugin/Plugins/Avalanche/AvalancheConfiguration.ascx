<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AvalancheConfiguration.ascx.cs" Inherits="RockWeb.Plugins.Avalanche.AvalancheConfiguration" %>

<asp:UpdatePanel ID="pnlConfiguration" runat="server" Class="panel panel-block">
    <ContentTemplate>
        <div class="panel-heading">
            <h1 class="panel-title">
                <i class="fa fa-mobile"></i>
                Avalanche Configuration</h1>
        </div>
        <div class="panel-body">
            <asp:ValidationSummary ID="vsDetails" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
            <div class="row">
                <div class="col-md-6">
                    <Rock:PagePicker runat="server" ID="ppHome" Label="Home Page" />
                </div>
                <div class="col-md-6">

                    <Rock:PagePicker ID="ppMenu" runat="server" Label="Menu Page" />
                </div>
            </div>
            <Rock:BootstrapButton runat="server" ID="btnSave" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
            <Rock:BootstrapButton runat="server" ID="btnBack" CssClass="btn btn-link" Text="Cancel" OnClick="btnBack_Click" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

