<html>
<head>
    <title>Published Summary</title>

</head>
<body>
    <link rel="stylesheet" href="../css/style.css">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.10.2/jquery-ui.js" type="text/javascript"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

    <script>
        $(document).ready(function () {
            $.ajax({
                type: "GET",
                url: document.location.origin + "/Alchemy/Plugins/PublishedSummary/api/Service/GetPublicationList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                    $.each(response, function (i, obj) {
                        var div_data = "<option value=" + obj.id + ">" + obj.title + "</option>";
                        $(div_data).appendTo('#ddlPubList');
                    });

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
                type: "GET",
                url: document.location.origin + "/Alchemy/Plugins/PublishedSummary/api/Service/AjaxGetComponent",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                    alert(response);
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });

        });
    </script>
    <div class="container">
        <form id="form1" runat="server">
            <div class="form-group row">
                <label for="lgFormGroupInput" class="col-sm-2 col-form-label col-form-label-lg">Select Publication :-</label>
                <div class="col-sm-10">
                    <div class="bootstrap-select">
                        <select name="pubList" class="selectpicker" id="ddlPubList">
                            <option selected="selected" value="all">--Select Publiaction--</option>
                        </select>
                    </div>
                </div>
            </div>
        </form>
    </div>

</body>
</html>
