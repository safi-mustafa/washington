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
            var rawProjectData = await (
                        from cp in _db.CustomerProjects
                        join o in _db.Orders on cp.Id equals o.CustomerProjectId into orderGroup
                        from og in orderGroup.DefaultIfEmpty()
                        group og by new
                        {
                            cp.Id,
                            cp.JobName,
                            cp.ProjectStartDate,
                            cp.ProjectEndDate
                        } into g
                        select new
                        {
                            g.Key.JobName,
                            g.Key.ProjectStartDate,
                            g.Key.ProjectEndDate,
                            TotalCost = g.Sum(x => x != null ? x.Cost : 0)
                        }
                    ).ToListAsync();

            // Now compute PercentageComplete safely in-memory
            var projectCosts = rawProjectData.Select(p =>
            {
                double? percentageComplete = null;

                if (p.ProjectStartDate.HasValue && p.ProjectEndDate.HasValue)
                {
                    var totalDays = (p.ProjectEndDate.Value - p.ProjectStartDate.Value).TotalDays;
                    var daysElapsed = (DateTime.Now - p.ProjectStartDate.Value).TotalDays;

                    if (totalDays <= 0)
                    {
                        percentageComplete = 100;
                    }
                    else if (daysElapsed < 0)
                    {
                        percentageComplete = 0;
                    }
                    else
                    {
                        percentageComplete = Math.Min(100, (daysElapsed / totalDays) * 100);
                    }
                }

                return new CustomerProjectsViewModel
                {
                    JobName = p.JobName,
                    ProjectStartDate = p.ProjectStartDate,
                    ProjectEndDate = p.ProjectEndDate,
                    TotalCost = p.TotalCost,
                    PercentageComplete = percentageComplete
                };
            }).ToList();


            return projectCosts;
        }
    }
}