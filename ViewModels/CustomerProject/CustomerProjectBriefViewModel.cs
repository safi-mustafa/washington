using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.ComponentModel;

namespace ViewModels
{
    public class CustomerProjectBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public CustomerProjectBriefViewModel() : base(true, "Customer is required")
        {

        }

        public CustomerProjectBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }

        [DisplayName("Name")]
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