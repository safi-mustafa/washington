using Microsoft.AspNetCore.Mvc;

using Repositories.Services.CustomerProfile.Interface;
using Repositories.Services.MyOrder.Interface;

using System;
using System.Collections.Generic;

using ViewModels.CRUD;
using ViewModels.Shared.Notes;

namespace Web.Controllers
{
    public class MyOrdersController : Controller
    {
        private readonly IMyOrderService _myOrderService;
        private readonly ILogger<MyOrdersController> _logger;

        public MyOrdersController(IMyOrderService myOrderService, ILogger<MyOrdersController> logger)
        {
            _myOrderService = myOrderService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var allOrdersCount = await _myOrderService.GetStatusCount();

            ViewBag.AllOrderCount = allOrdersCount.AllOrderCount;
            ViewBag.PendingApprovalCount = allOrdersCount.PendingApprovalCount;
            ViewBag.DeliveredCount = allOrdersCount.DeliveredCount;
            ViewBag.ScheduledCount = allOrdersCount.ScheduledCount;
            ViewBag.ReturnedCount = allOrdersCount.ReturnedCount;
            ViewBag.OnRentCount = allOrdersCount.OnRentCount;
            ViewBag.OffRentCount = allOrdersCount.OffRentCount;

            var allDrodowns = await _myOrderService.AllOrderDropDown();
            return View(allDrodowns);
        }

        [HttpGet]
        public async Task<ActionResult> AllOrders(string? keyword, string? customerId, string? projectId, string? poNumber)
        {

            try
            {
                var allOrders = await _myOrderService.GetAllOrders();
                if (!string.IsNullOrEmpty(keyword) || customerId != null || projectId != null || !string.IsNullOrEmpty(poNumber))
                {
                    if (customerId != null)
                    {
                        allOrders = allOrders.Where(o =>(o.CustomerId == customerId)).ToList();
                    }
                    if (projectId != null)
                    {
                        allOrders = allOrders.Where(o => (o.ProjectId == projectId)).ToList();
                    }
                    if(poNumber != null)
                    {
                        allOrders = allOrders.Where(o => o.PurchanseOrderNumber.Contains(poNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if(keyword != null)
                    {
                        allOrders = allOrders.Where(o =>
                            o.orderItems.Any(e =>
                                e.OrderItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }
                return PartialView("_AllOrder", allOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"All Order threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewOrderDetailsModal(int orderId)
        {
            try
            {
                var orderDetails = await _myOrderService.GetAllOrders(orderId);
                return PartialView("_ViewOrderDetailsModal", orderDetails.FirstOrDefault());
            }
            catch (Exception ex)
            {
                _logger.LogError($"ViewOrderDetailsModal threw an exception, Message: {ex.Message}");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult> PendingOrders(string? keyword, string? customerId, string? projectId, string? poNumber)
        {

            try
            {
                var pendingsOrders = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.PendingApproval);
                if (!string.IsNullOrEmpty(keyword) || customerId != null || projectId != null || !string.IsNullOrEmpty(poNumber))
                {
                    if (customerId != null)
                    {
                        pendingsOrders = pendingsOrders.Where(o => (o.CustomerId == customerId)).ToList();
                    }
                    if (projectId != null)
                    {
                        pendingsOrders = pendingsOrders.Where(o => (o.ProjectId == projectId)).ToList();
                    }
                    if (poNumber != null)
                    {
                        pendingsOrders = pendingsOrders.Where(o => o.PurchanseOrderNumber.Contains(poNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (keyword != null)
                    {
                        pendingsOrders = pendingsOrders.Where(o =>
                            o.orderItems.Any(e =>
                                e.OrderItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }
                return PartialView("_PendingOrder", pendingsOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Pending Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ScheduledOrders(string? keyword, string? customerId, string? projectId, string? poNumber)
        {

            try
            {
                var scheduledOrder = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.Scheduled);
                if (!string.IsNullOrEmpty(keyword) || customerId != null || projectId != null || !string.IsNullOrEmpty(poNumber))
                {
                    if (customerId != null)
                    {
                        scheduledOrder = scheduledOrder.Where(o => (o.CustomerId == customerId)).ToList();
                    }
                    if (projectId != null)
                    {
                        scheduledOrder = scheduledOrder.Where(o => (o.ProjectId == projectId)).ToList();
                    }
                    if (poNumber != null)
                    {
                        scheduledOrder = scheduledOrder.Where(o => o.PurchanseOrderNumber.Contains(poNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (keyword != null)
                    {
                        scheduledOrder = scheduledOrder.Where(o =>
                            o.orderItems.Any(e =>
                                e.OrderItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }
                return PartialView("_ScheduledOrder", scheduledOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> DeliveredOrders(string? keyword, string? customerId, string? projectId, string? poNumber)
        {

            try
            {
                var DeliveredOrders = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.Delivered);
                if (!string.IsNullOrEmpty(keyword) || customerId != null || projectId != null || !string.IsNullOrEmpty(poNumber))
                {
                    if (customerId != null)
                    {
                        DeliveredOrders = DeliveredOrders.Where(o => (o.CustomerId == customerId)).ToList();
                    }
                    if (projectId != null)
                    {
                        DeliveredOrders = DeliveredOrders.Where(o => (o.ProjectId == projectId)).ToList();
                    }
                    if (poNumber != null)
                    {
                        DeliveredOrders = DeliveredOrders.Where(o => o.PurchanseOrderNumber.Contains(poNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (keyword != null)
                    {
                        DeliveredOrders = DeliveredOrders.Where(o =>
                            o.orderItems.Any(e =>
                                e.OrderItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }
                return PartialView("_DeliveredOrder", DeliveredOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> OffrentOrders(string? keyword, string? customerId, string? projectId, string? poNumber)
        {

            try
            {
                var Offrent = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.OffRent);
                if (!string.IsNullOrEmpty(keyword) || customerId != null || projectId != null || !string.IsNullOrEmpty(poNumber))
                {
                    if (customerId != null)
                    {
                        Offrent = Offrent.Where(o => (o.CustomerId == customerId)).ToList();
                    }
                    if (projectId != null)
                    {
                        Offrent = Offrent.Where(o => (o.ProjectId == projectId)).ToList();
                    }
                    if (poNumber != null)
                    {
                        Offrent = Offrent.Where(o => o.PurchanseOrderNumber.Contains(poNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (keyword != null)
                    {
                        Offrent = Offrent.Where(o =>
                            o.orderItems.Any(e =>
                                e.OrderItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }
                return PartialView("_OffrentOrder", Offrent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> OnrentOrders(string? keyword, string? customerId, string? projectId, string? poNumber)
        {

            try
            {
                var Onrent = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.OnRent);
                if (!string.IsNullOrEmpty(keyword) || customerId != null || projectId != null || !string.IsNullOrEmpty(poNumber))
                {
                    if (customerId != null)
                    {
                        Onrent = Onrent.Where(o => (o.CustomerId == customerId)).ToList();
                    }
                    if (projectId != null)
                    {
                        Onrent = Onrent.Where(o => (o.ProjectId == projectId)).ToList();
                    }
                    if (poNumber != null)
                    {
                        Onrent = Onrent.Where(o => o.PurchanseOrderNumber.Contains(poNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (keyword != null)
                    {
                        Onrent = Onrent.Where(o =>
                            o.orderItems.Any(e =>
                                e.OrderItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }
                return PartialView("_OnrentOrder", Onrent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ReturnedOrders(string? keyword, string? customerId, string? projectId, string? poNumber)
        {

            try
            {
                var ReturnedOrders = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.Returned);
                if (!string.IsNullOrEmpty(keyword) || customerId != null || projectId != null || !string.IsNullOrEmpty(poNumber))
                {
                    if (customerId != null)
                    {
                        ReturnedOrders = ReturnedOrders.Where(o => (o.CustomerId == customerId)).ToList();
                    }
                    if (projectId != null)
                    {
                        ReturnedOrders = ReturnedOrders.Where(o => (o.ProjectId == projectId)).ToList();
                    }
                    if (poNumber != null)
                    {
                        ReturnedOrders = ReturnedOrders.Where(o => o.PurchanseOrderNumber.Contains(poNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                    if (keyword != null)
                    {
                        ReturnedOrders = ReturnedOrders.Where(o =>
                            o.orderItems.Any(e =>
                                e.OrderItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }

                }
                return PartialView("_ReturnedOrder", ReturnedOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus(int? status, int? orderId)
        {
            try
            {
                await _myOrderService.ChangeOrderStatus(status, orderId);

                var allOrdersCount = await _myOrderService.GetStatusCount();

                return Json(new { success = true, count = allOrdersCount });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ChangeOrderStatus threw an exception, Message: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                var result = await _myOrderService.DeleteOrder(orderId);
                if (result)
                {
                    return Json(new { success = true, message = "Order deleted successfully" });
                }
                return Json(new { success = false, message = "Order not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"DeleteOrder threw an exception, Message: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while deleting the order" });
            }
        }
    }
}
