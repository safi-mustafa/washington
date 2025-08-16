using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;
using System.Collections.Generic;
using System;

namespace Web.Controllers
{
    public class ReportTableController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
