using Centangle.Common.ResponseHelpers.Models;
using Pagination;
using ViewModels.CRUD;

namespace Repositories.Services.Reports.Interface
{
    public interface IReportsService
    {
        Task<ReportsCountViewModel> Orders();
        Task<List<ActiveRentalsModel>> GetActiveRentals();
        Task<List<CustomerProjectsViewModel>> GetCustomerProjects();
    }
}
