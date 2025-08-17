using Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.MyOrder
{
    public class AllOrderViewModel
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; } = default!;
        public string OrderStatus { get; set; } = default!;
        public int? OrderStatusId { get; set; } = default!;
        public string Project { get; set; } = default!;
        public string ProjectId { get; set; } = default!;
        public string TotalCost { get; set; } = default!;
        public string TotalBudget { get; set; } = default!;
        public string UsageBudget { get; set; } = default!;
        public string RemainingBudget { get; set; } = default!;
        public string PercentageUsageBudget { get; set; } = default!;
        public string NumberUsageBudget { get; set; } = default!;
        public string OrderNotes { get; set; } = default!;
        public int TotalOrderItemCount { get; set; } = default!;
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string PurchanseOrderNumber { get; set; }
        public string WorkStepName { get; set; }
        public string Notes { get; set; }
        public List<OrderConfirmStatus> orderConfirmStatuses { get; set; } = default!;
        public List<OrderItemViewModel> orderItems { get; set; } = default!;
    }

    public class OrderItemViewModel
    {
        public long Id { get; set; }
        public string OrderItemPrice { get; set; }
        public long Quantity { get; set; } = 0;
        public string OrderItemName { get; set; }

        public string OrderStartDate { get; set; } = default!;
        public string MinStartDate { get; set; } = default!;
        public string OrderEndDate { get; set; } = default!;
        public string MaxEndDate { get; set; } = default!;
        public string? Total { get; set; } = "0";
        public string? Category { get; set; }
    }
}   
