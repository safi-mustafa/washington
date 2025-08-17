using Centangle.Common.ResponseHelpers.Models;
using Pagination;
using System.Net.Sockets;
using ViewModels.CRUD;

namespace Repositories.Services.Reports.Interface
{
    public interface IReportsService
    {
        Task<ReportsCountViewModel> Orders();
        Task<List<ActiveRentalsModel>> GetActiveRentals();
        Task<List<CustomerProjectsViewModel>> GetCustomerProjects();
        Task<GetCostbyOnRentModel> GetCostbyOnRent();
        Task<GetCostbyDeliveredModel> GetCostbyDelivered();
        Task<GetCostbyScheduledModel> GetCostbyScheduled();
        Task<GetCostbyPendingModel> GetCostbyPending();
    }
}
