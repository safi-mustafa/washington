using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class CustomerProjectNotes : BaseDBModel
    {
        public string Description { get; set; }
        public string? FileUrl { get; set; }

        [ForeignKey("CustomerProjectId")]
        public long CustomerProjectId { get; set; }
        public CustomerProject CustomerProject { get; set; }
    }
}
