<%@ Page Inherits="Tridion.Web.UI.Controls.TridionPage" %>

<html ng-app="alchmyApp">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../css/style.css">
    <link rel="stylesheet" type="text/css" href="../css/published-summary.css">
    <link rel="stylesheet" type="text/css" href="../css/custom-control.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script src="http://ajax.googleapis.com/ajax/libs/angularjs/1.2.26/angular.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <!--<script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>-->
    
    <script type="text/javascript" src="../js/PublishedSummary.js"></script>

    <script>
        alchmyApp = angular.module('alchmyApp', []);
alchmyApp.controller('alchmyController', function ($scope, $http) {
    $http.get(document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetPublicationTarget").success(function (response) {
        $scope.PublicationTarget = response;
    });
    $http.get(document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetAllPublishedItems").success(function (response) {
        $scope.PublishedItems = response;
    });
    $http.get(document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetPublicationList").success(function (response) {
        $scope.PublicationList = response;
    });

    $http.get(document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetAnalyticData").success(function (response) {
        $scope.AnalyticData = response;
    });

    $scope.publicationTargets = [];

    $scope.pages = true;
    $scope.components = true;
    $scope.categories = true;
    $scope.templates = true;

    $scope.filteredPublishedItems = function (items) {
        return function (item) {
            for (var i in $scope.publicationTargets) {
                if ($scope.publicationTargets[i]) {
                    selected = true;
                    if ($scope.PublicationTarget[i].title == item.publicationTarget) {
                        if (item.type == "Page" && $scope.pages) {
                            return true;
                        }
                        if (item.type == "Component" && $scope.components) {
                            return true;
                        }
                        if (item.type == "Category" && $scope.categories) {
                            return true;
                        }
                        if (item.type == "ComponentTemplate" && $scope.templates) {
                            return true;
                        }
                    }
                }
            }

        };
    };
});

    </script>



</head>
<body>
    <div id="pub-summary" class="" ng-controller="alchmyController">

        <div class="col-sm-2 fixed">
            <img src="../img/content-bloom-logo-150x75.jpg" alt="www.contentbloom.com" />
            <hr />
            <div id="clear-filters" class="row text-right clear-filter-link"><a href="#">Clear All Filters</a></div>
            <div id="publish-target-filters" class="filters">
                <h2>Publishing Target</h2>

                <label class="checkbox-container" ng-repeat="data in PublicationTarget">
                    <input type="checkbox" ng-model="publicationTargets[$index]" />{{data.title}}
                     <span class="checkmark"></span>
                </label>
            </div>

            <div id="item-type-filters" class="filters">
                <h2>Item Type</h2>

                <label class="checkbox-container">
                    Pages
				      <input type="checkbox" checked="checked" ng-model ="pages">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Components
				      <input type="checkbox" checked="checked" ng-model ="components">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Categories
				      <input type="checkbox" checked="checked" ng-model ="categories">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Templates
				      <input type="checkbox" checked="checked" ng-model ="templates">
                    <span class="checkmark"></span>
                </label>
            </div>

            <div id="date-range-filters" class="filters">
                <h2>Date Range</h2>

                <label for="fromdate" class="padding-top-5">From:</label>
                <input type="date" name="fromdate" id="fromdate">

                <label for="todate" class="padding-top-5">To:</label>
                <input type="date" name="todate" id="todate">
            </div>
        </div> <!-- Left Side Panel -->

        <div class="col-sm-9 flex-item">
            <div class="row">
                <div class="col-sm-3 top-left">
                    <div class="select">
                        <select name="publications" id="publications">
                            <option ng-repeat="data in PublicationList" value="{{data.id}}">{{data.title}}</option>
                        </select>
                    </div>
                    <div>
                        <input type="text" placeholder="Search" ng-model="SearchText" class="textbox" />
                    </div>
                </div>

                <div class="col-sm-9 top-left">
                    <div class="summary-panel-header-grid">
							<div class="row-summary-panel">
								<div class="summary-panel-heading col-sm-2">Live</div>	
								
								<div class="summary-panel-details col-sm-4">
									<div>Total:<b>57</b></div>
									<div>Pages:<b>37</b></div>
									<div>Components:<b>13</b></div>
									<div>Categories:<b>7</b></div>
								</div>
							
								<div class="summary-panel-heading col-sm-2">Staging</div>	
								
								<div class="summary-panel-details col-sm-4">
									<div>Total:<b>57</b></div>
									<div>Pages:<b>37</b></div>
									<div>Components:<b>13</b></div>
									<div>Categories:<b>7</b></div>
								</div>
								
								<div class="summary-panel-heading col-sm-2">Acceptance</div>	
								
								<div class="summary-panel-details col-sm-4">
									<div>Total:<b>57</b></div>
									<div>Pages:<b>37</b></div>
									<div>Components:<b>13</b></div>
									<div>Categories:<b>7</b></div>
								</div>
								
								<div class="summary-panel-heading col-sm-2">Prod</div>	
								
								<div class="summary-panel-details col-sm-4">
									<div>Total:<b>57</b></div>
									<div>Pages:<b>37</b></div>
									<div>Components:<b>13</b></div>
									<div>Categories:<b>7</b></div>
								</div>
								
								<div class="summary-panel-heading col-sm-2">Prod</div>	
								
								<div class="summary-panel-details col-sm-4">
									<div>Total:<b>57</b></div>
									<div>Pages:<b>37</b></div>
									<div>Components:<b>13</b></div>
									<div>Categories:<b>7</b></div>
								</div>
							</div>
						</div>

            </div>
            <!-- Summary Panel and Publication Dropdown -->

            <div class="row">
                <div class="col-sm-12 padding-left-5">
                    <hr>
                </div>
            </div>
            <!-- Horizontal Row - Right Pane - Top -->

            <div class="summary-grid">
                <div class="row-eq-height row-header align-middle">

                    <div class="col-xs-2">ID</div>
                    <div class="col-xs-2">Title</div>
                    <div class="col-xs-2">Item Type</div>
                    <div class="col-xs-1">Target</div>
                    <div class="col-xs-2">By</div>
                    <div class="col-xs-2">Date</div>
                    <div class="col-xs-1">Action</div>
                </div>
                <div class="row-eq-height" ng-repeat="data in PublishedItems | filter:SearchText | filter:filteredPublishedItems(data)">

                    <div class="col-xs-2">{{data.id}}</div>
                    <div class="col-xs-2">{{data.title}}</div>
                    <div class="col-xs-2">{{data.type}}</div>
                    <div class="col-xs-1">{{data.publicationTarget}}</div>
                    <div class="col-xs-2">{{data.user}}</div>
                    <div class="col-xs-2">{{data.publishedAt | date:"dd MMM yyyy"}}</div>
                    <div class="col-xs-1">
                        <a href="#" data-toggle="tooltip" title="Publish Item!">
                            <img class="action-icon publish-icon" src="#" /></a>
                        <a href="#" data-toggle="tooltip" title="Unpublish Item!">
                            <img class="action-icon unpublish-icon" src="#"/></a>
                        <a href="{{data.openItem}}" data-toggle="tooltip" title="Open Item!" target="_blank">
                            <img class="action-icon open-icon" src="#"/></a>
                        
                    </div>
                </div>
            </div> <!-- Summary Grid -->

            <div class="col-sm-12 actions padding-left-5">
                <button id="btn_export" class="col-sm-2 button">Export in CSV</button>
                <button class="col-sm-2 button">Publish Selected</button>
                <button class="col-sm-2 button">Unpublish Selected</button>
                <button class="col-sm-3 button">Sync Publishing with Live</button>
                <button class="col-sm-2 button">Show Delta</button>
            </div> <!-- CTA -->

        </div> <!-- Right Side Panel -->
    </div>

    </div>
    <!--<script type="text/javascript">var removeSdlWebLoadInterval = setInterval(function () { if (!window.$display) { return; } clearInterval(removeSdlWebLoadInterval); if ($display && !$display.getView()) { if (window._activityIndicatorControl) { window._activityIndicatorControl.dispose(); window._activityIndicatorControl = null; } var sdlWebLoadingIndicator = $('style#loadingIndicator'); if (sdlWebLoadingIndicator) { $dom.removeNode(sdlWebLoadingIndicator); } } }, 500);</script>-->
</body>
</html>
