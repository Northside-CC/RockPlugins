﻿<%@ Master Language="C#" AutoEventWireup="true" Inherits="Rock.Web.UI.RockMasterPage" %>
<%@ Import Namespace="System.Web.Optimization" %>
<!DOCTYPE html>

<script runat="server">
    
    // keep code below to call base class init method

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit( EventArgs e )
    {
        base.OnInit( e );
    }    
    
</script>

<html class="no-js">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <meta charset="utf-8">
    <title></title>
    
    <script src="<%# ResolveRockUrl("~/Scripts/jquery-1.10.2.min.js" ) %>"></script>

    <script src="<%# ResolveRockUrl("~~/Scripts/jquery.flexslider-min.js" ) %>" ></script>
    <script src="<%# ResolveRockUrl("~~/Scripts/theme.js", true) %>" ></script>

    <!-- Set the viewport width to device width for mobile -->
	<meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">

    <asp:ContentPlaceHolder ID="css" runat="server" />

	<!-- Included CSS Files -->
    <link rel="stylesheet" href="<%# ResolveRockUrl("~~/Styles/bootstrap.css", true) %>"/>
	<link rel="stylesheet" href="<%# ResolveRockUrl("~~/Styles/theme.css", true) %>"/>
	<link rel="stylesheet" href="<%# ResolveRockUrl("~/Styles/developer.css", true) %>"/>

    <script src="<%# ResolveRockUrl("~/Scripts/modernizr.js" ) %>" ></script>

    <asp:ContentPlaceHolder ID="head" runat="server"></asp:ContentPlaceHolder>

    <!-- Icons -->
    <link rel="shortcut icon" href="<%# ResolveRockUrl("~/Content/ExternalSite/Icons/favicon.ico", true) %>">
    <link rel="apple-touch-icon-precomposed" sizes="144x144" href="<%# ResolveRockUrl("~/Content/ExternalSite/Icons/touch-icon-ipad-retina.png", true) %>">
    <link rel="apple-touch-icon-precomposed" sizes="114x114" href="<%# ResolveRockUrl("~/Content/ExternalSite/Icons/touch-icon-iphone-retina.png", true) %>">
    <link rel="apple-touch-icon-precomposed" sizes="72x72" href="<%# ResolveRockUrl("~/Content/ExternalSite/Icons/touch-icon-ipad.png", true) %>">
    <link rel="apple-touch-icon-precomposed" href="<%# ResolveRockUrl("~/Content/ExternalSite/Icons/touch-icon-iphone.png", true) %>">
    
</head>
<body>

    <form id="form1" runat="server">

        <!-- Page Header -->
        <header>
        
            <!-- Brand Bar -->
            <nav class="navbar navbar-default navbar-static-top">
                <div class="container">
			        <div class="navbar-header">
                        <button class="navbar-toggle" type="button" data-toggle="collapse" data-target=".navbar-collapse">
                            <i class="fa fa-bars"></i>
                        </button>
                        <Rock:Zone Name="Header" runat="server" />
			        </div>	
                    <div class="navbar-collapse collapse">   
                        <!-- Main Navigation -->
                        <Rock:Zone Name="Login" runat="server" />
                        <Rock:Zone Name="Navigation" runat="server" />
			        </div>	
                </div>
            </nav>

        </header>
		
        <asp:ContentPlaceHolder ID="feature" runat="server"></asp:ContentPlaceHolder>

        <asp:ContentPlaceHolder ID="main" runat="server">
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
                <Rock:Zone Name="Feature" runat="server" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <Rock:Zone Name="Main" runat="server" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-12">
                <Rock:Zone Name="Section A" runat="server" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-4">
                <Rock:Zone Name="Section B" runat="server" />
            </div>
            <div class="col-md-4">
                <Rock:Zone Name="Section C" runat="server" />
            </div>
            <div class="col-md-4">
                <Rock:Zone Name="Section D" runat="server" />
            </div>
        </div>

        <!-- End Content Area -->

	</main>

</asp:Content>
        </asp:ContentPlaceHolder>

	    <footer>
            <div class="container">
		    
                <hr />

                <div class="row">
			        <div class="col-md-12">
				        <Rock:Zone Name="Footer" runat="server" />
			        </div>
		        </div>

            </div>
	    </footer>
        
        <%-- controls for scriptmanager and update panel --%>
        <asp:ScriptManager ID="sManager" runat="server"/>
        <asp:UpdateProgress id="updateProgress" runat="server" DisplayAfter="800">
		        <ProgressTemplate>
		            <div class="updateprogress-status">
                        <div class="spinner">
                          <div class="rect1"></div>
                          <div class="rect2"></div>
                          <div class="rect3"></div>
                          <div class="rect4"></div>
                          <div class="rect5"></div>
                        </div>
                    </div>
                    <div class="updateprogress-bg modal-backdrop"></div>
		        </ProgressTemplate>
        </asp:UpdateProgress>

    </form>

</body>

</html>