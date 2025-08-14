using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class SubCategoryBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public SubCategoryBriefViewModel() : base(true, "The Sub Category field is required.")
        {

        }
        public SubCategoryBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Category")]
        public string? Name { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
    }

}
