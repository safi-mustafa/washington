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
        public IActionResult Index()
        {
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
