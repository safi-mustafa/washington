$(function () {
    AllOrders();
});


$(function () {
    $("#pills-all-order-tab").on("click", function () {
        AllOrders();
    });

    $("#pills-pending-tab").on("click", function () {

    });

    $("#pills-scheduled-tab").on("click", function () {

    });

    $("#pills-delivered-tab").on("click", function () {

    });

    $("#pills-onrent-tab").on("click", function () {

    });

    $("#pills-offrent-tab").on("click", function () {

    });

    $("#pills-returned-tab").on("click", function () {

    });
});

function AllOrders(transactionId) {
    $.ajax({
        url: "/MyOrders/AllOrders",
        type: "GET",
        data: { "Id": transactionId },
        success: function (data) {
            if (data) {
                $("#pills-all-order").empty();
                $("#pills-all-order").html(data);
            }
            else {
                SwalErrorAlert("AllOrders failed to load!");
            }
            $("#allOrderCount").text("");
        },
        complete: function () {
            var count = $("#hdnCountAllOrder").val() != undefined ? $("#hdnCountAllOrder").val() : 0;
            var totalAllOrderCount = "(" + count + ")";
            $("#allOrderCount").text(totalAllOrderCount);
        },
        error: function (r, s, t) {
            SwalErrorAlert('AllOrders failed to load!');
        }
    });
}