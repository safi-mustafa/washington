using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TransactionsNotes : BaseDBModel
    {
        public string Description { get; set; }
        public string? FileUrl { get; set; }

        [ForeignKey("Transaction")]
        public long TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
