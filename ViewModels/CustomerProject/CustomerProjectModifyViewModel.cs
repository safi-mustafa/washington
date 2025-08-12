using Helpers.File;
using Models.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;

namespace ViewModels.CustomerProject
{
    public class CustomerProjectModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        public int? CustomerId { get; set; }
        public string JobName { get; set; }
        public string JobCode { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string WorkOrderNumber { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public string ProjectLocation { get; set; }
        public int? ProjectManagerId { get; set; }
        public string Notes { get; set; }
        public CustomerProjectBriefViewModel CustomerProject { get; set; } = new();
        public ProjectManagerBriefViewModel ProjectManager { get; set; } = new();
    }
}
