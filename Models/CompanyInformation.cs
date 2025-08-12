using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CompanyInformation : BaseDBModel
    {
        [Required]
        public string CompanyName { get; set; }

        public string? Industry { get; set; }

        // Address Fields
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }

        public string? Telephone { get; set; }
        public string? WebAddress { get; set; }

        // Navigation
        public List<CompanyContact> CompanyContacts { get; set; } = new();
        public List<JobSite> JobSites { get; set; } = new();
    }
}
