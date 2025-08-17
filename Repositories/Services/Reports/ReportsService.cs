using DataLibrary;
using DocumentFormat.OpenXml.InkML;
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

        public async Task<GetCostbyOnRentModel> GetCostbyOnRent()
        {
            var today = DateTime.Today;

            var hasScheduledOrders = await _db.Orders
                .AnyAsync(o => o.Status == OrderStatus.Canceled);

            if (!hasScheduledOrders)
            {
                return new GetCostbyOnRentModel
                {
                    TotalOrderCost = 0,
                    OldestStartDate = today,
                    LatestEndDate = today,
                    PercentageComplete = 0
                };
            }

            var totalCost = await (
                from o in _db.Orders
                where o.Status == OrderStatus.Canceled
                select o.Cost
            ).SumAsync();

            var dateStats = await (
                from o in _db.Orders
                join oi in _db.OrderItems on o.Id equals oi.OrderId
                where o.Status == OrderStatus.Canceled
                group oi by 1 into g
                select new
                {
                    OldestStartDate = g.Min(x => x.StartDate),
                    LatestEndDate = g.Max(x => x.EndDate)
                }
            ).FirstOrDefaultAsync();

            if (dateStats == null)
                return null;

            var totalDays = (dateStats.LatestEndDate - dateStats.OldestStartDate).GetValueOrDefault().Days;
            var daysPassed = (today - dateStats.OldestStartDate).GetValueOrDefault().Days;

            decimal percentageComplete;
            if (totalDays == 0)
            {
                percentageComplete = today.Date == dateStats.OldestStartDate.GetValueOrDefault().Date ? 100m : 0m;
            }
            else if (today <= dateStats.OldestStartDate)
            {
                percentageComplete = 0m;
            }
            else if (today >= dateStats.LatestEndDate)
            {
                percentageComplete = 100m;
            }
            else
            {
                percentageComplete = (decimal)daysPassed * 100m / totalDays;
            }

            return new GetCostbyOnRentModel
            {
                TotalOrderCost = totalCost,
                OldestStartDate = dateStats.OldestStartDate,
                LatestEndDate = dateStats.LatestEndDate,
                PercentageComplete = Math.Round(percentageComplete, 2)
            };
        }

        public async Task<GetCostbyDeliveredModel> GetCostbyDelivered()
        {
            var today = DateTime.Today;

            var hasScheduledOrders = await _db.Orders
                .AnyAsync(o => o.Status == OrderStatus.PastDue);

            if (!hasScheduledOrders)
            {
                return new GetCostbyDeliveredModel
                {
                    TotalOrderCost = 0,
                    OldestStartDate = today,
                    LatestEndDate = today,
                    PercentageComplete = 0
                };
            }

            var totalCost = await (
                from o in _db.Orders
                where o.Status == OrderStatus.PastDue
                select o.Cost
            ).SumAsync();

            var dateStats = await (
                from o in _db.Orders
                join oi in _db.OrderItems on o.Id equals oi.OrderId
                where o.Status == OrderStatus.PastDue
                group oi by 1 into g
                select new
                {
                    OldestStartDate = g.Min(x => x.StartDate),
                    LatestEndDate = g.Max(x => x.EndDate)
                }
            ).FirstOrDefaultAsync();

            if (dateStats == null)
                return null;

            var totalDays = (dateStats.LatestEndDate - dateStats.OldestStartDate).GetValueOrDefault().Days;
            var daysPassed = (today - dateStats.OldestStartDate).GetValueOrDefault().Days;

            decimal percentageComplete;
            if (totalDays == 0)
            {
                percentageComplete = today.Date == dateStats.OldestStartDate.GetValueOrDefault().Date ? 100m : 0m;
            }
            else if (today <= dateStats.OldestStartDate)
            {
                percentageComplete = 0m;
            }
            else if (today >= dateStats.LatestEndDate)
            {
                percentageComplete = 100m;
            }
            else
            {
                percentageComplete = (decimal)daysPassed * 100m / totalDays;
            }

            return new GetCostbyDeliveredModel
            {
                TotalOrderCost = totalCost,
                OldestStartDate = dateStats.OldestStartDate,
                LatestEndDate = dateStats.LatestEndDate,
                PercentageComplete = Math.Round(percentageComplete, 2)
            };
        }

        public async Task<GetCostbyScheduledModel> GetCostbyScheduled()
        {
            var today = DateTime.Today;

            var hasScheduledOrders = await _db.Orders
                .AnyAsync(o => o.Status == OrderStatus.Delivered);

            if (!hasScheduledOrders)
            {
                return new GetCostbyScheduledModel
                {
                    TotalOrderCost = 0,
                    OldestStartDate = today,
                    LatestEndDate = today,
                    PercentageComplete = 0
                };
            }

            var totalCost = await (
                from o in _db.Orders
                where o.Status == OrderStatus.Delivered
                select o.Cost
            ).SumAsync();

            var dateStats = await (
                from o in _db.Orders
                join oi in _db.OrderItems on o.Id equals oi.OrderId
                where o.Status == OrderStatus.Delivered
                group oi by 1 into g
                select new
                {
                    OldestStartDate = g.Min(x => x.StartDate),
                    LatestEndDate = g.Max(x => x.EndDate)
                }
            ).FirstOrDefaultAsync();

            if (dateStats == null)
                return null;

            var totalDays = (dateStats.LatestEndDate - dateStats.OldestStartDate).GetValueOrDefault().Days;
            var daysPassed = (today - dateStats.OldestStartDate).GetValueOrDefault().Days;

            decimal percentageComplete;
            if (totalDays == 0)
            {
                percentageComplete = today.Date == dateStats.OldestStartDate.GetValueOrDefault().Date ? 100m : 0m;
            }
            else if (today <= dateStats.OldestStartDate)
            {
                percentageComplete = 0m;
            }
            else if (today >= dateStats.LatestEndDate)
            {
                percentageComplete = 100m;
            }
            else
            {
                percentageComplete = (decimal)daysPassed * 100m / totalDays;
            }

            return new GetCostbyScheduledModel
            {
                TotalOrderCost = totalCost,
                OldestStartDate = dateStats.OldestStartDate,
                LatestEndDate = dateStats.LatestEndDate,
                PercentageComplete = Math.Round(percentageComplete, 2)
            };
        }

        public async Task<GetCostbyPendingModel> GetCostbyPending()
        {
            var today = DateTime.Today;

            var hasScheduledOrders = await _db.Orders
                .AnyAsync(o => o.Status == OrderStatus.DeliveryScheduled);

            if (!hasScheduledOrders)
            {
                return new GetCostbyPendingModel
                {
                    TotalOrderCost = 0,
                    OldestStartDate = today,
                    LatestEndDate = today,
                    PercentageComplete = 0
                };
            }

            var totalCost = await (
                from o in _db.Orders
                where o.Status == OrderStatus.DeliveryScheduled
                select o.Cost
            ).SumAsync();

            var dateStats = await (
                from o in _db.Orders
                join oi in _db.OrderItems on o.Id equals oi.OrderId
                where o.Status == OrderStatus.DeliveryScheduled
                group oi by 1 into g
                select new
                {
                    OldestStartDate = g.Min(x => x.StartDate),
                    LatestEndDate = g.Max(x => x.EndDate)
                }
            ).FirstOrDefaultAsync();

            if (dateStats == null)
                return null;

            var totalDays = (dateStats.LatestEndDate - dateStats.OldestStartDate).GetValueOrDefault().Days;
            var daysPassed = (today - dateStats.OldestStartDate).GetValueOrDefault().Days;

            decimal percentageComplete;
            if (totalDays == 0)
            {
                percentageComplete = today.Date == dateStats.OldestStartDate.GetValueOrDefault().Date ? 100m : 0m;
            }
            else if (today <= dateStats.OldestStartDate)
            {
                percentageComplete = 0m;
            }
            else if (today >= dateStats.LatestEndDate)
            {
                percentageComplete = 100m;
            }
            else
            {
                percentageComplete = (decimal)daysPassed * 100m / totalDays;
            }

            return new GetCostbyPendingModel
            {
                TotalOrderCost = totalCost,
                OldestStartDate = dateStats.OldestStartDate,
                LatestEndDate = dateStats.LatestEndDate,
                PercentageComplete = Math.Round(percentageComplete, 2)
            };
        }
    }
}