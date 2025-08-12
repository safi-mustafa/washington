using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;

namespace ViewModels.CustomerProject
{
    public class CustomerProjectDetailViewModel:BaseCrudViewModel
    {
        public long Id { get; set; } = default!;
        public long CustomerId { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public string JobName { get; set; } = default!;
        public string JobCode { get; set; } = default!;
        public string PurchaseOrderNumber { get; set; } = default!;
        public string WorkOrderNumber { get; set; } = default!;
        public string ProjectStartDate { get; set; } = default!;
        public string ProjectEndDate { get; set; } = default!;
        public string ProjectLocation { get; set; } = default!;
        public string ProjectManagerName { get; set; } = default!;
        public int? ProjectManagerId { get; set; } = default!;
        public string Notes { get; set; } = default!;
        public CustomerProjectBriefViewModel Customer { get; set; } = new();
        public ProjectManagerBriefViewModel ProjectManager { get; set; } = new();
    }
}
