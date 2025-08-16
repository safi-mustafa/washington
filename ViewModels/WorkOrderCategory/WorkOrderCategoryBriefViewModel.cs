using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.WorkOrderCategory
{
    public class WorkOrderCategoryBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public WorkOrderCategoryBriefViewModel() : base(true, "The Category field is required.")
        {

        }
        public WorkOrderCategoryBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
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
