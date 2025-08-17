using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.WorkStepCategory
{
    public class WorkStepCategoryBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public WorkStepCategoryBriefViewModel() : base(true, "The Category field is required.")
        {

        }
        public WorkStepCategoryBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
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
