<%@ Page Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" Inherits="Rock.Web.UI.RockPage" %>

<asp:Content ID="ctMain" ContentPlaceHolderID="main" runat="server">
    
	<main class="container">
        
        <!-- Start Content Area -->
        
        <!-- Page Title -->
        <Rock:PageIcon ID="PageIcon" runat="server" /> <h1><Rock:PageTitle ID="PageTitle" runat="server" /></h1>
        
        <!-- Breadcrumbs -->    
        <Rock:PageBreadCrumbs ID="PageBreadCrumbs" runat="server" />

        <!-- Ajax Error -->
        <div class="alert alert-danger ajax-error" style="display:none">
            <p><strong>Error</strong></p>
            <span class="ajax-error-message"></span>
        </div>
        <div class="row">
            <div class="col-md-4">
                <Rock:Zone Name="TopLeft" runat="server" />
            </div>
            <div class="col-md-4">
                <Rock:Zone Name="TopCenter" runat="server" />
            </div>
            <div class="col-md-4">
                <Rock:Zone Name="TopRight" runat="server" />
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <Rock:Zone Name="Featured" runat="server" />
            </div>
        </div>
               
        <div class="row">
            <div class="col-md-12">
                <Rock:Zone Name="Main" runat="server" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-4">
                <Rock:Zone Name="Left" runat="server" />
            </div>
            <div class="col-md-4">
                <Rock:Zone Name="Center" runat="server" />
            </div>
            <div class="col-md-4">
                <Rock:Zone Name="Right" runat="server" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <Rock:Zone Name="Footer" runat="server" />
            </div>
        </div>
        <!-- End Content Area -->

	</main>

</asp:Content>

