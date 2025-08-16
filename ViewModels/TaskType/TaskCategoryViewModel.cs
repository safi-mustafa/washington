using ViewModels.WorkOrderCategory;

namespace ViewModels
{
    public class TaskCategoryViewModel
    {
        public long Id { get; set; }
        public WorkOrderCategoryBriefViewModel WorkOrderCategory { get; set; } = new();
        public double Hours { get; set; }
        public double Rate { get; set; }
        public double Total { get => Rate * Hours; }
    }
}
