using DataLibrary;
using DocumentFormat.OpenXml.Spreadsheet;
using Enums;
using Microsoft.EntityFrameworkCore;
using Repositories.Services.Report.Interface;
using Repositories.Services.Reports.Interface;
using ViewModels.CRUD;

namespace Repositories.Services.Reports
{
    public class ReportsService : IReportsService

    {
        private readonly ApplicationDbContext _db;

        public ReportsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ReportsCountViewModel> Orders()
        {
            var viewModel = new ReportsCountViewModel
            {
                TotalOrders = await _db.Orders.CountAsync(o => !o.IsDeleted),
                TotalCost = (decimal)await _db.Orders.Where(o => !o.IsDeleted).SumAsync(o => o.Cost),
                TotalActiveRentals = await _db.OrderItems.Where(item => !item.IsDeleted &&
                   DateTime.Now >= item.StartDate &&
                   DateTime.Now <= item.EndDate)
                   .CountAsync()
            };

            // Correct fix: Return a new List containing the single viewModel object.
            return viewModel;
        }
        public async Task<List<ActiveRentalsModel>> GetActiveRentals()
        {
            var activeRentals = await (
                    from oi in _db.OrderItems
                    join o in _db.Orders on oi.OrderId equals o.Id
                    join e in _db.Equipments on oi.EquipmentId equals e.Id
                    join cp in _db.CustomerProjects on o.CustomerProjectId equals cp.Id
                    where !oi.IsDeleted
                        && oi.EndDate.HasValue
                        && EF.Functions.DateDiffDay(DateTime.Now, oi.EndDate.Value) >= 0
                        && EF.Functions.DateDiffDay(DateTime.Now, oi.EndDate.Value) <= 5
                    select new ActiveRentalsModel // Projecting directly into ActiveRentalsModel
                    {
                        Order = o.OrderNumber,
                        Item = e.ItemNo,
                        Project = cp.JobName,
                        DueDate = oi.EndDate.Value.Date,
                        DaysLeft = EF.Functions.DateDiffDay(DateTime.Now, oi.EndDate.Value).ToString() + " days left",
                        DailyCost = o.Cost.ToString()
                    }
                    ).ToListAsync();


            return activeRentals;
        }
        public async Task<List<CustomerProjectsViewModel>> GetCustomerProjects()
        {
            var customerProjects = await _db.CustomerProjects
                .GroupJoin(
                    _db.Orders,
                    cp => cp.Id,
                    o => o.CustomerProjectId,
                    (cp, orders) => new { Project = cp, Orders = orders }
                )
                .SelectMany(
                    x => x.Orders.DefaultIfEmpty(),
                    (x, o) => new
                    {
                        Project = x.Project,
                        Cost = o.Cost
                    }
                )
                .GroupBy(x => x.Project)
                .Select(g => new CustomerProjectsViewModel
                {
                    JobName = g.Key.JobName,
                    TotalCost = (g.Sum(x => x.Cost) ?? 0).ToString(),
                    ProjectStartDate = g.Key.ProjectStartDate,
                    ProjectEndDate = g.Key.ProjectEndDate,
                    // Calculate the percentage, then round up
                    PercentageComplete = g.Key.ProjectStartDate.HasValue && g.Key.ProjectEndDate.HasValue &&
                                         EF.Functions.DateDiffDay(g.Key.ProjectStartDate.Value, g.Key.ProjectEndDate.Value) > 0
                        ? (decimal?)Math.Ceiling(
                            (float)EF.Functions.DateDiffDay(g.Key.ProjectStartDate.Value, DateTime.Now) /
                             EF.Functions.DateDiffDay(g.Key.ProjectStartDate.Value, g.Key.ProjectEndDate.Value) * 100
                          )
                        : (decimal?)null
                })
                .ToListAsync();

            return customerProjects;
        }
    }
}