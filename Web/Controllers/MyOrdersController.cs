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

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> AllOrders()
        {

            try
            {
                var allOrders = await _myOrderService.GetAllOrders();
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
        public async Task<ActionResult> PendingOrders()
        {

            try
            {
                var pendingsOrders = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.PendingApproval);
                return PartialView("_PendingOrder", pendingsOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Pending Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ScheduledOrders()
        {

            try
            {
                var scheduledOrder = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.Scheduled);
                return PartialView("_ScheduledOrder", scheduledOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> DeliveredOrders()
        {

            try
            {
                var DeliveredOrders = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.Delivered);
                return PartialView("_DeliveredOrder", DeliveredOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> OffrentOrders()
        {

            try
            {
                var Offrent = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.OffRent);
                return PartialView("_OffrentOrder", Offrent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> OnrentOrders()
        {

            try
            {
                var Onrent = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.OnRent);
                return PartialView("_OnrentOrder", Onrent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Scheduled Orders threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ReturnedOrders()
        {

            try
            {
                var ReturnedOrders = await _myOrderService.GetAllOrders(0, (int)Enums.OrderConfirmStatusEnum.Returned);
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
    }
}
