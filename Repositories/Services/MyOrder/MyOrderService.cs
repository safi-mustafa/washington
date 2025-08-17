using DataLibrary;

using Microsoft.EntityFrameworkCore;

using Models;

using Repositories.Services.MyOrder.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.MyOrder;

namespace Repositories.Services.MyOrder
{
    public class MyOrderService : IMyOrderService
    {
        private readonly ApplicationDbContext _db;
        public MyOrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<AllOrderViewModel>> GetAllOrders(int? orderId = 0)
        {
            try
            {
                List<AllOrderViewModel> allOrderViewModels = new();

                var orders = orderId > 0 ? await _db.Orders.Where(x=>x.Id == orderId && !x.IsDeleted).ToListAsync() : await _db.Orders.Where(x => !x.IsDeleted).ToListAsync();

                var orderIds = orders.Select(p => p.Id).ToList();

                var orderItems = await _db.OrderItems.Include(p => p.Equipment).Include(p => p.Inventory)
                    .Where(oi => orderIds.Contains(oi.OrderId))
                    .ToListAsync();

                var orderConfirmStatus = await _db.OrderConfirmStatus
                    .ToListAsync();

                var projects = await _db.CustomerProjects
                    .ToListAsync();

                foreach (var order in orders)
                {
                    var OrderStatus = string.Empty;
                    var projectsName = string.Empty;
                    var customerName = string.Empty;
                    var purchanseOrderNumber = string.Empty;
                    var workStepName = string.Empty;
                    var notes = order.Notes != null ? order.Notes : string.Empty ;
                    var projectId = string.Empty;
                    var customerId = string.Empty;


                    var orderItemsForOrder = order.Status != null ? orderConfirmStatus.Where(p => p.Id == (int)order.Status).FirstOrDefault() : null;
                    if (orderItemsForOrder != null)
                    {
                        OrderStatus = orderItemsForOrder.Name;
                    }

                    var projectsOrder = order.CustomerProjectId != null
                                    ? projects.FirstOrDefault(p => p.Id == order.CustomerProjectId)
                                    : null;
                    if (projectsOrder != null)
                    {
                        projectsName = projectsOrder.JobName;
                        purchanseOrderNumber = projectsOrder.PurchaseOrderNumber;
                        projectId = Convert.ToString(projectsOrder.Id);
                    }

                    var customer = projectsOrder != null ? await _db.CompanyInformations.FirstOrDefaultAsync(p => p.Id == projectsOrder.CustomerId) : null;

                    if (customer != null)
                    {
                        customerName = customer.CompanyName;
                        customerId = customer.Id.ToString();
                    }

                    var TaskType = await _db.TaskTypes.FindAsync(order.WorkOrderId);
                    if (TaskType != null)
                    {
                        workStepName = TaskType.Code;
                    }

                    var orderItemsList = orderItems.Where(p => p.OrderId == order.Id).ToList();

                    var percentageBudget = GetBudgetUsagePercantage(Convert.ToString(order.Cost), orderItemsList, Convert.ToString(order.Cost));

                    var numberUsageBudget = percentageBudget;

                    percentageBudget = percentageBudget + "%";


                    var usageBudget = GetUsageCost(Convert.ToString(order.Cost), orderItemsList);
                    usageBudget = !string.IsNullOrWhiteSpace(usageBudget) ? Convert.ToDouble(usageBudget).ToString("F2") : "0";

                    var totalBudget = order.Cost != null ? Convert.ToDouble(order.Cost).ToString("F2") : "0";

                    var remainingBudget = RemainingCost(Convert.ToString(order.Cost), orderItemsList);
                    remainingBudget = !string.IsNullOrWhiteSpace(remainingBudget) ? Convert.ToDouble(remainingBudget).ToString("F2") : "0";


                    var minStartDate = orderItemsList
                    .Where(x => x.StartDate.HasValue)
                    .Select(x => x.StartDate.Value)
                    .DefaultIfEmpty()
                    .Min();

                    var maxEndDate = orderItemsList
                        .Where(x => x.EndDate.HasValue)
                        .Select(x => x.EndDate.Value)
                        .DefaultIfEmpty()
                        .Max();

                    //    orderItems = orderItemsList.Select(p => new OrderItemViewModel()
                    //    {
                    //        Id = p.Id,
                    //                OrderItemName = p.EquipmentId > 0
                    //? Convert.ToString(p.Equipment.Description)
                    //: Convert.ToString(p.Inventory.Description),

                    //                OrderItemPrice = p.EquipmentId > 0
                    //? Convert.ToString(p.Equipment.TotalValue)
                    //: Convert.ToString(p.Inventory.UnitCost),

                    //                OrderStartDate = p.StartDate.HasValue
                    //? p.StartDate.Value.ToString("yyyy-MM-dd")
                    //: string.Empty,

                    //                OrderEndDate = p.EndDate.HasValue
                    //? p.EndDate.Value.ToString("yyyy-MM-dd")
                    //: string.Empty,

                    //                Quantity = p.Quantity,

                    //                // new props
                    //                MinStartDate = minStartDate != default(DateTime)
                    //? minStartDate.ToString("yyyy-MM-dd")
                    //: string.Empty,

                    //                MaxEndDate = maxEndDate != default(DateTime)
                    //? maxEndDate.ToString("yyyy-MM-dd")
                    //: string.Empty

                    //    }).ToList();


                    var orderViewModel = new AllOrderViewModel
                    {
                        Id = order.Id,
                        OrderNumber = order.OrderNumber,
                        OrderStatus = OrderStatus,
                        OrderStatusId = (int)order.Status,
                        Project = projectsName,
                        TotalCost = Convert.ToString(order.Cost),
                        TotalBudget = totalBudget,
                        //UsageBudget = orderItemsList.Count > 0 ? (orderItemsList[0].InventoryId > 0 ? "100" : usageBudget) : "0",
                        UsageBudget = usageBudget,
                        RemainingBudget = remainingBudget,
                        PercentageUsageBudget = percentageBudget,
                        NumberUsageBudget = numberUsageBudget,
                        OrderNotes = order.Notes,
                        TotalOrderItemCount = orderItems.Count(oi => oi.OrderId == order.Id),
                        orderConfirmStatuses = orderConfirmStatus,
                        CustomerName = customerName,
                        PurchanseOrderNumber = purchanseOrderNumber,
                        WorkStepName = workStepName,
                        Notes = notes,
                        CustomerId = customerId,
                        ProjectId = projectId,
                        orderItems = orderItemsList.Select(p => new OrderItemViewModel()
                        {
                            Id = p.Id,
                            OrderItemName = p.EquipmentId > 0 ? Convert.ToString(p.Equipment.Description) : Convert.ToString(p.Inventory.Description),
                            OrderItemPrice = p.EquipmentId > 0 ? Convert.ToString(p.Equipment.TotalValue) : Convert.ToString(p.Inventory.UnitCost),
                            OrderStartDate = minStartDate != null ? minStartDate.ToString("yyyy-MM-dd") : string.Empty,
                            OrderEndDate = maxEndDate != null ? maxEndDate.ToString("yyyy-MM-dd") : string.Empty,
                            Quantity = p.Quantity,
                            Total = (p.EquipmentId > 0 ? (p.Quantity * p.Equipment.TotalValue) : (p.Quantity * (double.TryParse(p.Inventory.UnitCost, out var unitCost) ? unitCost : 0))).ToString(),
                            Category = p.EquipmentId > 0 ? "Equipment" : "Inventory",
                        }).ToList()
                    };
                    allOrderViewModels.Add(orderViewModel);
                }


                return allOrderViewModels;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        private double? CalculateDailyCost(string totalCost, List<OrderItem> orderItemsList)
        {
            try
            {
                if (orderItemsList == null || orderItemsList.Count == 0)
                    return null;

                if (!double.TryParse(totalCost, out double parsedCost))
                    return null;

                DateTime startDate;
                DateTime endDate;

                if (orderItemsList.Count == 1)
                {
                    var firstItem = orderItemsList[0];

                    if (firstItem.InventoryId == null)
                    {
                        if (!firstItem.StartDate.HasValue || !firstItem.EndDate.HasValue)
                            return null;

                        startDate = firstItem.StartDate.Value;
                        endDate = firstItem.EndDate.Value;
                    }
                    else if (firstItem.EquipmentId == null)
                    {
                        if (!firstItem.StartDate.HasValue)
                            return null;

                        startDate = firstItem.StartDate.Value;
                        endDate = firstItem.StartDate.Value; // treat EndDate = StartDate
                    }
                    else
                    {
                        if (!firstItem.StartDate.HasValue || !firstItem.EndDate.HasValue)
                            return null;

                        startDate = firstItem.StartDate.Value;
                        endDate = firstItem.EndDate.Value;
                    }
                }
                else
                {
                    // multiple items → use min StartDate and max EndDate across all items
                    var validStartDates = orderItemsList
                        .Where(x => x.StartDate.HasValue)
                        .Select(x => x.StartDate.Value)
                        .ToList();

                    var validEndDates = orderItemsList
                        .Where(x => x.EndDate.HasValue)
                        .Select(x => x.EndDate.Value)
                        .ToList();

                    if (!validStartDates.Any() || !validEndDates.Any())
                        return null;

                    startDate = validStartDates.Min();
                    endDate = validEndDates.Max();
                }

                var totalDays = (endDate - startDate).TotalDays + 1;
                if (totalDays <= 0)
                    return null;

                return parsedCost / totalDays; // daily cost
            }
            catch
            {
                return 0;
            }
        }

        public string GetUsageCost(string cost, List<OrderItem> orderItemsList)
        {
            try
            {
                var dailyCost = CalculateDailyCost(cost, orderItemsList);
                if (dailyCost == null)
                    return cost;

                DateTime startDate;
                //DateTime endDate;
                //double maxDate;

                if (orderItemsList.Count == 1)
                {
                    var firstItem = orderItemsList[0];

                    if (firstItem.InventoryId == null)
                    {
                        if (!firstItem.StartDate.HasValue)
                            return cost;

                        startDate = firstItem.StartDate.Value;
                        //endDate = firstItem.EndDate.Value;
                        //maxDate = endDate.Subtract(startDate).TotalDays;
                    }
                    else if (firstItem.EquipmentId == null)
                    {
                        if (!firstItem.StartDate.HasValue)
                            return cost;

                        startDate = firstItem.StartDate.Value; // same for start & end
                        //endDate = firstItem.StartDate.Value;
                        //maxDate = endDate.Subtract(startDate).TotalDays;
                    }
                    else
                    {
                        if (!firstItem.StartDate.HasValue)
                            return cost;

                        startDate = firstItem.StartDate.Value;
                        
                        //endDate = firstItem.StartDate.Value;
                        //maxDate = endDate.Subtract(startDate).TotalDays;
                    }
                }
                else
                {
                    // multiple items → use earliest start date
                    var validStartDates = orderItemsList
                        .Where(x => x.StartDate.HasValue)
                        .Select(x => x.StartDate.Value)
                        .ToList();

                    if (!validStartDates.Any())
                        return cost;

                    startDate = validStartDates.Min();
                }

                var totalDaysFromToday = ((DateTime.Now - startDate).TotalDays + 1);
                if (totalDaysFromToday <= 0)
                    return cost;

                var usageCost = dailyCost.Value * totalDaysFromToday;
                return usageCost.ToString(); // keep it clean, 2 decimals
            }
            catch
            {
                return string.Empty;
            }
        }


        public string RemainingCost(string totalCost, List<OrderItem> orderItemsList)
        {
            try
            {
                var dailyCost = CalculateDailyCost(totalCost, orderItemsList);
                if (dailyCost == null) return totalCost;

                DateTime startDate;

                if (orderItemsList.Count == 1)
                {
                    var firstItem = orderItemsList[0];

                    if (firstItem.InventoryId == null)
                    {
                        if (!firstItem.StartDate.HasValue)
                            return totalCost;

                        startDate = firstItem.StartDate.Value;
                    }
                    else if (firstItem.EquipmentId == null)
                    {
                        if (!firstItem.StartDate.HasValue)
                            return totalCost;

                        startDate = firstItem.StartDate.Value; // same for start & end
                    }
                    else
                    {
                        if (!firstItem.StartDate.HasValue)
                            return totalCost;

                        startDate = firstItem.StartDate.Value;
                    }
                }
                else
                {
                    // multiple items → use earliest start date
                    var validStartDates = orderItemsList
                        .Where(x => x.StartDate.HasValue)
                        .Select(x => x.StartDate.Value)
                        .ToList();

                    if (!validStartDates.Any())
                        return totalCost;

                    startDate = validStartDates.Min();
                }

                var totalDaysFromToday = (DateTime.Now - startDate).TotalDays + 1;
                if (!double.TryParse(totalCost, out double parsedCost))
                    return totalCost;

                var usedCost = dailyCost.Value * totalDaysFromToday;
                var remainingCost = parsedCost - usedCost;

                return remainingCost.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetBudgetUsagePercantage(string cost, List<OrderItem> orderItemsList, string totalBudget)
        {
            try
            {
                string usedBudget = GetUsageCost(cost, orderItemsList);
                if (string.IsNullOrWhiteSpace(usedBudget)) return "0";


                decimal totalBudgetDecimal = Convert.ToDecimal(totalBudget);

                if (totalBudgetDecimal <= 0) return "0";

                var percentage = (Convert.ToDecimal(usedBudget) / totalBudgetDecimal) * 100;
                return percentage.ToString("0");
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<bool> DeleteOrder(int orderId)
        {
            try
            {
                var order = await _db.Orders.FindAsync((long)orderId);
                if (order != null)
                {
                    order.IsDeleted = true;
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<AllOrderDropDownViewModel> AllOrderDropDown()
        {
            AllOrderDropDownViewModel allOrderDropDownViewModel = new AllOrderDropDownViewModel();
            try
            {
                allOrderDropDownViewModel.Customers = await _db.CompanyInformations
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();
                allOrderDropDownViewModel.Projects = await _db.CustomerProjects
                    .Where(p => !p.IsDeleted)
                    .ToListAsync();
                return allOrderDropDownViewModel;
            }
            catch (Exception EX)
            {
                return allOrderDropDownViewModel;
            }
        }
    }
}
