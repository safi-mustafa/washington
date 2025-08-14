using AutoMapper;
using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Helpers.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Common;
using Repositories.Services.SubCategory.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using ViewModels.CustomerProject;
using ViewModels.Shared;

namespace Repositories.Services.SubCategory
{
    public class SubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel>
       : BaseService<Subcategory, CreateViewModel, UpdateViewModel, DetailViewModel>,
         ISubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel>
       where DetailViewModel : class, IBaseCrudViewModel, new()
       where CreateViewModel : class, IBaseCrudViewModel, new()
       where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<SubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IRepositoryResponse _response;
        public SubCategoryService(
           ApplicationDbContext db,
           ILogger<SubCategoryService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
           IMapper mapper,
           IRepositoryResponse response,
           IActionContextAccessor actionContext
       ) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _response = response;
        }
        public override async Task<Expression<Func<Subcategory, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as SubCategorySearchViewModel;

            return x =>
                (
                    string.IsNullOrEmpty(searchFilters.Search.value)
                    || x.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                )
                &&
                (string.IsNullOrEmpty(searchFilters.Name)
                    || x.Name.ToLower().Contains(searchFilters.Name.ToLower()))
                &&
                (searchFilters.CategoryId == null
                    || x.CategoryId == searchFilters.CategoryId);
        }
        public async Task<Subcategory> GetSubCategoryInfoById(long id)
        {
            try
            {
                var subCategoryInfo = await _db.Subcategories
                    .Include(s => s.Category) // Load category too
                    .FirstOrDefaultAsync(x => x.Id == id);

                return subCategoryInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetSubCategoryInfoById() for {typeof(Models.Subcategory).FullName} threw an exception");
                return (Subcategory)Response.BadRequestResponse(_response);
            }
        }



        public async override Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                var filters = await SetQueryFilter(search);

                var assetsQueryable =
                            from sub in _db.Subcategories.Where(filters)
                            join cat in _db.Categories
                                on sub.CategoryId equals cat.Id

                            select new SubCategoryDetailViewModel
                            {
                                ActiveStatus = sub.ActiveStatus,
                                CategoryId = cat.Id,
                                CategoryName = cat.Name,
                                Name = sub.Name,
                                Id = sub.Id,
                            };


                var result = await assetsQueryable.Paginate(search);
                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<SubCategoryDetailViewModel>();
                    paginatedResult.Items = result.Items.ToList();
                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<SubCategoryDetailViewModel>> { ReturnModel = paginatedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for {typeof(Asset).FullName} in GetAll()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(Asset).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }

    }
}
