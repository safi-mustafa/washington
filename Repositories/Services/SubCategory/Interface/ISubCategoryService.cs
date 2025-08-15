using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Services.SubCategory.Interface
{
    public interface ISubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {

        Task<Subcategory> GetSubCategoryInfoById(long id);
      

        Task<PaginatedResultModel<T>> GetSubCategoryById<T>(SubCategorySearchViewModel searchVM);
    }
}

