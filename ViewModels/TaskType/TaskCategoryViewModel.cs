using ViewModels.WorkStepCategory;

namespace ViewModels
{
    public class TaskCategoryViewModel
    {
        public long Id { get; set; }
        public WorkStepCategoryBriefViewModel WorkOrderCategory { get; set; } = new();
        public string Description { get; set; } = default!;
        public double Qty { get; set; }
        public double Rate { get; set; }
        public double Total { get => Rate * Qty; }
    }
}
