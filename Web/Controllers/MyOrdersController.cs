using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;
using System.Collections.Generic;
using System;

namespace Web.Controllers
{
    public class MyOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
