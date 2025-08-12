using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.ComponentModel;

namespace ViewModels
{
    public class CustomerProjectBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public CustomerProjectBriefViewModel() : base(false, "")
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