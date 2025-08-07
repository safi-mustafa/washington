using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Models.Common.Interfaces;
using Models;
using Pagination;
using Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;
using ViewModels;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;


namespace Repositories.Common
{
    public class AssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<AssetType, CreateViewModel, UpdateViewModel, DetailViewModel>, IAssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;

        public AssetTypeService(ApplicationDbContext db, ILogger<AssetTypeService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
        }

        public override async Task<Expression<Func<AssetType, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as AssetTypeSearchViewModel;

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


        public async Task<List<AssetTypeTreeViewModel>> GetHirearchy()
        {
            var assetTree = await _db.AssetTypes.Include(at => at.AssetTypeLevel1)
                    .ThenInclude(atl1 => atl1.AssetTypeLevel2)
                .Select(at => new AssetTypeTreeViewModel
                {
                    Id= at.Id,
                    Name = at.Name,
                    AssetTypeLevel1 = at.AssetTypeLevel1.Select(atl1 => new AssetTypeLevel1TreeViewModel
                    {
                        Id = atl1.Id,
                        Name = atl1.Name,
                        AssetTypeLevel2 = atl1.AssetTypeLevel2.Select(atl2 => new AssetTypeLevel2TreeViewModel
                        {
                            Id = atl2.Id,
                            Name = atl2.Name
                        }).ToList()
                    }).ToList()
                }).ToListAsync();
            return assetTree;
        }

    }
}
