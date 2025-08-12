using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.ComponentModel;

namespace ViewModels
{
    public class ProjectManagerBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public ProjectManagerBriefViewModel() : base(false, "")
        {

        }
        [DisplayName("Role")]
        public string? Role { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Role;
            }
        }
    }
}