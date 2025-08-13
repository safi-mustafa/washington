using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.ComponentModel;

namespace ViewModels
{
    public class ProjectManagerBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ProjectManagerBriefViewModel() : base(true, "Project manager is required.")
        {

        }
        public ProjectManagerBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }

        [DisplayName("ContactPersonName")]
        public string? ContactPersonName { get; set; }

        public override string? Select2Text
        {
            get
            {
                return ContactPersonName;
            }
        }
    }
}