using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class SubCategoryDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}