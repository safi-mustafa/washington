using Microsoft.AspNetCore.Mvc;
using Repositories.Services.Reports;
using Repositories.Services.Reports.Interface;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IReportsService _reportsService;
        public ReportsController(
            ILogger<ReportsController> logger
            , IReportsService reportsService)
        {
            _logger = logger;
            _reportsService = reportsService;
        }
        public async Task<IActionResult> Index(string activeTab)
        {
            var tab = new TabViewModel()
            {
                Id = "report-tab",
                ActiveTab = activeTab,
                ContentId = "report-tab-content-Id",
                Title = "Report",
                TabItems = new List<TabItemViewModel>()
                {
                    new TabItemViewModel()
                    {
                        Id="maintenance-report",
                        Name="Maintenance",
                        Url="/Report/Maintenance"

                    },
                    new TabItemViewModel()
                    {
                        Id="replacement-report",
                        Name="Replacement",
                        Url="/Report/Replacement"

                    },
                    new TabItemViewModel()
                    {
                        Id="time-sheet-report",
                        Name="TimeSheet",
                        Url="/Report/TimeSheet"

                    },
                    new TabItemViewModel()
                    {
                        Id="transaction-report",
                        Name="Transaction",
                        Url="/Report/Transaction"

                    }
                }
            };
            var result = await _reportsService.Orders();
            var result1 = await _reportsService.GetActiveRentals();
            var result2 = await _reportsService.GetCustomerProjects();
            //return View(result);
            var viewModel = new ReportMasterViewModel
            {
                TabData = tab,
                ReportsCount = result,
                ActiveRentals = result1,
                CustomerProjects = result2
            };
            return View(viewModel);
        }
    }
}
