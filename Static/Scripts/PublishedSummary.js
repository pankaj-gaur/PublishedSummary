Type.registerNamespace("Alchemy.Plugins.PublishedSummary.Views");

Alchemy.Plugins.PublishedSummary.Views.PublishedSummary = function PublishedSummary() {
    Tridion.OO.enableInterface(this, "Alchemy.Plugins.PublishedSummary.Views.PublishedSummary");
    this.addInterface("Tridion.Cme.View");
};

Alchemy.Plugins.PublishedSummary.Views.PublishedSummary.prototype.initialize = function PublishedSummary$initialize() {
    this.callBase("Tridion.Cme.View", "initialize");
};

$display.registerView(Alchemy.Plugins.PublishedSummary.Views.PublishedSummary);


$(document).ready(function () {

    $.ajax({
        type: "GET",
        url: document.location.origin + "/Alchemy/Plugins/Published_Summary/api/Service/GetItemPublishedHistory",
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