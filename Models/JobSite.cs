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
    public class JobSite : BaseDBModel
    {
        [Required]
        public string SiteName { get; set; }

        public string? ContactPersonName { get; set; }
        public string? ContactPersonEmail { get; set; }
        public string? ContactPersonMobile { get; set; }
        public string? SpecialNotes { get; set; }

        [ForeignKey(nameof(CompanyInformation))]
        public long CompanyInformationId { get; set; }
        public CompanyInformation CompanyInformation { get; set; }
    }
}
