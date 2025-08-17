using Pagination;

namespace ViewModels
{
    public class WorkStepCategorySearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public override string OrderByColumn { get; set; } = "Name";
    }
}
