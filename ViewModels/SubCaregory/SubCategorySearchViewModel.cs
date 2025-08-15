using Pagination;

namespace ViewModels
{
    public class SubCategorySearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public long? CategoryId { get; set; }

        public override string OrderByColumn { get; set; } = "Name";
    }
}