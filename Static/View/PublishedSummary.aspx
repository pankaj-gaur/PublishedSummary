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
</head>
<body>
    <script>
        $(document).ready(function () {
            $.ajax({
                type: "GET",
                url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetPagesInsideSG",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                },
                failure: function (response) {
                },
                error: function (response) {
                }
            });

        });

        $(document).ready(function () {
            $.ajax({
                type: "GET",
                url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetComponents",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                },
                failure: function (response) {
                },
                error: function (response) {
                }
            });

        });

        $(document).ready(function () {
            $.ajax({
                type: "GET",
                url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetAnalyticData",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                    StateCityChart(response);
                },
                failure: function (response) {

                },
                error: function (response) {

                }
            });

        });

        function initialize() {
            $('.callChart').live('click', function () {
                drawChart();
            });
        };
        function StateCityChart(response) {
            var dataArray = response;
            var queryObject = "";
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Key');
            data.addColumn('number', 'Count');
            $.each(response, function (i, response) {
                var value = response.total;
                var name = response.key;
                data.addRows([[name, value]]);
            });

            //var data = new google.visualization.arrayToDataTable(dataArray);
            var Graphtitle;

            Graphtitle = "Analytics  ByYear";



            var options = {
                title: Graphtitle,
                is3D: true,
                animation: {
                    duration: 3000,
                    easing: 'out',
                    startup: true
                },
                colorAxis: { colors: ['#54C492', '#cc0000'] },
                datalessRegionColor: '#dedede',
                defaultColor: '#dedede',
                vAxis: { title: "Vertical Axis Title" }, //Bar of Pie Charts
                hAxis: { title: "Horizontal Axis Title " }, //Bar of Pie Charts
            };
            var chart;

            chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
            chart.draw(data, options);
            chart = new google.visualization.PieChart(document.getElementById('piechart_div'));
            chart.draw(data, options);
            chart = new google.visualization.BarChart(document.getElementById('bar_div'));
            chart.draw(data, options);

            return false;
        };

        google.load("visualization", "1", { packages: ["corechart"] });
        google.setOnLoadCallback(initialize);


        $(document).ready(function () {
            $("#btn_export").click(function () {
                var csvArray = [];
                var csv = "";
                var dom = $$(".row-eq-height div");
                console.log("DOM: "+dom);
                for (var index = 0; index < dom.length; index++) {
                    if ((index + 1) % 7 != 0) {
                        csv += dom[index].innerText + ",";
                    }
                    else {
                        csvArray.push(csv.split(0, -1));
                        csv = "";
                    }
                }
                csvArray.push(csv.split(0, -1));

                console.log("csvArray JSON: "+JSON.stringify(csvArray));
                /*$$(".row-eq-height div").each(function (index) {
                    if ((index + 1) % 8 != 0) {
                        csv += $(this).text() + ",";
                    }
                    else {
                        csvArray.push(csv.split(0, -1));
                        csv = "";
                    }
                })*/
                //downloadJSON2CSV(JSON.stringify(csvArray));
                downloadJSON2CSV(csvArray);
            });

            function downloadJSON2CSV(objArray) {
                //var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
                var array = objArray;
                console.log("Array Passed: "+array);
            var str = '';
                console.log("Array Length: " + array.length);
                for (var i = 0; i < array.length; i++) {
                    str += array[i] + '\r\n';
                }

                console.log("str: "+ str);

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

            $scope.publicationTargets = [];

            //$scope.pages = true;

            $scope.filterPublicationTarget = function (pts) {
                return function (pt) {
                    for (var i in $scope.publicationTargets) {
                        if (pt.publicationTarget == $scope.PublicationTarget[i].title && $scope.publicationTargets[i]) {
                            return true;
                        }
                    }
                    //if (itemType.Type == "pages" && $scope.pages) {
                    //    return true;
                    //}
                };
            };
        });

    </script>
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
				      <input type="checkbox" checked="checked">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Components
				      <input type="checkbox" checked="checked">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Categories
				      <input type="checkbox">
                    <span class="checkmark"></span>
                </label>

                <label class="checkbox-container">
                    Templates
				      <input type="checkbox">
                    <span class="checkmark"></span>
                </label>
            </div>

            <div id="record-count-filters" class="filters">
                <h2>Record Count</h2>

                <label class="checkbox-container">
                    All
				      <input type="radio" name="radio">
                    <span class="checkmark"></span>
                </label>
                <label class="checkbox-container">
                    Top 100
				      <input type="radio" name="radio" checked="checked">
                    <span class="checkmark"></span>
                </label>
                <label class="checkbox-container">
                    Top 1000
				      <input type="radio" name="radio">
                    <span class="checkmark"></span>
                </label>
                <label class="checkbox-container">
                    Top 5000
				      <input type="radio" name="radio">
                    <span class="checkmark"></span>
                </label>
            </div>

            <div id="date-range-filters" class="filters">
                <h2>Date Range</h2>

                <label for="fromdate">From:</label>
                <input type="date" name="fromdate" id="fromdate">

                <label for="todate">To:</label>
                <input type="date" name="todate" id="todate">
            </div>
        </div>
        <!-- Left Side Panel -->

        <div class="col-sm-9 flex-item">
            <div class="row">
                <div class="col-sm-3 top-left">
                    <div class="select">
                        <select name="publications" id="publications">
                            <option ng-repeat="data in PublicationList" value="{{data.id}}">{{data.title}}</option>
                        </select>
                    </div>
                    <div>
                        Search: 
                        <input type="text" placeholder="Search" ng-model="SearchText" />
                    </div>
                </div>

                <div class="col-sm-9 top-left">
                    <div class="col-sm-1 glyphicon glyphicon-chevron-left"></div>
                    <div class=" col-sm-9 summary-panel">
                        <div class="summary-panel-heading col-sm-2">Live</div>
                        <div class="summary-panel-details col-sm-3">
                            <div>Total Published:<b>57</b></div>
                            <div>Pages:<b>37</b></div>
                            <div>Components:<b>13</b></div>
                            <div>Categories:<b>7</b></div>
                        </div>
                        <div class="summary-panel-heading col-sm-2">Staging</div>
                        <div class="summary-panel-details col-sm-3">
                            <div>Total Published:<b>68</b></div>
                            <div>Pages:<b>42</b></div>
                            <div>Components:<b>18</b></div>
                            <div>Categories:<b>8</b></div>
                        </div>
                    </div>
                    <div class="col-sm-1 glyphicon glyphicon-chevron-right"></div>


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
                <div class="row-eq-height-header align-middle">
                    
                    <div class="col-xs-1">ID</div>
                    <div class="col-xs-2">Title</div>
                    <div class="col-xs-1">Item Type</div>
                    <div class="col-xs-2">Target</div>
                    <div class="col-xs-2">By</div>
                    <div class="col-xs-2">Date</div>
                    <div class="col-xs-2">Action</div>
                </div>
                <div class="row-eq-height" ng-repeat="data in PublishedItems | filter:SearchText | filter:filterPublicationTarget(data)">
                    
                    <div class="col-xs-1">{{data.id}}</div>
                    <div class="col-xs-2">{{data.title}}</div>
                    <div class="col-xs-1">{{data.type}}</div>
                    <div class="col-xs-2">{{data.publicationTarget}}</div>
                    <div class="col-xs-2">{{data.user}}</div>
                    <div class="col-xs-2">{{data.publishedAt | date:"dd/MM/yyyy"}}</div>
                    <div class="col-xs-2">
                        <a href="#" data-toggle="tooltip" title="Publish Item!">
                            <img class="action-icon" src="../img/publish.png" alt="Publish" /></a>
                        <a href="#" class="action-icon" data-toggle="tooltip" title="Unpublish Item!">
                            <img class="action-icon" src="../img/unpublish.png" alt="Unpublish" /></a>
                        <a href="{{data.openItem}}" class="action-icon" data-toggle="tooltip" title="Open Item!" target="_blank">
                            <img class="action-icon" src="../img/open.png" alt="Open" /></a>
                    </div>
                </div>
            </div>
            <!-- Summary Grid -->

            <div class="col-sm-12 actions padding-left-5">
                <button id="btn_export" class="col-sm-2 button">Export in CSV</button>
                <button class="col-sm-2 button">Publish Selected</button>
                <button class="col-sm-2 button">Unpublish Selected</button>
                <button class="col-sm-3 button">Sync Publishing with Live</button>
                <button class="col-sm-2 button">Show Delta</button>
            </div>
            <!-- CTA -->

        </div>
        <!-- Right Side Panel -->
    </div>
    <script type="text/javascript">var removeSdlWebLoadInterval = setInterval(function () { if (!window.$display) { return; } clearInterval(removeSdlWebLoadInterval); if ($display && !$display.getView()) { if (window._activityIndicatorControl) { window._activityIndicatorControl.dispose(); window._activityIndicatorControl = null; } var sdlWebLoadingIndicator = $('style#loadingIndicator'); if (sdlWebLoadingIndicator) { $dom.removeNode(sdlWebLoadingIndicator); } } }, 500);</script>
</body>
</html>
