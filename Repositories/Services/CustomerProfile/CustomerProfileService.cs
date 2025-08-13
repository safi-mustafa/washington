using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Models;
using ViewModels.CustomerProfile;
using Repositories.Services.CustomerProfile.Interface;

namespace Repositories.Services.CustomerProfile
{
    public class CustomerProfileService : ICustomerProfileService
    {
        private readonly ApplicationDbContext _db;

        public CustomerProfileService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<CompanyProfileViewModel>> GetCompanyProfilesAsync()
        {
            return await _db.CompanyInformations
                .Where(c => c.ActiveStatus == Enums.ActiveStatus.Active && c.IsDeleted == false)
                .Select(ci => new CompanyProfileViewModel
                {
                    Name = ci.CompanyName,
                    Industry = ci.Industry ?? "",
                    Location = $"{ci.Street}, {ci.City}, {ci.State}, {ci.ZipCode}",
                    Phone = ci.Telephone ?? "",
                    Website = ci.WebAddress ?? "",
                    ContactsCount = ci.CompanyContacts
                        .Where(cc => cc.IsDeleted == false)
                        .Count(),
                    JobSitesCount = ci.JobSites
                        .Where(js => js.IsDeleted == false)
                        .Count(),
                    ProjectsCount = _db.CustomerProjects
                        .Where(cp => cp.IsDeleted == false &&
                                     cp.ActiveStatus == Enums.ActiveStatus.Active &&
                                     cp.CustomerId == ci.Id)
                        .Count(),
                    CreatedDate = ci.CreatedOn,
                    UpdatedDate = ci.UpdatedOn,
                    ViewUrl = "#",
                    EditUrl = $"/CustomerProfile/GetCompany/{ci.Id}",
                    DeleteUrl = $"/CustomerProfile/Delete/{ci.Id}"
                })
                .ToListAsync();
        }

        public async Task<bool> CreateCompanyAsync(CompanyFormData formData)
        {
            try
            {
                var companyInfo = new CompanyInformation
                {
                    CompanyName = formData.CompanyName,
                    Industry = formData.Industry,
                    Street = formData.Street,
                    City = formData.City,
                    State = formData.State,
                    ZipCode = formData.ZipCode,
                    Telephone = formData.Telephone,
                    WebAddress = formData.WebAddress,
                    ActiveStatus = Enums.ActiveStatus.Active,
                };

                _db.CompanyInformations.Add(companyInfo);
                await _db.SaveChangesAsync();

                foreach (var contact in formData.Contacts)
                {
                    var companyContact = new CompanyContact
                    {
                        CompanyInformationId = companyInfo.Id,
                        ContactPersonName = contact.Name,
                        Role = contact.Role,
                        PhoneNumber = contact.Phone,
                        EmailAddress = contact.Email,
                        ActiveStatus = Enums.ActiveStatus.Active
                    };
                    _db.CompanyContacts.Add(companyContact);
                }

                foreach (var jobSite in formData.JobSites)
                {
                    var site = new JobSite
                    {
                        CompanyInformationId = companyInfo.Id,
                        SiteName = jobSite.SiteName,
                        ContactPersonName = jobSite.ContactName,
                        ContactPersonEmail = jobSite.ContactEmail,
                        ContactPersonMobile = jobSite.ContactMobile,
                        SpecialNotes = jobSite.Notes,
                        ActiveStatus = Enums.ActiveStatus.Active
                    };
                    _db.JobSites.Add(site);
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCompanyAsync(long id)
        {
            try
            {
                var company = await _db.CompanyInformations
                    .Include(c => c.CompanyContacts)
                    .Include(c => c.JobSites)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company != null)
                {
                    company.IsDeleted = true;
                    
                    foreach (var contact in company.CompanyContacts)
                    {
                        contact.IsDeleted = true;
                    }
                    
                    foreach (var jobSite in company.JobSites)
                    {
                        jobSite.IsDeleted = true;
                    }
                    
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<object> GetCompanyAsync(long id)
        {
            var company = await _db.CompanyInformations
                .Include(c => c.CompanyContacts)
                .Include(c => c.JobSites)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company != null)
            {
                return new
                {
                    Id = company.Id,
                    CompanyName = company.CompanyName,
                    Industry = company.Industry,
                    Street = company.Street,
                    City = company.City,
                    State = company.State,
                    ZipCode = company.ZipCode,
                    Telephone = company.Telephone,
                    WebAddress = company.WebAddress,
                    Contacts = company.CompanyContacts.Select(c => new
                    {
                        Name = c.ContactPersonName,
                        Role = c.Role,
                        Phone = c.PhoneNumber,
                        Email = c.EmailAddress
                    }).ToList(),
                    JobSites = company.JobSites.Select(j => new
                    {
                        SiteName = j.SiteName,
                        ContactName = j.ContactPersonName,
                        ContactEmail = j.ContactPersonEmail,
                        ContactMobile = j.ContactPersonMobile,
                        Notes = j.SpecialNotes
                    }).ToList()
                };
            }
            return null;
        }

        public async Task<bool> UpdateCompanyAsync(CompanyUpdateData formData)
        {
            try
            {
                var company = await _db.CompanyInformations
                    .Include(c => c.CompanyContacts)
                    .Include(c => c.JobSites)
                    .FirstOrDefaultAsync(c => c.Id == formData.Id);

                if (company != null)
                {
                    company.CompanyName = formData.CompanyName;
                    company.Industry = formData.Industry;
                    company.Street = formData.Street;
                    company.City = formData.City;
                    company.State = formData.State;
                    company.ZipCode = formData.ZipCode;
                    company.Telephone = formData.Telephone;
                    company.WebAddress = formData.WebAddress;

                    _db.CompanyContacts.RemoveRange(company.CompanyContacts);
                    _db.JobSites.RemoveRange(company.JobSites);

                    foreach (var contact in formData.Contacts)
                    {
                        var companyContact = new CompanyContact
                        {
                            CompanyInformationId = company.Id,
                            ContactPersonName = contact.Name,
                            Role = contact.Role,
                            PhoneNumber = contact.Phone,
                            EmailAddress = contact.Email,
                            ActiveStatus = Enums.ActiveStatus.Active
                        };
                        _db.CompanyContacts.Add(companyContact);
                    }

                    foreach (var jobSite in formData.JobSites)
                    {
                        var site = new JobSite
                        {
                            CompanyInformationId = company.Id,
                            SiteName = jobSite.SiteName,
                            ContactPersonName = jobSite.ContactName,
                            ContactPersonEmail = jobSite.ContactEmail,
                            ContactPersonMobile = jobSite.ContactMobile,
                            SpecialNotes = jobSite.Notes,
                            ActiveStatus = Enums.ActiveStatus.Active
                        };
                        _db.JobSites.Add(site);
                    }

                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
