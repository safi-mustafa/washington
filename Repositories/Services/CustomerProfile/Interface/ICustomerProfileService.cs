using ViewModels.CustomerProfile;

namespace Repositories.Services.CustomerProfile.Interface
{
    public interface ICustomerProfileService
    {
        Task<List<CompanyProfileViewModel>> GetCompanyProfilesAsync();
        Task<bool> CreateCompanyAsync(CompanyFormData formData);
        Task<bool> DeleteCompanyAsync(long id);
        Task<object> GetCompanyAsync(long id);
        Task<bool> UpdateCompanyAsync(CompanyUpdateData formData);
    }
}
