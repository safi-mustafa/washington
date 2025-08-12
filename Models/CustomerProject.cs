using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CustomerProject : BaseDBModel
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
    }
}
