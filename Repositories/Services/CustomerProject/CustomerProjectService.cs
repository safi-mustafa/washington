using AutoMapper;

using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.Spreadsheet;

using Enums;

using Helpers.Extensions;
using Helpers.File;

using Irony.Parsing;

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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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

        public override async Task<Expression<Func<Models.CustomerProject, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as CustomerProjectSearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.JobCode.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.JobName.ToLower().Contains(searchFilters.Search.value.ToLower())
                            )
                        )
                        &&
                        (searchFilters.Customer.Id == null || x.CustomerId == searchFilters.Customer.Id)
                        &&
                        (searchFilters.ProjectManager.Id == null || x.ProjectManagerId == searchFilters.ProjectManager.Id)
                        ;
        }
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
                var filters = await SetQueryFilter(search);

                var assetsQueryable =
                            from project in _db.CustomerProjects.Where(filters)
                            join companyInfo in _db.CompanyInformations
                                on (long?)project.CustomerId equals (long?)companyInfo.Id into companyGroup
                            from company in companyGroup.DefaultIfEmpty()

                            join contact in _db.CompanyContacts
                                on (long?)project.ProjectManagerId equals (long?)contact.Id into contactGroup
                            from companyContact in contactGroup.DefaultIfEmpty()

                            select new CustomerProjectDetailViewModel
                            {
                                ActiveStatus = project.ActiveStatus,
                                CustomerId = (long)project.CustomerId,
                                JobCode = project.JobCode,
                                JobName = project.JobName,
                                ProjectStartDate = project.ProjectStartDate.Value.Date.ToString("MM/dd/yyyy"),
                                ProjectEndDate = project.ProjectEndDate.Value.Date.ToString("MM/dd/yyyy"),
                                CustomerName = company.CompanyName ?? "-",
                                ProjectManagerName = companyContact.ContactPersonName ?? "-",
                                Id = project.Id,
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

        public async Task<CompanyInformation> GetCompanyInfoById(long id)
        {
            try
            {
                var companyInformation = await _db.CompanyInformations
                              .FirstOrDefaultAsync(x => x.Id == id);

                return companyInformation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetCompanyInfoById() for {typeof(Models.CompanyInformation).FullName} threw an exception");
                return (CompanyInformation)Response.BadRequestResponse(_response);
            }
        }

        public async Task<CompanyContact> GetCompanyContactInfoById(long id)
        {
            try
            {
                var CompanyContactInfo = await _db.CompanyContacts
                              .FirstOrDefaultAsync(x => x.Id == id);

                return CompanyContactInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetCompanyContactInfoById() for {typeof(Models.CompanyContact).FullName} threw an exception");
                return (CompanyContact)Response.BadRequestResponse(_response);
            }
        }
        public async Task<List<Models.CustomerProject>> GetProjects()
        {
            try
            {
                var customerProjectInfo = await _db.CustomerProjects.ToListAsync();

                return customerProjectInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetProjects() for {typeof(Models.CompanyContact).FullName} threw an exception");
                return (List<Models.CustomerProject>)Response.BadRequestResponse(_response);
            }
        }
        public async Task<Models.CustomerProject> GetProjectByid(long id)
        {
            try
            {
                var projectInfo = await _db.CustomerProjects.FirstOrDefaultAsync(x => x.Id == id); ;

                return projectInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetProjectByid() for {typeof(Models.CompanyContact).FullName} threw an exception");
                return (Models.CustomerProject)Response.BadRequestResponse(_response);
            }
        }

        public async Task<List<CompanyInformation>> GetCompanies()
        {
            try
            {
                var companyInfoList = await _db.CompanyInformations.ToListAsync();

                return companyInfoList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetCompanies() for {typeof(Models.CompanyContact).FullName} threw an exception");
                return (List<CompanyInformation>)Response.BadRequestResponse(_response);
            }
        }

        public async Task<List<WorkOrder>> GetWorkOrders()
        {
            try
            {
                var workOrderList = await _db.WorkOrder.ToListAsync();

                return workOrderList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetWorkOrders() for {typeof(Models.CompanyContact).FullName} threw an exception");
                return (List<WorkOrder>)Response.BadRequestResponse(_response);
            }
        }

        public async Task<PaginatedResultModel<T>> GetCustomerDropdown<T>(CustomerProjectSearchViewModel searchVM)
        {
            try
            {
                var companyInformation = await _db.CompanyInformations
                    .Where(p => !p.IsDeleted && p.ActiveStatus == ActiveStatus.Active).AsNoTracking().ToListAsync();

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
                var companyContacts = await _db.CompanyContacts.Where(p => !p.IsDeleted && p.ActiveStatus == ActiveStatus.Active).AsNoTracking().ToListAsync();

                var paginated = await companyContacts
                    .OrderBy(x => x.ContactPersonName)
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

        public async Task<IRepositoryResponse> CreateOrder(OrderModifyViewModel model, long customerProjectId, long workOrderId, string? imageUrl)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Create a WorkOrder first (required for Order)
                var customerProject = await _db.CustomerProjects.FindAsync(customerProjectId);
                if (customerProject == null)
                {
                    return Response.NotFoundResponse(_response);
                }
                
                //var workOrderCount = await _db.WorkOrder.IgnoreQueryFilters().CountAsync();
                //var workOrder = new WorkOrder
                //{
                //    SystemGeneratedId = "WO-" + (workOrderCount + 1).ToString("D4"),
                //    Title = $"Order for {customerProject.JobName}",
                //    Description = model.Notes ?? "Customer order",
                //    Status = WOStatusCatalog.Open,
                //    Urgency = Urgency.Low,
                //    Task = TaskCatalog.Maintenance,
                //    Type = WorkOrderTypeCatalog.Maintenance,
                //    CreatedOn = DateTime.UtcNow
                //};
                
                //await _db.AddAsync(workOrder);
                //await _db.SaveChangesAsync();
                
                // Create the Order
                var order = new Order
                {
                    Notes = model.Notes,
                    Status = OrderStatus.DeliveryScheduled,
                    Type = OrderTypeCatalog.Inventory,
                    WorkOrderId = workOrderId,
                    CreatedOn = DateTime.UtcNow,
                    ImageUrl = imageUrl,
                    CustomerProjectId = customerProjectId,
                    ActiveStatus = ActiveStatus.Active,
                    Cost = model.Cost ?? 0,
                };
                
                var currentCount = await _db.Orders.IgnoreQueryFilters().CountAsync();
                order.OrderNumber = "OD-" + (currentCount + 1).ToString("D4");
                
                await _db.AddAsync(order);
                await _db.SaveChangesAsync();
                
                // Create OrderItems
                var orderItems = new List<OrderItem>();
                foreach (var item in model.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        InventoryId = item.Inventory?.Id > 0 ? item.Inventory.Id : null,
                        EquipmentId = item.Equipment?.Id > 0 ? item.Equipment.Id : null,
                        IsIssued = false,
                        CreatedOn = DateTime.UtcNow,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        ActiveStatus = ActiveStatus.Active
                    };
                    orderItems.Add(orderItem);
                }
                
                await _db.AddRangeAsync(orderItems);
                await _db.SaveChangesAsync();
                
                await transaction.CommitAsync();
                var response = new RepositoryResponseWithModel<long> { ReturnModel = order.Id };
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Exception thrown in CreateOrder method");
                return Response.BadRequestResponse(_response);
            }
        }

    }
}