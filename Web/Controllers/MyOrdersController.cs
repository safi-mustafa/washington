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
    }
}
