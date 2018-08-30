<%@ Page Inherits="Tridion.Web.UI.Controls.TridionPage" %>

<html ng-app="alchemyApp">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="../css/style.css">
    <link rel="stylesheet" type="text/css" href="../css/published-summary.css">
    <link rel="stylesheet" type="text/css" href="../css/custom-control.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/angularjs/1.2.26/angular.min.js"></script>
    

    <script>
        $(document).ready(function () {
            alert("JavaScript: "+this.window.dialogArguments);
            $.ajax({
                type: "POST",
                url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/PublishItems",
                data: "{'IDs':[{ 'Id': 'tcm:14-877-64', 'Target': 'DXA Staging'},{ 'Id': 'tcm:14-1867-64', 'Target': 'DXA Staging'}]}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });

        });
        $(document).ready(function () {
            $.ajax({
                type: "POST",
                url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/UnPublishItems",
                data: "{'IDs':[{ 'Id': 'tcm:14-877-64', 'Target': 'DXA Staging'},{ 'Id': 'tcm:14-1867-64', 'Target': 'DXA Staging'}]}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });

        });
        alchemyApp = angular.module('alchemyApp', []);
        alchemyApp.controller('alchemyController',['$scope', '$http', '$window', function ($scope, $http, $window) {

            $scope.publicationTargets = [];
            $scope.pages = true;
            $scope.components = true;
            $scope.categories = true;
            $scope.templates = true;

            $scope.fromDate = null;
            $scope.toDate = null;

            $scope.orderByField = 'title';
            $scope.reverseSort = false;


            $scope.Publications = {};
            $scope.Publications.itemId = "tcm:0-14-1";//$window.alert("AngularJS: "+$window.dialogArguments);


            $http.get(document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetPublicationTarget").success(function (response) {
                $scope.PublicationTarget = response;
            });
            $http({
                url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetAllPublishedItems",
                method: "POST",
                data: "{'IDs':['" + $scope.Publications.itemId + "']}"
            }).success(function (response) {
                $scope.PublishedItems = response;
            });
            $http.get(document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetPublicationList").success(function (response) {
                $scope.Publications.PublicationList = response;
            });

            $http({
                url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetSummaryPanelData",
                method: "POST",
                data: "{'IDs':['" + $scope.Publications.itemId + "']}"
            }).success(function (response) {
                $scope.PublishedSummaryPanelData = response;
            });

            $scope.filteredPublishedItems = function (items) {
                return function (item) {
                    for (var i in $scope.publicationTargets) {
                        if ($scope.publicationTargets[i]) {
                            if ($scope.PublicationTarget[i].title == item.publicationTarget) {
                                if (item.type == "Page" && $scope.pages) {
                                    return filteredDate(item);
                                }
                                if (item.type == "Component" && $scope.components) {
                                    return filteredDate(item);
                                }
                                if (item.type == "Category" && $scope.categories) {
                                    return filteredDate(item);
                                }
                                if (item.type == "ComponentTemplate" && $scope.templates) {
                                    return filteredDate(item);
                                }

                            }
                        }
                    }
                };
            };
            var filteredDate = function (item) {
                var returned = false;
                if ($scope.fromDate != null && $scope.toDate != null) {
                    if ($scope.toDate != "" && $scope.fromDate != "") {
                        returned = true;
                        var publishDate = new Date(item.publishedAt);
                        var fromDate = new Date($scope.fromDate);
                        var toDate = new Date($scope.toDate);
                        if (publishDate > fromDate && publishDate < toDate) {
                            return true;
                        }
                    }
                }
                if (!returned) {
                    return true;
                }
            };

            var publish = function (item) {
                alert(item);
                return true;
            }

        }]);

        alchemyApp.directive("toggleClass", function () {
            return {
                link: function ($scope, element, attr) {
                    element.on("click", function () {
                        element.toggleClass("selected-row");
                    });
                }
            }
        });

        $(document).ready(function () {
            $("#btn_export").click(function () {
                var csvArray = [];
                var csv = "";
                var dom = $$(".row-eq-height div");
                for (var index = 0; index < dom.length; index++) {
                    if ((index + 1) % 7 != 0) {
                        csv += dom[index].innerText + ",";
                    }
                    else {
                        csvArray.push(csv.slice(0, -1));
                        csv = "";
                    }
                }
                csvArray.push(csv.slice(0, -1));

                console.log("csvArray JSON: " + JSON.stringify(csvArray));
                downloadJSON2CSV(csvArray);
            });

            $("#btn_publish_selected").click(function () {
                var publishParams = getSelectedItems();
                alert(JSON.stringify(publishParams));
            });

            $("#btn_unpublish_selected").click(function () {
                var unpublishParams = getSelectedItems();
                alert(JSON.stringify(unpublishParams));
            });

            function getSelectedItems() {
                var dom = $$(".selected-row div");
                var params = [];
                for (var index = 0; index < dom.length; index = index + 7) {
                    var temp = {};
                    temp.id = dom[index].innerText;
                    temp.target = dom[index + 3].innerText
                    params.push(temp);
                }
                return params;
            }

            function publish(tcmURI) {
                alert(tcmURI);
            }

            function unpublish(tcmURI) {
                alert(tcmURI);
            }

            function downloadJSON2CSV(objArray) {
                //var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
                var array = objArray;
                console.log("Array Passed: " + array);
                var str = '';
                console.log("Array Length: " + array.length);
                for (var i = 0; i < array.length; i++) {
                    str += array[i] + '\r\n';
                }

                console.log("str: " + str);

                if (navigator.appName != 'Microsoft Internet Explorer') {
                    window.open('data:text/csv;charset=utf-8,' + escape(str));
                }
                else {
                    var popup = window.open('', 'csv', '');
                    popup.document.body.innerHTML = '<pre>' + str + '</pre>';
                }
            };

        });

    </script>



</head>
<body>
    <div id="pub-summary" class="" ng-controller="alchemyController">

        <div class="col-sm-2 fixed">
            <img src="../img/content-bloom-logo-150x75.jpg" alt="www.contentbloom.com" />
            <hr />
            <div id="clear-filters" class="row text-right clear-filter-link"><a href="#">Clear All Filters</a></div>
            <div id="publish-target-filters" class="filters">
                <h2>Publishing Target</h2>

                <label class="checkbox-container" ng-repeat="data in PublicationTarget">
                    <input type="checkbox" ng-checked="true" ng-model="publicationTargets[$index]" ng-init="publicationTargets[$index]=true" />{{data.title}}
                     <span class="checkmark"></span>
                </label>
            </div>

            <div id="item-type-filters" class="filters">
                <h2>Item Type</h2>

                <label class="checkbox-container">
                    Pages
				      <input type="checkbox" checked="checked" ng-model="pages">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Components
				      <input type="checkbox" checked="checked" ng-model="components">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Categories
				      <input type="checkbox" checked="checked" ng-model="categories">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Templates
				      <input type="checkbox" checked="checked" ng-model="templates">
                    <span class="checkmark"></span>
                </label>
            </div>

            <div id="date-range-filters" class="filters">
                <h2>Date Range</h2>

                <label for="fromdate" class="padding-top-5">From:</label>
                <input type="date" name="fromdate" id="fromdate" ng-model="fromDate">

                <label for="todate" class="padding-top-5">To:</label>
                <input type="date" name="todate" id="todate" ng-model="toDate">
            </div>
        </div>
        <!-- Left Side Panel -->

        <div class="col-sm-9 flex-item">
            <div class="row">
                <div class="col-sm-3 top-left">
                    <div class="select">
                        <select name="publications" id="publications" ng-model="Publications.itemId" ng-options="publication.id as publication.title for publication in Publications.PublicationList">
                        </select>
                    </div>
                    <div>
                        <input type="text" placeholder="Search" ng-model="SearchText" class="textbox" />
                    </div>
                </div>

                <div class="col-sm-9 top-left">
                    <div class="summary-panel-header-grid">
                        <div class="row-summary-panel">
                            <div ng-repeat="data in PublishedSummaryPanelData">
                                <div class="summary-panel-heading col-sm-2">{{data.title}}</div>

                                <div class="summary-panel-details col-sm-4">
                                    <div>Total:<b>{{data.page + data.component  + data.category + data.componentTemplate }}</b></div>
                                    <div>Pages:<b>{{data.page}}</b></div>
                                    <div>Components:<b>{{data.component}}</b></div>
                                    <div>Categories:<b>{{data.category}}</b></div>
                                    <div>Templates:<b>{{data.componentTemplate}}</b></div>
                                </div>
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
                        <div class="col-xs-2" ng-click="orderByField='id'; reverseSort = !reverseSort">
                            ID <span ng-show="orderByField == 'id'"><span ng-show="!reverseSort">^</span><span ng-show="reverseSort">v</span></span>
                        </div>
                        <div class="col-xs-2" ng-click="orderByField='title'; reverseSort = !reverseSort">
                            Title <span ng-show="orderByField == 'title'"><span ng-show="!reverseSort">^</span><span ng-show="reverseSort">v</span></span>
                        </div>
                        <div class="col-xs-2" ng-click="orderByField='type'; reverseSort = !reverseSort">
                            Type <span ng-show="orderByField == 'type'"><span ng-show="!reverseSort">^</span><span ng-show="reverseSort">v</span></span>
                        </div>
                        <div class="col-xs-1" ng-click="orderByField='publicationTarget'; reverseSort = !reverseSort">
                            Target <span ng-show="orderByField == 'publicationTarget'"><span ng-show="!reverseSort">^</span><span ng-show="reverseSort">v</span></span>
                        </div>
                        <div class="col-xs-2" ng-click="orderByField='user'; reverseSort = !reverseSort">
                            By <span ng-show="orderByField == 'user'"><span ng-show="!reverseSort">^</span><span ng-show="reverseSort">v</span></span>
                        </div>
                        <div class="col-xs-2" ng-click="orderByField='publishedAt'; reverseSort = !reverseSort">
                            Date <span ng-show="orderByField == 'publishedAt'"><span ng-show="!reverseSort">^</span><span ng-show="reverseSort">v</span></span>
                        </div>
                        <div class="col-xs-1">Action</div>
                    </div>
                    <div class="row-eq-height" toggle-class ng-repeat="data in PublishedItems | filter:SearchText | filter:filteredPublishedItems(data) | orderBy:orderByField:reverseSort">

                        <div class="col-xs-2">{{data.id}}</div>
                        <div class="col-xs-2">{{data.title}}</div>
                        <div class="col-xs-2">{{data.type}}</div>
                        <div class="col-xs-1">{{data.publicationTarget}}</div>
                        <div class="col-xs-2">{{data.user}}</div>
                        <div class="col-xs-2">{{data.publishedAt | date:"dd MMM yyyy"}}</div>
                        <div class="col-xs-1">
                            <a href="#" data-toggle="tooltip" title="Publish Item!">
                                <img class="action-icon publish-icon" src="#" /></a>
                            <a href="unpublish({{data.id}});" data-toggle="tooltip" title="Unpublish Item!">
                                <img class="action-icon unpublish-icon" src="#" /></a>
                            <a href="{{data.openItem}}" data-toggle="tooltip" title="Open Item!" target="_blank">
                                <img class="action-icon open-icon" src="#" /></a>

                        </div>
                    </div>
                </div>
                <!-- Summary Grid -->

                <div class="col-sm-12 actions padding-left-5">
                    <button id="btn_export" class="col-sm-2 button">Export in CSV</button>
                    <button id="btn_publish_selected" class="col-sm-2 button">Publish Selected</button>
                    <button id="btn_unpublish_selected" class="col-sm-2 button">Unpublish Selected</button>
                    <button class="col-sm-3 button">Sync Publishing with Live</button>
                    <button class="col-sm-2 button">Show Delta</button>
                </div>
                <!-- CTA -->

            </div>
            <!-- Right Side Panel -->
        </div>
    </div>
    <!--<script type="text/javascript">var removeSdlWebLoadInterval = setInterval(function () { if (!window.$display) { return; } clearInterval(removeSdlWebLoadInterval); if ($display && !$display.getView()) { if (window._activityIndicatorControl) { window._activityIndicatorControl.dispose(); window._activityIndicatorControl = null; } var sdlWebLoadingIndicator = $('style#loadingIndicator'); if (sdlWebLoadingIndicator) { $dom.removeNode(sdlWebLoadingIndicator); } } }, 500);</script>-->
</body>
</html>
