using AutoMapper;

using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.Spreadsheet;

using Enums;

using Helpers.Extensions;
using Helpers.File;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.CustomerProject;
using ViewModels.ProjectManager;
using ViewModels.Shared;
using ViewModels.Timesheet;

namespace Repositories.Services.CustomerProject
{

    public class CustomerProjectService<CreateViewModel, UpdateViewModel, DetailViewModel> :
      BaseService<Models.CustomerProject, CreateViewModel, UpdateViewModel, DetailViewModel>,
      ICustomerProjectService<CreateViewModel, UpdateViewModel, DetailViewModel>
      where DetailViewModel : class, IBaseCrudViewModel, new()
      where CreateViewModel : class, IBaseCrudViewModel, new()
      where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CustomerProjectService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IFileHelper _fileHelper;

        public CustomerProjectService(ApplicationDbContext db, ILogger<CustomerProjectService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger, IMapper mapper, IRepositoryResponse response, IActionContextAccessor actionContext, IFileHelper fileHelper) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
            _fileHelper = fileHelper;
        }

        //public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        //{
        //    var viewModel = model as CustomerProjectModifyViewModel;
        //    var totalEquipmentCount = await _db.Equipments.IgnoreQueryFilters().CountAsync();
        //    viewModel.SystemGeneratedId = "EQP-" + (totalEquipmentCount + 1).ToString("D4");
        //    return await base.Create(model);


        //}

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {

            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (await CanCreate() == false)
                {
                    return UnAuthorizedResponse();
                }
                var mappedModel = _mapper.Map<Models.CustomerProject>(model);
                await _db.AddAsync(mappedModel);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                var response = new RepositoryResponseWithModel<long> { ReturnModel = mappedModel.Id };
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Exception thrown in Create method of Asset");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                //var companyInformation = await _db.CompanyInformations.AsNoTracking()
                //    .ToListAsync();

                //var searchFilters = search as CustomerProjectSearchViewModel;
                //var assetsQueryable = _db.CustomerProjects.Select(a => new CustomerProjectDetailViewModel
                //{
                //    ActiveStatus = a.ActiveStatus,
                //    CustomerId = a.CustomerId,
                //    JobCode = a.JobCode,
                //    JobName = a.JobName,
                //    CustomerName = string.Empty,
                //    Id = a.Id,
                //    Customer = new CustomerProjectBriefViewModel()
                //    {
                //        Id = companyInformation.Select(x => x.CompanyInformationId).FirstOrDefault(),
                //        Name = companyInformation.Select(x => x.CompanyName).FirstOrDefault(),
                //    },
                //}).AsQueryable();

                var assetsQueryable =
                                from project in _db.CustomerProjects
                                join companyInfo in _db.CompanyInformations
                                    on project.CustomerId equals companyInfo.Id into companyGroup
                                from company in companyGroup.DefaultIfEmpty()
                                join contact in _db.CompanyContacts
                                    on company.Id equals contact.CompanyInformationId into contactGroup
                                from companyContact in contactGroup.DefaultIfEmpty()
                                select new CustomerProjectDetailViewModel
                                {
                                    ActiveStatus = project.ActiveStatus,
                                    CustomerId = project.CustomerId,
                                    JobCode = project.JobCode,
                                    JobName = project.JobName,
                                    ProjectStartDate = project.ProjectStartDate,
                                    ProjectEndDate = project.ProjectEndDate,
                                    CustomerName = company != null ? company.CompanyName : "-",
                                    ProjectManager = companyContact != null ? companyContact.Role : "-",
                                    Id = project.Id,
                                    Customer = new CustomerProjectBriefViewModel()
                                    {
                                        Id = company != null ? company.Id : 0,
                                        Name = company != null ? company.CompanyName : "-",
                                    },
                                    ProjectManagerBVM = new ProjectManagerBriefViewModel()
                                    {
                                        Id = companyContact != null ? companyContact.Id : 0,
                                        Role = companyContact != null ? companyContact.Role : "-",
                                    },
                                };


                var result = await assetsQueryable.Paginate(search);
                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<CustomerProjectDetailViewModel>();
                    paginatedResult.Items = result.Items.ToList();
                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<CustomerProjectDetailViewModel>> { ReturnModel = paginatedResult };
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

        public async override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (await CanUpdate(model.Id) == false)
                {
                    return UnAuthorizedResponse();
                }
                var updateModel = model as BaseUpdateVM;
                if (updateModel != null)
                {
                    var record = await _db.CustomerProjects.FindAsync(updateModel?.Id);
                    if (record != null)
                    {
                        var dbModel = _mapper.Map(model, record);
                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        var response = new RepositoryResponseWithModel<long> { ReturnModel = record.Id };
                        return response;
                    }
                    _logger.LogWarning($"Record for id: {updateModel?.Id} not found in Asset in Update()");
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Update() for Asset threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                var dbModel = await _db.CustomerProjects
                                        .Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();

                if (dbModel != null)
                {
                    var result = _mapper.Map<DetailViewModel>(dbModel);
                    var response = new RepositoryResponseWithModel<DetailViewModel> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(Asset).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Asset).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<PaginatedResultModel<T>> GetCustomerDropdown<T>(CustomerProjectSearchViewModel searchVM)
        {
            try
            {
                var companyInformation = await _db.CompanyInformations.AsNoTracking().ToListAsync();

                // Paginate the CompanyInformation list
                var paginated = await companyInformation
                    .OrderBy(x => x.CompanyName)
                    .ToList()
                    .PaginateList(searchVM);

                // Map the PaginatedResultModel<CompanyInformation> to PaginatedResultModel<T>
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

        public async Task<PaginatedResultModel<T>> GetProjectDropdown<T>(ProjectManagerSearchViewModel searchVM)
        {
            try
            {
                var companyContacts = new List<CompanyContact>();
                if (string.IsNullOrWhiteSpace(searchVM.SearchView))
                {
                    companyContacts = await _db.CompanyContacts.AsNoTracking().ToListAsync();
                }
                else
                {
                    companyContacts = await _db.CompanyContacts.Where(p => p.CompanyInformationId == Convert.ToInt64(searchVM.SearchView)).AsNoTracking().ToListAsync();
                }

                var paginated = await companyContacts
                    .OrderBy(x => x.Role)
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