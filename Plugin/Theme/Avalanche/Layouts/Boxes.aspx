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
            <div class="col-md-12">
                <Rock:Zone Name="ZoneA" runat="server" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-8">
                <Rock:Zone Name="ZoneB" runat="server" />
            </div>
             <div class="col-md-4">
                <Rock:Zone Name="ZoneC" runat="server" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-6">
                <Rock:Zone Name="ZoneD" runat="server" />
            </div>
             <div class="col-md-6">
                <Rock:Zone Name="ZoneE" runat="server" />
            </div>
        </div>
        <!-- End Content Area -->

	</main>

</asp:Content>

