// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

// ==================== [ Printing Function ] ====================

PrintJobDetail = (Header) => {

    $("#PrintHeader").html(Header);
    $("#PrintContent").printThis();
}


// ==================== [ Edit Product Toggle ] ====================

ToggleProductEditor = (Method) => {

    if (Method == "Enable")
    {
        $("#DeleteProduct").attr("hidden", true);
        $("#EnableEditProduct").attr("hidden", true);

        $("#DisableEditProduct").attr("hidden", false);
        $("#EditProductSubmit").attr("hidden", false);

        $("input#Name").attr("readonly", false);
        $("input#CostPrice").attr("readonly", false);
        $("input#SellingPrice").attr("readonly", false);
    }
    else
    {
        $("#DeleteProduct").attr("hidden", false);
        $("#EnableEditProduct").attr("hidden", false);

        $("#DisableEditProduct").attr("hidden", true);
        $("#EditProductSubmit").attr("hidden", true);

        $("input#Name").attr("readonly", true);
        $("input#CostPrice").attr("readonly", true);
        $("input#SellingPrice").attr("readonly", true);
    }
}


// ==================== [ EditStock Page Function ] ====================

var ArrayOfProductObject = [];
EditStockInputForm = (Id) => {

    var StockCount = $("#EditId" + Id).val();

    // Fix number = null or empty string
    if (StockCount == "") {
        StockCount = 0;
        $("#EditId" + Id).val(StockCount);
    }

    // Set Value of list
    var EditProductObject = {
        Id: Id,
        Count: StockCount
    }

    ArrayOfProductObject.push(EditProductObject);
    $("#ProductObject").val(JSON.stringify(ArrayOfProductObject));

    $.ajax({
        type: "GET",
        url: "/Stock/FindProductCount/" + Id
    }).done(function (result) {

        // Form Color Adjust
        if (result > StockCount) {
            $("#EditId" + Id).css({
                "background-color": "rgba(222, 49, 99, 0.2)"
            });
        }
        else if (result < StockCount) {
            $("#EditId" + Id).css({
                "background-color": "rgba(126, 239, 104, 0.2)"
            });
        }
        else {
            $("#EditId" + Id).removeAttr("style");
        }
    });
}


// ==================== [ Stock Calculator Add Sum ] ====================

AddSum = (addnum) => {
    $(document).ready(function () {

        var Operator = $("#SelectOperator").val();
        var sum;

        if (Operator == "+") {
            sum = parseInt($("#ClickForCalculationSum").val()) + parseInt(addnum);
        }
        else if (Operator == "-") {
            sum = parseInt(addnum) - parseInt($("#ClickForCalculationSum").val());
        }

        if (sum <= 0) {
            $("#calculation").html(0);
            $("#ResultNumber").val(0);
        }
        else if (isNaN(sum)) {
            $("#calculation").html(parseInt(addnum));
            $("#ResultNumber").val(addnum);
        }
        else {
            $("#calculation").html(sum);
            $("#ResultNumber").val(sum);
        }
    });
}


// ==================== [ Modal Showing ] ====================

ShowInPopup = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal #modal-dialog").removeClass("modal-xl");
            $("#form-modal .modal-body").html(res);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
        }
    })
}

ShowInLargePopup = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal #modal-dialog").addClass("modal-xl");
            $("#form-modal .modal-body").html(res);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
        }
    })
}


// ==================== [ Create or Update JobDetail ] ====================

ShowProductPopup = (url, title, method, JobId) => {

    if (method == "CreateJob") {
        var StringId = $("#StringProducts").val();

        if (StringId == "") {
            StringId = "Empty";
        }

        $.ajax({
            type: "GET",
            url: url + '/' + StringId,
            success: function (res) {
                $("#form-modal #modal-dialog").addClass("modal-xl");
                $("#form-modal .modal-body").html(res);
                $("#form-modal .modal-title").html(title);
                $("#form-modal").modal('show');
            }
        })
    }
    else {
        var StringId = $("#EditStringProducts").val();

        if (StringId == "") {
            StringId = "Empty";
        }

        $.ajax({
            type: "GET",
            url: url + '/' + StringId + '/' + JobId,
            success: function (res) {
                $("#form-modal #modal-dialog").addClass("modal-xl");
                $("#form-modal .modal-body").html(res);
                $("#form-modal .modal-title").html(title);
                $("#form-modal").modal('show');
            }
        })
    }
}

ChooseProduct = (id, method) => {

    if (method == "CreateJob") {
        var StringId;
        if ($("#StringProducts").val() != "") {
            StringId = $("#StringProducts").val() + "_" + id.toString();
        }
        else {
            StringId = id.toString();
        }

        $("#StringProducts").val(StringId);
        DisplayProduct(method);
    }
    else {
        var StringId;
        if ($("#EditStringProducts").val() != "") {
            StringId = $("#EditStringProducts").val() + "_" + id.toString();
        }
        else {
            StringId = id.toString();
        }

        $("#EditStringProducts").val(StringId);
        DisplayProduct(method);
    }
}

DisplayProduct = (method) => {

    if (method == "CreateJob") {
        var StringId = $("#StringProducts").val();
        if (StringId != "") {

            $.ajax({
                type: "GET",
                url: "/JobDetail/GetProductDetail/" + StringId
                //url: "/JobDetail/GetProductDetail/" + "?StringId=" + StringId
            }).done(function (result) {

                //console.log("Result : ");
                //console.log(result);

                var table = $('#DisplayProduct').DataTable({
                    searching: false, paging: false, info: false,
                    processing: true,
                    serverSide: false,
                    destroy: true,
                    ordering: false,
                    data: result,
                    columns: [
                    {
                        "data": "name",
                        render: function (data, type, obj) {
                            return '<div class="pt-1">' + data + '</div>'
                        }
                    },
                    {
                        "data": "productCount",
                        render: function (data, type, obj) {
                            return '<div class="text-end pt-1">' + data + ' ชิ้น</div>'
                            //return '<div class="text-end pt-1 d-inline-flex"><input class="form-control form-control-sm" type="number" value="' + data + '" id="ProductNumber" onkeyup="EditProductNumber()"><p class=""> ชิ้น</p></div>'
                        }
                    },
                    {
                        "data": "sellingPrice",
                        render: function (data, type, obj) {
                            return '<div class="text-end pt-1">' + data.toLocaleString("en-US") + ' บาท</div>'
                        }
                    },
                    {
                        data: "id",
                        render: function (data, type, obj) {
                            return '<div class="row d-flex justify-content-center"><div class="col-lg-2 col-4 p-0 text-center"><a onclick="AddProduct(\'' + data + '\', \'' + StringId + '\', \'' + method + '\')" class="btn btn-sm btn-outline-success border-0"><i class="fa-solid fa-plus"></i></a></div>' +
                                '<div class="col-lg-2 col-4 p-0 text-center mx-lg-1"><a onclick="DeleteProduct(\'' + data + '\', \'' + StringId + '\', \'' + method + '\')" class="btn btn-sm btn-outline-success border-0"><i class="fa-solid fa-minus"></i></a></div>' +
                                '<div class="col-lg-2 col-4 p-0 text-center"><a onclick="RemoveProduct(\'' + data + '\', \'' + StringId + '\', \'' + method + '\')" class="btn btn-sm btn-outline-danger border-0"><i class="fa-regular fa-trash-can"></i></a></div></div>'
                        }
                    },
                    ],
                });

                var TotalNumber = 0;
                result.forEach(element => TotalNumber += element.productCount);
                //console.log(TotalNumber);

                var TotalPrice = 0;
                result.forEach(element => TotalPrice += element.sellingPrice);
                //console.log(TotalPrice);

                var WagePrice = $("#JobWage").val();
                if (WagePrice == "") {
                    WagePrice = 0;
                }

                $("#TotalNumber").html(TotalNumber.toLocaleString("en-US") + " ชิ้น");
                $("#TotalPrice").html((parseInt(TotalPrice) + parseInt(WagePrice)).toLocaleString("en-US") + " บาท");
            })

            $("#form-modal").modal('hide');
        }
        else {
            var table = $('#DisplayProduct').DataTable();

            //clear and destroy datatable
            table.clear().draw();
            //table.destroy();

            var WagePrice = $("#JobWage").val();
            if (WagePrice == "") {
                WagePrice = 0;
            }

            $("#TotalNumber").html("0 ชิ้น");
            $("#TotalPrice").html(parseInt(WagePrice).toLocaleString("en-US") + " บาท");
        }
    }
    else {
        var StringId = $("#EditStringProducts").val();
        if (StringId != "") {

            $.ajax({
                type: "GET",
                url: "/JobDetail/GetProductDetail/" + StringId
                //url: "/JobDetail/GetProductDetail/" + "?StringId=" + StringId
            }).done(function (result) {

                //console.log("Result : ");
                //console.log(result);

                var table = $('#DisplayEditProduct').DataTable({
                    searching: false, paging: false, info: false,
                    processing: true,
                    serverSide: false,
                    destroy: true,
                    ordering: false,
                    data: result,
                    columns: [
                    {
                        "data": "name",
                        render: function (data, type, obj) {
                            return '<div class="pt-1">' + data + '</div>'
                        }
                    },
                    {
                        "data": "productCount",
                        render: function (data, type, obj) {
                            return '<div class="text-end pt-1">' + data + ' ชิ้น</div>'
                            //return '<div class="text-end pt-1 d-inline-flex"><input class="form-control form-control-sm" type="number" value="' + data + '" id="ProductNumber" onkeyup="EditProductNumber()"><p class=""> ชิ้น</p></div>'
                        }
                    },
                    {
                        "data": "sellingPrice",
                        render: function (data, type, obj) {
                            return '<div class="text-end pt-1">' + data.toLocaleString("en-US") + ' บาท</div>'
                        }
                    },
                    {
                        data: "id",
                        render: function (data, type, obj) {
                            return '<div class="row d-flex justify-content-center"><div class="col-lg-2 col-4 p-0 text-center"><a onclick="AddProduct(\'' + data + '\', \'' + StringId + '\', \'' + method + '\')" class="btn btn-sm btn-outline-success border-0"><i class="fa-solid fa-plus"></i></a></div>' +
                                '<div class="col-lg-2 col-4 p-0 text-center mx-lg-1"><a onclick="DeleteProduct(\'' + data + '\', \'' + StringId + '\', \'' + method + '\')" class="btn btn-sm btn-outline-success border-0"><i class="fa-solid fa-minus"></i></a></div>' +
                                '<div class="col-lg-2 col-4 p-0 text-center"><a onclick="RemoveProduct(\'' + data + '\', \'' + StringId + '\', \'' + method + '\')" class="btn btn-sm btn-outline-danger border-0"><i class="fa-regular fa-trash-can"></i></a></div></div>'
                        }
                    },
                    ],
                });

                var TotalNumber = 0;
                result.forEach(element => TotalNumber += element.productCount);
                //console.log(TotalNumber);

                var TotalPrice = 0;
                result.forEach(element => TotalPrice += element.sellingPrice);
                //console.log(TotalPrice);

                var WagePrice = $("#EditJobWage").val();
                if (WagePrice == "") {
                    WagePrice = 0;
                }

                $("#EditTotalNumber").html(TotalNumber.toLocaleString("en-US") + " ชิ้น");
                $("#EditTotalPrice").html((parseInt(TotalPrice) + parseInt(WagePrice)).toLocaleString("en-US") + " บาท");
            })

            $("#form-modal").modal('hide');
        }
        else {

            var table = $('#DisplayEditProduct').DataTable({
                searching: false,
                paging: false,
                info: false,
                destroy: true
            });

            //clear and destroy datatable
            table.clear().draw();
            //table.destroy();

            var WagePrice = $("#EditJobWage").val();
            if (WagePrice == "") {
                WagePrice = 0;
            }

            $("#EditTotalNumber").html("0 ชิ้น");
            $("#EditTotalPrice").html(parseInt(WagePrice).toLocaleString("en-US") + " บาท");
        }
    }
}

AddProduct = (id, StringId, method) => {

    if (method == "CreateJob") {
        $.ajax({
            type: "GET",
            url: "/JobDetail/GetProductStock/" + id
        }).done(function (result) {

            var ArrayId = StringId.split("_");
            var ProductCount = 0;

            for (var i = 0; i < ArrayId.length; ++i) {
                if (ArrayId[i] == id)
                    ProductCount++;
            }

            if (ProductCount < result.productCount) {
                StringId = StringId + '_' + id;
                $("#StringProducts").val(StringId);
            }

            DisplayProduct(method);
        });
    }
    else {
        var JobId = $('#JobIdForUpdate').val();

        $.ajax({
            type: "GET",
            url: "/JobDetail/UpdateGetProductStock/" + id + "/" + JobId
        }).done(function (result) {
            //console.log(result);

            var ArrayId = StringId.split("_");
            var ProductCount = 0;

            for (var i = 0; i < ArrayId.length; ++i) {
                if (ArrayId[i] == id)
                    ProductCount++;
            }

            if (ProductCount < result.productCount) {
                StringId = StringId + '_' + id;
                $("#EditStringProducts").val(StringId);
            }

            DisplayProduct(method);
        });
    }
}

DeleteProduct = (id, StringId, method) => {

    if (method == "CreateJob") {
        var ArrayId = StringId.split("_");
        ArrayId.reverse();

        var index = ArrayId.indexOf(id.toString());
        ArrayId.splice(index, 1);

        var ListStringId;
        ArrayId.reverse()

        for (let i = 0; i < ArrayId.length; i++) {
            if (ListStringId != null) {
                ListStringId = ListStringId + '_' + ArrayId[i];
            }
            else {
                ListStringId = ArrayId[i];
            }
        }

        $("#StringProducts").val(ListStringId);
        DisplayProduct(method);
    }
    else {
        var ArrayId = StringId.split("_");
        ArrayId.reverse();

        var index = ArrayId.indexOf(id.toString());
        ArrayId.splice(index, 1);

        var ListStringId;
        ArrayId.reverse()

        for (let i = 0; i < ArrayId.length; i++) {
            if (ListStringId != null) {
                ListStringId = ListStringId + '_' + ArrayId[i];
            }
            else {
                ListStringId = ArrayId[i];
            }
        }

        $("#EditStringProducts").val(ListStringId);
        DisplayProduct(method);
    }
}

RemoveProduct = (id, StringId, method) => {

    var ArrayId = StringId.split("_");
    var FilteredId = ArrayId.filter(Id => Id != id);

    var ListStringId;
    for (let i = 0; i < FilteredId.length; i++) {
        if (ListStringId != null) {
            ListStringId = ListStringId + '_' + FilteredId[i];
        }
        else {
            ListStringId = FilteredId[i];
        }
    }

    if (method == "CreateJob") {
        $("#StringProducts").val(ListStringId);
        DisplayProduct(method);
    }
    else {
        $("#EditStringProducts").val(ListStringId);
        DisplayProduct(method);
    }
}

AddWagePrice = (method) => {

    DisplayProduct(method);
}


// ==================== [ Find Customer location for Create JobDetail ] ====================

FindCompanyInfo = (CompanyName) => {

    if (CompanyName != "" && $("#CustomerName").val() == "") {
        $.ajax({
            type: "GET",
            url: "/JobDetail/FindCompanyInfo/" + CompanyName
        }).done(function (result) {

            if (result != "" && result != null && result != undefined) {

                $("#CompanyName").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                })

                $("#CustomerName").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                }).val(result.customerName);

                $("#JobLocation").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                }).val(result.jobLocation);

                $("#CustomerPhoneNumber").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                }).val(result.customerPhoneNumber);

                $("#TaxId").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                }).val(result.taxId);
            }
            else {
                $("#CustomerName").removeAttr("style");
            }
        });
    }
    else {
        $("#CompanyName").removeAttr("style");
    }
}

FindCustomerInfo = (CustomerName) => {

    if (CustomerName != "" && $("#CompanyName").val() == "") {
        $.ajax({
            type: "GET",
            url: "/JobDetail/FindCustomerInfo/" + CustomerName
        }).done(function (result) {

            if (result != "" && result != null && result != undefined) {
                $("#CustomerName").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                })

                $("#JobLocation").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                }).val(result.jobLocation);

                $("#CustomerPhoneNumber").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                }).val(result.customerPhoneNumber);

                $("#TaxId").css({
                    "background-color": "rgba(126, 239, 104, 0.2)"
                }).val(result.taxId);
            }
            else {
                $("#CustomerName").removeAttr("style");
            }
        });
    }
    else {
        $("#CustomerName").removeAttr("style");
    }
}

StyleRemove = (Id) => {
    $("#" + Id).removeAttr("style");
}


// ==================== [ Product Datatable ] ====================

//$("#ProductPreview").ready(() => {

//    if (window.location.pathname == "/Product/DashBoard") {

//        $('#ProductTable').DataTable({

//            stateSave: true,
//            lengthMenu: [
//                [20, 50, 80, 100],
//                [20, 50, 80, 100],
//            ],

//            columnDefs: [{
//                "searchable": false,
//                "orderable": false,
//                "targets": 5,
//            }],
//            // Fix SortingDate Bug
//            //"columnDefs": [{
//            //    "searchable": true,
//            //    "orderable": true,
//            //    "targets": 4,
//            //    "type": 'date'
//            //}]
//        });
//    }
//})


// ==================== [ Stock Datatable ] ====================

$("#StockPreview").ready(() => {

    if (window.location.pathname == "/Stock/StockPreview") {

        $('#StockTable').DataTable({

            stateSave: true,
            columnDefs: [{
                "searchable": false,
                "orderable": false,
                "targets": 3,
            }],

            lengthMenu: [
                [10, 25, 50, 100],
                [10, 25, 50, 100],
            ],
        });
    }
})

$("#StockEdit").ready(() => {

    if (window.location.pathname == "/Stock/StockPreview")
    {
        $('#EditStockTable').DataTable({

            stateSave: true,
            //order: [[4, "desc"], [0, "asc"]], //or asc
            columnDefs: [{
                "searchable": false,
                "orderable": false,
                "targets": 1,
            }],

            lengthMenu: [
                [10, 25, 50, 100],
                [10, 25, 50, 100],
                //[20, 50, 80, 100],
                //[20, 50, 80, 100],
            ]
        });
    }
})


// ==================== [ StockHistory Datatable ] ====================

$("#StockHistory").ready(() => {

    if (window.location.pathname == "/History/HistoryPreview")
    {
        DisplayHistoryTable();
    }
})

DisplayHistoryTable = () => {

    var Checkbox = [];
    var Selecter = $("#MonthFilter").val();

    if ($("#IncreaseCheckbox").is(":checked")) {
        Checkbox.push("Increase");
    }

    if ($("#DecreaseCheckbox").is(":checked")) {
        Checkbox.push("Decrease");
    }

    if ($("#AddCheckbox").is(":checked")) {
        Checkbox.push("Add");
    }

    $.ajax({
        type: "GET",
        url: "/History/GetStockHistory/" + Selecter + "/" + Checkbox
    }).done(function (result) {

        $("#HistoryCount").html(result.length);

        //console.log(result);
        //console.log(result[0].actionId);

        $('#HistoryTable').DataTable({

            //"pageLength": 50,
            lengthMenu: [
                [20, 50, 100, 200],
                [20, 50, 100, 200],
            ],

            order: [[4, "desc"], [0, "asc"]], //or asc
            stateSave: true,
            destroy: true,
            data: result,
            columns: [{
                data: "productName",
                className: 'align-middle text-break',
                render: function (data, type, obj) {
                    return '<div class="text-break">' + data + '</div>'
                },
            },
            {
                data: "actionName",
                className: 'align-middle text-center text-break',
                render: function (data, type, obj) {

                    //console.log(obj)
                    if (obj.actionId == 2 || obj.actionId == 7) {
                        return '<div class="text-center align-middle text-break bg-success bg-opacity-25 rounded-pill">' + data + '</div>'
                    }

                    else if (obj.actionId == 3 || obj.actionId == 6) {
                        return '<div class="text-center align-middle text-break bg-danger bg-opacity-25 rounded-pill">' + data + '</div>'
                    }

                    else if (obj.actionId == 4) {
                        return '<div class="text-center align-middle text-break bg-secondary bg-opacity-25 rounded-pill">' + data + '</div>'
                    }

                    else {
                        return '<div class="text-center align-middle text-break bg-primary bg-opacity-25 rounded-pill">' + data + '</div>'
                    }
                }
            },
            {
                data: "actionNumber",
                className: 'align-middle text-center text-break',
                render: function (data, type, obj) {

                    if (obj.actionId == 2 || obj.actionId == 7) {
                        return '<div class="text-center align-middle text-break"><strong>+</strong>' + data + '</div>'
                    }
                    else if (obj.actionId == 3 || obj.actionId == 6) {
                        return '<div class="text-center align-middle text-break"><strong>-</strong>' + data + '</div>'
                    }
                    else {
                        return '<div class="text-center align-middle text-break">-</div>'
                    }
                }
            },
            {
                data: "jobNo",
                className: 'align-middle text-center text-nowrap',
                render: function (data, type, obj) {

                    if (data == null || data == "") {
                        return '<div class="text-center align-middle text-break">-</div>'
                    }

                    return '<div class="text-center align-middle text-break">' +
                        '<a id="JobDetailLink" class="text-decoration-none" href="/JobDetail/JobDetailPreview?JobId=' + data + '">' + data + '</a>' +
                        '</div>'
                }
            },
            {
                data: "stringDisplayDate",
                className: 'align-middle text-center text-break',
                render: function (data, type, obj) {
                    return '<div class="text-center align-middle text-break">' + data + '<span hidden>' + obj.remark + '</span>' + '</div>'
                }
            },
            ],

            //initComplete: function () {
            //    this.api()
            //        .columns(0)
            //        .every(function () {
            //            var column = this;
            //            var select = $('<select><option value=""></option></select>')
            //                .appendTo($(column.header()).empty())
            //                .on('change', function () {
            //                    var val = $.fn.dataTable.util.escapeRegex($(this).val());

            //                    column.search(val ? '^' + val + '$' : '', true, false).draw();
            //                });

            //            column
            //                .data()
            //                .unique()
            //                .sort()
            //                .each(function (d, j) {
            //                    select.append('<option value="' + d + '">' + d + '</option>');
            //                });
            //        });
            //},
        });

        $('[type=search]').each(function () {
            $(this).attr("placeholder", "ค้นหาทุกหัวข้อ");
            $(this).removeClass("form-control-sm");
        });

        $('.form-select').each(function () {
            $(this).removeClass("form-select-sm");
        });
    });
}


// ==================== [ JobDetail Datatable ] ====================

$("#CreateJob").ready(() => {

    $('#DisplayProduct').DataTable({
        ordering: false,
        searching: false,
        paging: false,
        info: false
    })
})

$("#EditJob").ready(() => {

    if (window.location.pathname.includes("/JobDetail/EditJobDetail"))
    {
        DisplayProduct('UpdateJob');
    }
})

$("#JobPreview").ready(() => {
    DisplayJobTable();
})

DisplayJobTable = () => {

    if (window.location.pathname == "/JobDetail/JobPreview") {

        var JobStatusSelector = $("#JobStatusSelector").val();

        $.ajax({
            type: "GET",
            url: "/JobDetail/GetJob/" + JobStatusSelector
        }).done(function (result) {

            //console.log(result)
            $("#JobCount").html(result.length);

            $('#JobTable').DataTable({

                order: [[4, "desc"], [0, "asc"]], //or asc
                //"columnDefs": [{ "targets": 2, "type": 'date' }],
                stateSave: true,
                destroy: true,
                data: result,
                columns: [{
                    data: "jobId",
                    className: 'align-middle text-break text-nowrap',
                },
                {
                    data: "companyName",
                    className: 'align-middle text-center text-break user-select-all',
                },
                {
                    data: "customerName",
                    className: 'align-middle text-center text-break user-select-all',
                },
                {
                    data: "jobStatusName",
                    className: 'align-middle text-center text-break',
                    render: function (data, type, obj) {

                        //console.log(obj)
                        if (obj.jobStatusId == 1) {
                            return '<p class="m-0 py-2 rounded-pill border bg-secondary bg-opacity-25">' + data + '</p>'
                        }

                        else if (obj.jobStatusId == 5) {
                            return '<p class="m-0 py-2 rounded-pill border bg-success bg-opacity-25">' + data + '</p>'
                        }

                        else {
                            return '<p class="m-0 py-2 rounded-pill border bg-warning bg-opacity-25">' + data + '</p>'
                        }
                    }
                },
                {
                    data: "stringDisplayDate",
                    className: 'align-middle text-center text-break',
                },
                {
                    data: "jobId",
                    className: 'align-middle text-center text-break',
                    render: function (data, type, obj) {

                        return '<a class="btn btn-primary opacity-75 shadow" href="/JobDetail/JobDetailPreview?JobId=' + data + '">รายละเอียด</a>'
                    }
                },
                ]
            });

            $('[type=search]').each(function () {
                $(this).attr("placeholder", "ค้นหาทุกหัวข้อ");
                $(this).removeClass("form-control-sm");
            });

            $('.form-select').each(function () {
                $(this).removeClass("form-select-sm");
            });
        })
    }
}

UpdateJobStatus = (JobId, StatusInput) => {

    $.ajax({
        type: "GET",
        url: "/JobDetail/UpdateJobStatus/" + JobId + '/' + StatusInput 
    });
}


// ==================== [ JobHistory Datatable ] ====================

$("#JobHistory").ready(() => {

    if (window.location.pathname == "/History/JobHistoryPreview") {

        DisplayJobHistoryTable();
    }
})

DisplayJobHistoryTable = () => {

    if (window.location.pathname == "/History/JobHistoryPreview") {
        var StatusFilter = $("#StatusFilter").val();
        var MonthFilter = $("#MonthFilter").val();

        $.ajax({
            type: "GET",
            url: "/History/GetJobHistory/" + StatusFilter + "/" + MonthFilter
        }).done(function (result) {

            $("#JobHistoryCount").html(result.length);

            $('#JobHistoryTable').DataTable({

                data: result,
                destroy: true,
                stateSave: true,
                lengthMenu: [
                    [20, 50, 100, 200],
                    [20, 50, 100, 200],
                ],

                //language: {
                //    searchPlaceholder: "ค้นหาทุกหัวข้อ"
                //},

                //"pageLength": 50,

                order: [[3, "desc"], [0, "asc"]], //or asc
                //"columnDefs": [{ "targets": 4, "type": 'date' }],

                columns: [{
                    "data": "jobNo",
                    className: 'align-middle text-break',
                    render: function (data, type, obj) {

                        return '<div class="text-break">' +
                            '<a id="JobDetailLink" class="text-decoration-none" href="/JobDetail/JobDetailPreview?JobId=' + data + '">' + data + '</a>' +
                            '</div>'
                    },
                },
                {
                    "data": "actionName",
                    className: 'align-middle text-center text-break',
                    render: function (data, type, obj) {

                        if (obj.actionId == 5) {
                            return '<div class="text-center bg-success bg-opacity-25 rounded-pill">' + data + '</div>'
                        }

                        else if (obj.actionId == 1) {
                            return '<div class="text-center align-middle text-break bg-secondary bg-opacity-25 rounded-pill">' + data + '</div>'
                        }

                        else {
                            return '<div class="text-center align-middle text-break bg-warning bg-opacity-25 rounded-pill">' + data + '</div>'
                        }
                    }
                },
                {
                    data: "productCount",
                    className: 'align-middle text-center text-break',
                    render: function (data, type, obj) {
                        return '<div class="text-center align-middle text-break fw-bold">' + data + '</div>'
                    }
                },
                    {
                    data: "stringDisplayDate",
                    className: 'align-middle text-center text-break',
                    render: function (data, type, obj) {
                        return '<div class="text-center align-middle text-break">' + data + '<span hidden>' + obj.remark + '</span>' + '</div>'
                    }
                },
                ]
            });

            $('[type=search]').each(function () {
                $(this).attr("placeholder", "ค้นหาทุกหัวข้อ");
                $(this).removeClass("form-control-sm");
            });

            $('.form-select').each(function () {
                $(this).removeClass("form-select-sm");
            });
        });
    }
}


// ==================== [ Document.ready ] ====================

$(document).ready(function () {

    //$("input").removeClass("form-select-sm");

    $('[type=search]').each(function () {
        $(this).attr("placeholder", "ค้นหาทุกหัวข้อ");
        $(this).removeClass("form-control-sm");
        //$("input").addClass("w-100");
    });

    $('.form-select').each(function () {
        $(this).removeClass("form-select-sm");
    });

    //$("label").addClass("fw-bold");
});

CloseFileModal = () => {
    $("#form-modal").modal('hide');
}