using Centangle.Common.ResponseHelpers.Models;

using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Interfaces;

using ViewModels;
using ViewModels.CustomerProject;
using ViewModels.ProjectManager;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface ICustomerProjectService<CreateViewModel, UpdateViewModel, DetailViewModel> : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<PaginatedResultModel<T>> GetCustomerDropdown<T>(CustomerProjectSearchViewModel searchVM);
        Task<PaginatedResultModel<T>> GetProjectDropdown<T>(ProjectManagerSearchViewModel searchVM);
        Task<CompanyInformation> GetCompanyInfoById(long id);
        Task<CompanyContact> GetCompanyContactInfoById(long id);
        Task<List<CustomerProject>> GetProjects();
        Task<CustomerProject> GetProjectByid(long id);
        Task<List<CompanyInformation>> GetCompanies();
        Task<List<CustomerProjectNotesViewModel>> GetNotesByCustomerProjectId(int id);
        Task<bool> SaveCustomerProjectNotes(CustomerProjectNotesViewModel model);
        Task<List<TaskType>> GetWorkOrders();
        Task<IRepositoryResponse> CreateOrder(OrderModifyViewModel model, long customerProjectId, long workStepid, string imageUrl);
    }
}
