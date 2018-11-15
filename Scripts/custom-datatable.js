//$(document).ready(function () {
//    $('#example').DataTable({
//        ajax: "Content/files/data.json",
//        columns: [
//            { data: 'name' },
//            { data: 'position' },
//            { data: 'office' },
//            { data: 'extn' },
//            { data: 'start_date' },
//            { data: 'salary' }
//        ],
//        order: [[2, 'asc']],
//        rowGroup: {
//            dataSrc: 'office'
//        }
//    });
//});
$(document).ready(function ()
{
    //var colModel = [
    //    { "data": "Sr" },
    //    { "data": "OrderTrackNumber" },
    //    { "data": "Quantity" },
    //    { "data": "ProductName" },
    //    { "data": "SpecialOffer" },
    //    { "data": "UnitPrice" },
    //    { "data": "UnitPriceDiscount" }
    //];


    var colModel;
    $.get("/Plugin/GetColumnsDefinition", function (result) {
        $(".result").html(result);       

        $('#TableId').DataTable(
            {
                "scrollX": true,
                //"columnDefs": [
                //    { "width": "5%", "targets": [0] },
                //   // { "className": "text-center custom-middle-align", "targets": [0, 1, 2, 3, 4, 5, 6] },
                //],
                "language":
                {
                    "processing": "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
                },
                "processing": true,
                "serverSide": true,
                "ajax":
                {
                    "url": "/Plugin/GetData",
                    "type": "POST",
                    "dataType": "JSON"
                },
                "columns": result.data,
                order: [[3, 'asc']],
                rowGroup: {
                    startRender: null,
                    endRender: function (rows, group) {
                        var aa = 10;

                        return $('<tr/>')
                            .append('<td colspan="3">Averages for ' + group + '</td>')
                            .append('<td/>');

                    },

                    dataSrc: '3'
                },
                "drawCallback": function (settings) {
                    var api = this.api();
                    var rows = api.rows({ page: 'current' }).nodes();
                    var last = null;
                    var totalCount = 0;
                    var total = 0;
                    api.column(3, { page: 'current' }).data().each(function (group, i) {
                        if (last !== group) {
                            var price = CalculateUnitPrice(api.rows({ page: 'current' }).data(), last);
                            if (i != 0) {
                                $(rows).eq(i).before(
                                    '<tr class="group"><td colspan="5">Total ' + last + '</td><td colspan="2" class="float-right">(' + price.toFixed(2) + ' ) </td><td></td></tr>'
                                );
                                total = totalCount;
                                totalCount = 0;
                            }
                            $(rows).eq(i).before(
                                '<tr class="group-head"><td colspan="7">' + group + ' (' + GroupCount(api.rows({ page: 'current' }).data(), group) + ' ) </td></tr>'
                            );
                            last = group;
                        }
                    });
                },
                dom: 'Bfrtip',
                buttons: [
                    //'copy', 'csv', 'excel', 'pdf', 'print'
                    {
                        extend: 'pdf',
                        text: 'pdf',
                        filename: function () { return "test.pdf"; },
                        messageTop: "Testing"
                    },
                    {
                        extend: 'csv',
                        text: 'csv',
                        filename: function () { return "test.csv"; }
                    },
                    {
                        extend: 'excel',
                        text: 'excel',
                        filename: function () { return "test.xls"; }
                    },
                    {
                        extend: 'print',
                        text: 'print',
                        filename: function () { return "test.pdf"; }
                    }
                ]

            });
    });

    
    


    function CalculateUnitPrice(ob, group) {
        var i;
        var price = 0;        
        for (i = 0; i < ob.count(); i++) {            
            if (group === ob[i]['ProductName']) {
                price = price + ob[i]['UnitPrice'];
            }
        }
        return price;
    }
    function GroupCount(ob, group) {
        var i;
        var count = 0;
        for (i = 0; i < ob.count(); i++) {
            if (group === ob[i]['ProductName']) {
                ++count;
            }
        }
        return count;
    }

});
