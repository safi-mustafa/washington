$(function () {
    AllOrders();
});


$(function () {
    $("#pills-all-order-tab").on("click", function () {
        AllOrders();
    });

    $("#pills-pending-tab").on("click", function () {
        PendingOrders();
    });

    $("#pills-scheduled-tab").on("click", function () {
        ScheduledOrders();
    });

    $("#pills-delivered-tab").on("click", function () {
        DeliveredOrders();
    });

    $("#pills-onrent-tab").on("click", function () {
        OnrentOrders();
    });

    $("#pills-offrent-tab").on("click", function () {
        OffrentOrders();
    });

    $("#pills-returned-tab").on("click", function () {
        ReturnedOrders();
    });

    $(document).on('click', '.view-details-btn', function () {
        var orderId = $(this).data('order-id');
        $.get("MyOrders/ViewOrderDetailsModal",
            { orderId: orderId },
            function (data) {
                $('body').append(data);
                $('#viewOrderDetailsById').modal('show');
            });
    });

    $(document).on('hidden.bs.modal', '#viewOrderDetailsById', function () {
        $(this).remove();
    });

});

function AllOrders() {
    $.ajax({
        url: "/MyOrders/AllOrders",
        type: "GET",
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

function PendingOrders() {
    $.ajax({
        url: "/MyOrders/PendingOrders",
        type: "GET",
        success: function (data) {
            if (data) {
                $("#pills-pending").empty();
                $("#pills-pending").html(data);
            }
            else {
                SwalErrorAlert("AllOrders failed to load!");
            }
            $("#pendingApprovalOrderCount").text("");
        },
        complete: function () {
            var count = $("#hdnCountPendingOrder").val() != undefined ? $("#hdnCountPendingOrder").val() : 0;
            var totalAllOrderCount = "(" + count + ")";
            $("#pendingApprovalOrderCount").text(totalAllOrderCount);
        },
        error: function (r, s, t) {
            SwalErrorAlert('AllOrders failed to load!');
        }
    });
}

function ScheduledOrders() {
    $.ajax({
        url: "/MyOrders/ScheduledOrders",
        type: "GET",
        success: function (data) {
            if (data) {
                $("#pills-scheduled").empty();
                $("#pills-scheduled").html(data);
            }
            else {
                SwalErrorAlert("AllOrders failed to load!");
            }
            $("#scheduledCountOrderCount").text("");
        },
        complete: function () {
            var count = $("#hdnCountScheduleOrder").val() != undefined ? $("#hdnCountScheduleOrder").val() : 0;
            var totalAllOrderCount = "(" + count + ")";
            $("#scheduledCountOrderCount").text(totalAllOrderCount);
        },
        error: function (r, s, t) {
            SwalErrorAlert('AllOrders failed to load!');
        }
    });
}

function DeliveredOrders() {
    $.ajax({
        url: "/MyOrders/DeliveredOrders",
        type: "GET",
        success: function (data) {
            if (data) {
                $("#pills-delivered").empty();
                $("#pills-delivered").html(data);
            }
            else {
                SwalErrorAlert("Delivered Orders failed to load!");
            }
            $("#deliveredCountOrderCount").text("");
        },
        complete: function () {
            var count = $("#hdndeliveredCountOrderCount").val() != undefined ? $("#hdndeliveredCountOrderCount").val() : 0;
            var totalAllOrderCount = "(" + count + ")";
            $("#deliveredCountOrderCount").text(totalAllOrderCount);
        },
        error: function (r, s, t) {
            SwalErrorAlert('Delivered Orders failed to load!');
        }
    });
}

function OnrentOrders() {
    $.ajax({
        url: "/MyOrders/OnrentOrders",
        type: "GET",
        success: function (data) {
            if (data) {
                $("#pills-onrent").empty();
                $("#pills-onrent").html(data);
            }
            else {
                SwalErrorAlert("OnRent Orders failed to load!");
            }
            $("#onRentCountOrderCount").text("");
        },
        complete: function () {
            var count = $("#hdnonRentCountOrderCount").val() != undefined ? $("#hdnonRentCountOrderCount").val() : 0;
            var totalAllOrderCount = "(" + count + ")";
            $("#onRentCountOrderCount").text(totalAllOrderCount);
        },
        error: function (r, s, t) {
            SwalErrorAlert('OnRent Orders failed to load!');
        }
    });
}


function OffrentOrders() {
    $.ajax({
        url: "/MyOrders/OffrentOrders",
        type: "GET",
        success: function (data) {
            if (data) {
                $("#pills-offrent").empty();
                $("#pills-offrent").html(data);
            }
            else {
                SwalErrorAlert("OffRent Orders failed to load!");
            }
            $("#offRentCountOrderCount").text("");
        },
        complete: function () {
            var count = $("#hdnOffRentCountOrderCount").val() != undefined ? $("#hdnOffRentCountOrderCount").val() : 0;
            var totalAllOrderCount = "(" + count + ")";
            $("#offRentCountOrderCount").text(totalAllOrderCount);
        },
        error: function (r, s, t) {
            SwalErrorAlert('OffRent Orders failed to load!');
        }
    });
}

function ReturnedOrders() {
    $.ajax({
        url: "/MyOrders/ReturnedOrders",
        type: "GET",
        success: function (data) {
            if (data) {
                $("#pills-returned").empty();
                $("#pills-returned").html(data);
            }
            else {
                SwalErrorAlert("Returned Orders failed to load!");
            }
            $("#returnedCountOrderCount").text("");
        },
        complete: function () {
            var count = $("#hdnCountReturnOrder").val() != undefined ? $("#hdnCountReturnOrder").val() : 0;
            var totalAllOrderCount = "(" + count + ")";
            $("#returnedCountOrderCount").text(totalAllOrderCount);
        },
        error: function (r, s, t) {
            SwalErrorAlert('Returned Orders failed to load!');
        }
    });
}

function ChangeOrderStatus(value, orderId) {
    $.ajax({
        url: "/MyOrders/ChangeOrderStatus",
        type: "POST", // matches controller
        data: { status: value, orderId: orderId },
        success: function (data) {
            if (data.success) {

                SwalSuccessAlert("Order status updated successfully.");

                $("#returnedCountOrderCount").text("");
                $("#offRentCountOrderCount").text("");
                $("#onRentCountOrderCount").text("");
                $("#deliveredCountOrderCount").text("");
                $("#scheduledCountOrderCount").text("");
                $("#pendingApprovalOrderCount").text("");
                $("#allOrderCount").text("");

                $("#returnedCountOrderCount").text("(" + data.count.ReturnedCount + ")");
                $("#offRentCountOrderCount").text("(" + data.count.OffRentCount + ")");
                $("#onRentCountOrderCount").text("(" + data.count.OnRentCount + ")");
                $("#deliveredCountOrderCount").text("(" + data.count.DeliveredCount + ")");
                $("#scheduledCountOrderCount").text("(" + data.count.ScheduledCount + ")");
                $("#pendingApprovalOrderCount").text("(" + data.count.PendingApprovalCount + ")");
                $("#allOrderCount").text("(" + data.count.AllOrderCount + ")");

            } else {
                SwalErrorAlert('Failed: ' + data.error);
            }
        },
        complete: function () {
            AllOrders();
        },
        error: function (r, s, t) {
            SwalErrorAlert('Changes Orders failed to load!');
        }
    });
}
