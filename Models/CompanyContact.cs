using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CompanyContact : BaseDBModel
    {
        [Required]
        public string ContactPersonName { get; set; }

        public string? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }

        [ForeignKey(nameof(CompanyInformation))]
        public long CompanyInformationId { get; set; }
        public CompanyInformation CompanyInformation { get; set; }
    }
}
