using Pagination;
using ViewModels.Charts.Shared;
using ViewModels.Dashboard.Common.Card;
using ViewModels.Dashboard.Common.Table;
using ViewModels.Dashboard.interfaces;

namespace ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {

        }

        public ChartViewModel PendingOrder { get; set; } = new();
        public ChartViewModel WorkOrder { get; set; } = new();
        public ChartViewModel WorkOrderByAssetType { get; set; } = new();
        public ChartViewModel WorkOrderByRepairType { get; set; } = new();
        public ChartViewModel WorkOrderByTechnician { get; set; } = new();
        public ChartViewModel WorkOrderByManager { get; set; } = new();
        public DashboardCardViewModel AverageCompletionTime { get; set; } = new("average-completion-time", "Average Completion Time", "/Home/GetAverageOrderCompletionTimeCardData");
        public ChartViewModel AssetMaintenanceDue { get; set; } = new();
        public ChartViewModel AssetReplacementDue { get; set; } = new();
        public ChartViewModel AssetByCondition { get; set; } = new();
        public DashboardCardViewModel GetAverageLaborCost { get; set; } = new("average-labor-cost", "Average Labor Cost", "/Home/GetAverageLaborCostCardData");
        public DashboardCardViewModel GetAverageEquipmentCost { get; set; } = new("average-equipment-time", "Average Equipment Cost", "/Home/GetAverageEquipmentCostCardData");
        public DashboardCardViewModel GetAverageMaterialCost { get; set; } = new("average-asset-time", "Average Material Cost", "/Home/GetAverageMaterialCostCardData");
        public ChartViewModel GetCostAccuracy { get; set; } = new();
        public DashboardCardViewModel PendingOrderCardData { get; set; } = new("pending-order-overview", "Pending Order Overview", "/Home/GetPendingOrderCardData");
        public DashboardTableViewModel PendingOrderTableData { get; set; } = new("pending-recent-order", "Recent Pending Order", "/Home/GetPendingOrderTableData");
    }

    public class DashboardSearchViewModel : BaseSearchModel, ITimePeriodSearch
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

}
