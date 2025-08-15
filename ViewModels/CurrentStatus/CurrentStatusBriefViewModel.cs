using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class CurrentStatusBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public CurrentStatusBriefViewModel() : base(true, "The Current Status field is required.")
        {

        }
        public CurrentStatusBriefViewModel(bool isValidationEnabled,string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Current Status")]
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
