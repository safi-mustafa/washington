using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;
using System.Collections.Generic;
using System;

namespace Web.Controllers
{
    public class CustomerProfileController : Controller
    {
        public IActionResult Create()
        {
            // Remove TabItems, provide static example list for company profiles
            var model = new TabViewModel
            {
                Title = "Company Profiles",
                Id = "customer-profile-list",
                ContentId = "customer-profile-list-content",
                AddNewCompanyProfileUrl = Url.Action("Create", "CustomerProfile"),
                CompanyProfiles = new List<CompanyProfileViewModel>
                {
                    new CompanyProfileViewModel
                    {
                        Name = "Acme Construction Co.",
                        Industry = "Construction",
                        Location = "Houston, TX",
                        Phone = "(555) 123-4567",
                        Website = "https://acme.example.com",
                        ContactsCount = 2,
                        JobSitesCount = 1,
                        ProjectsCount = 1,
                        CreatedDate = new DateTime(2024, 12, 1),
                        UpdatedDate = new DateTime(2025, 1, 10),
                        ViewUrl = "#",
                        EditUrl = "#",
                        DeleteUrl = "#"
                    },
                    new CompanyProfileViewModel
                    {
                        Name = "Texas Oil & Gas LLC",
                        Industry = "Oil & Gas",
                        Location = "Dallas, TX",
                        Phone = "(555) 987-6543",
                        Website = "https://texasoil.example.com",
                        ContactsCount = 1,
                        JobSitesCount = 1,
                        ProjectsCount = 1,
                        CreatedDate = new DateTime(2024, 11, 15),
                        UpdatedDate = new DateTime(2025, 1, 8),
                        ViewUrl = "#",
                        EditUrl = "#",
                        DeleteUrl = "#"
                    }
                }
            };
            return View(model);
        }
    }
}
