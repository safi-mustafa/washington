using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TaskCategory : BaseDBModel
    {
        public string Description { get; set; }

        [ForeignKey("WorkStepCategory")]
        public long? WorkStepCategoryId { get; set; }
        public WorkStepCategory? WorkStepCategory { get; set; }

        [ForeignKey("TaskType")]
        public long TaskTypeId { get; set; }
        public TaskType TaskType { get; set; }

        public int Qty { get; set; }
        public string Rate { get; set; }
        public string Total { get; set; }
    }
}
