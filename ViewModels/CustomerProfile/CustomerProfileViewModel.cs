namespace ViewModels.CustomerProfile
{
    public class CustomerProfileViewModel
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string ContentId { get; set; }
        public string AddNewCompanyProfileUrl { get; set; }
        public IEnumerable<CompanyProfileViewModel> CompanyProfiles { get; set; }
    }
    public class CompanyProfileViewModel
    {
        public string Name { get; set; }
        public string Industry { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public int ContactsCount { get; set; }
        public int JobSitesCount { get; set; }
        public int ProjectsCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ViewUrl { get; set; }
        public string EditUrl { get; set; }
        public string DeleteUrl { get; set; }
    }

    public class CompanyUpdateData : CompanyFormData
    {
        public long Id { get; set; }
    }

    public class CompanyFormData
    {
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Telephone { get; set; }
        public string WebAddress { get; set; }
        public List<ContactData> Contacts { get; set; } = new();
        public List<JobSiteData> JobSites { get; set; } = new();
    }

    public class ContactData
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class JobSiteData
    {
        public string SiteName { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobile { get; set; }
        public string Notes { get; set; }
    }
}
