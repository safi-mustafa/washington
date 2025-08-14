using AutoMapper;

using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using Enums;

using Helpers.Extensions;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using System.Linq.Expressions;

using ViewModels;
using ViewModels.ProjectManager;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class SubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<Subcategory, CreateViewModel, UpdateViewModel, DetailViewModel>, ISubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public SubCategoryService(ApplicationDbContext db, ILogger<SubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _mapper = mapper;
        }

        public override async Task<Expression<Func<Subcategory, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as SubCategorySearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (string.IsNullOrEmpty(searchFilters.Name) || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
                        ;
        }

        public async Task<PaginatedResultModel<T>> GetSubCategoryById<T>(SubCategorySearchViewModel searchVM)
        {
            try
            {
                var subcategoriesList = new List<Subcategory>();
                int? categoryId = null;
                if (int.TryParse(searchVM.SearchView, out var parsedValue))
                {
                    categoryId = parsedValue;
                }

                if (categoryId != null)
                {
                    subcategoriesList = await _db.Subcategories
                        .Where(p => !p.IsDeleted
                            && p.ActiveStatus == ActiveStatus.Active
                            && p.CategoryId == categoryId)
                        .AsNoTracking()
                        .ToListAsync();
                }
                else
                {
                    subcategoriesList = await _db.Subcategories
                        .Where(p => !p.IsDeleted
                            && p.ActiveStatus == ActiveStatus.Active)
                        .AsNoTracking()
                        .ToListAsync();
                }
                var paginated = await subcategoriesList
                    .OrderBy(x => x.Name)
                    .ToList()
                    .PaginateList(searchVM);

                var mappedItems = _mapper.Map<List<T>>(paginated.Items);

                return new PaginatedResultModel<T>
                {
                    Items = mappedItems,
                    _links = paginated._links,
                    _meta = paginated._meta
                };
            }
            catch
            {
                return new PaginatedResultModel<T>();
            }
        }

    }
}

