using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Subcategory : BaseDBModel
    {
        public string Name { get; set; }
        [ForeignKey("CategoryId")]

        public long? CategoryId { get; set; }

        public Category Category { get; set; }

    }
}
