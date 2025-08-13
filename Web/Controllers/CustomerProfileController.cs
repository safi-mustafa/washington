using Microsoft.AspNetCore.Mvc;
using ViewModels.CustomerProfile;
using Repositories.Services.CustomerProfile.Interface;

namespace Web.Controllers
{
    public class CustomerProfileController : Controller
    {
        private readonly ICustomerProfileService _customerProfileService;

        public CustomerProfileController(ICustomerProfileService customerProfileService)
        {
            _customerProfileService = customerProfileService;
        }

        public async Task<IActionResult> Create()
        {
            var companyProfiles = await _customerProfileService.GetCompanyProfilesAsync();

            var model = new CustomerProfileViewModel
            {
                Title = "Company Profiles",
                Id = "customer-profile-list",
                ContentId = "customer-profile-list-content",
                AddNewCompanyProfileUrl = Url.Action("Create", "CustomerProfile"),
                CompanyProfiles = companyProfiles
            };

            return View("_CompanyProfileList", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyFormData formData)
        {
            var success = await _customerProfileService.CreateCompanyAsync(formData);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _customerProfileService.DeleteCompanyAsync(id);
            return Json(new { success, error = success ? null : "Company not found" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCompany(long id)
        {
            var result = await _customerProfileService.GetCompanyAsync(id);
            return result != null ? Json(result) : Json(new { error = "Company not found" });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] CompanyUpdateData formData)
        {
            var success = await _customerProfileService.UpdateCompanyAsync(formData);
            return Json(new { success, error = success ? null : "Company not found" });
        }
    }
}
