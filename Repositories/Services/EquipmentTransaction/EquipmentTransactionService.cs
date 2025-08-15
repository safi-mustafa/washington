using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Models.Common.Interfaces;
using Models;
using Pagination;
using System.Linq.Expressions;
using ViewModels.Shared;
using ViewModels;
using Microsoft.EntityFrameworkCore;
using Enums;

namespace Repositories.Common
{
    public class EquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel> :
        BaseService<EquipmentTransaction, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IEquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        public EquipmentTransactionService(
            ApplicationDbContext db,
            ILogger<EquipmentTransactionService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
            IMapper mapper,
            IRepositoryResponse response,
            IActionContextAccessor actionContext
            )
            : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
        }

        public override async Task<Expression<Func<EquipmentTransaction, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as EquipmentTransactionSearchViewModel;

            return x =>
                        (
                            (
                                string.IsNullOrEmpty(searchFilters.Search.value)
                                ||
                                x.Supplier.Name.ToLower().Contains(searchFilters.Search.value.ToLower())
                                ||
                                x.Location.Name.ToLower().Contains(searchFilters.Search.value.ToLower())

                            )
                        )
                        &&
                        (searchFilters.Supplier.Id == null || x.Supplier.Id == searchFilters.Supplier.Id)
                          &&
                        (searchFilters.Location.Id == null || x.Location.Id == searchFilters.Location.Id)

                        ;
        }

        internal override List<string> GetIncludeColumns()
        {
            return new List<string> { "Equipment", "Location", "Source", "Supplier" };
        }

        public async Task<List<EquipmentTransactionDetailViewModel>> GetGroupedTransactionsByItems(List<long> EquipmentId, long? currentstatusId = 0, long? conditionId = 0)
        {
            try
            {
                var itemsQueryable =
                                from t in _db.EquipmentTransactions.Where(x => EquipmentId.Contains(x.EquipmentId))
                                 join i in _db.Equipments on t.EquipmentId equals i.Id
                                 join l in _db.Locations on t.LocationId equals l.Id
                                 join sp in _db.Suppliers on t.SupplierId equals sp.Id

                                 // Left join for Condition table
                                 join cnd in _db.Conditions on t.ConditionId equals cnd.Id into cndGroup
                                 from cnd in cndGroup.DefaultIfEmpty()

                                     // Left join for CurrentStatus table
                                 join cs in _db.CurrentStatus on t.CurrentStatusId equals cs.Id into csGroup
                                 from cs in csGroup.DefaultIfEmpty()

                                 select new
                                 {
                                     t.PoNo,
                                     t.EquipmentId,
                                     EquipmentName = i.Description ?? "-",
                                     t.SupplierId,
                                     SupplierName = sp.Name ?? "-",
                                     t.LocationId,
                                     LocationName = l != null ? l.Name : "-",
                                     ConditionName = cnd != null ? cnd.Name : "-",          // Condition name from left join
                                     CurrentStatusName = cs != null ? cs.Name : "-",       // Current status name from left join
                                     t.ItemPrice,
                                     t.Quantity,
                                     i.HourlyRate,
                                     t.CreatedOn,
                                     t.PurchaseDate,
                                     t.AssetTag,
                                     t.ConditionId,
                                     t.CurrentStatusId
                                 };

                // ✅ Apply currentstatusId filter only if > 0
                if (currentstatusId.HasValue && currentstatusId > 0)
                {
                    itemsQueryable = itemsQueryable.Where(x => x.CurrentStatusId == currentstatusId);
                }
                // ✅ Apply conditionId filter only if > 0
                if (conditionId.HasValue && conditionId > 0)
                {
                    itemsQueryable = itemsQueryable.Where(x => x.ConditionId == conditionId);
                }


                var items = await itemsQueryable.GroupBy(x => new
                        {
                            x.PoNo,
                            x.EquipmentId,
                            x.SupplierId,
                            x.LocationId,
                            x.ItemPrice,
                            x.PurchaseDate,
                            x.ConditionName,
                            x.CurrentStatusName,
                            x.ConditionId,
                            x.CurrentStatusId,
                        })
                        .Select(x => new EquipmentTransactionDetailViewModel()
                        {
                            Equipment = new EquipmentDetailViewModel()
                            {
                                Id = x.Key.EquipmentId,
                                HourlyRate = x.Max(m => m.HourlyRate),
                                Description = x.Max(y => y.EquipmentName),
                            },
                            Supplier = new SupplierBriefViewModel()
                            {
                                Id = x.Key.SupplierId,
                                Name = x.Max(y => y.SupplierName)
                            },
                            Location = new LocationBriefViewModel()
                            {
                                Id = x.Key.LocationId,
                                Name = x.Max(y => y.LocationName)
                            },
                            CurrentStatus = new CurrentStatusBriefViewModel()
                            {
                                Id = x.Key.CurrentStatusId,
                                Name = x.Max(y => y.CurrentStatusName)
                            },
                            Condition = new ConditionBriefViewModel()
                            {
                                Id = x.Key.ConditionId,
                                Name = x.Max(y => y.ConditionName)
                            },
                            ItemPrice = x.Key.ItemPrice,
                            PONo = x.Key.PoNo,
                            HourlyRate = x.Max(y => y.HourlyRate),
                            Quantity = x.Sum(y => y.Quantity),
                            CreatedOn = x.Max(y => y.CreatedOn),
                            PurchaseDate = x.Max(y => y.CreatedOn),
                            AssetTag = x.Max(y => y.AssetTag)
                        })
                        .OrderByDescending(x => x.CreatedOn).ToListAsync();

                //var items = await itemsQueryable.ToListAsync();
                return items;



                //var itemsQueryable = (from t in _db.EquipmentTransactions.Where(x => EquipmentId.Contains(x.EquipmentId))
                //                      join i in _db.Equipments on t.EquipmentId equals i.Id
                //                      join l in _db.Locations on t.LocationId equals l.Id
                //                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                //                      select new
                //                      {
                //                          t.PoNo,
                //                          t.EquipmentId,
                //                          EquipmentName = i.Description ?? "-",
                //                          t.SupplierId,
                //                          SupplierName = sp.Name ?? "-",
                //                          t.LocationId,
                //                          LocationName = l.Name ?? "-",
                //                          t.ItemPrice,
                //                          t.Quantity,
                //                          t.Equipment.HourlyRate,
                //                          t.CreatedOn,
                //                          t.PurchaseDate,
                //                          t.AssetTag,

                //                      }).GroupBy(x => new { x.PoNo, x.EquipmentId, x.SupplierId, x.LocationId, x.ItemPrice, x.PurchaseDate })
                //                      .Select(x => new EquipmentTransactionDetailViewModel()
                //                      {
                //                          Equipment = new EquipmentDetailViewModel()
                //                          {
                //                              Id = x.Key.EquipmentId,
                //                              HourlyRate = x.Max(x => x.HourlyRate),
                //                              Description = x.Max(y => y.EquipmentName)
                //                          },
                //                          Supplier = new SupplierBriefViewModel()
                //                          {
                //                              Id = x.Key.SupplierId,
                //                              Name = x.Max(y => y.SupplierName)
                //                          },
                //                          Location = new LocationBriefViewModel()
                //                          {
                //                              Id = x.Key.LocationId,
                //                              Name = x.Max(y => y.LocationName)
                //                          },
                //                          ItemPrice = x.Key.ItemPrice,//x.Average(x => x.ItemPrice)
                //                          PONo = x.Key.PoNo,
                //                          HourlyRate = x.Max(x => x.HourlyRate),//x.Average(x => x.ItemPrice)
                //                          Quantity = x.Sum(x => x.Quantity),
                //                          CreatedOn = x.Max(x => x.CreatedOn),
                //                          PurchaseDate = x.Max(x => x.CreatedOn)
                //                      })
                //                      //.Where(x => x.Quantity > 0)
                //                      .OrderByDescending(x => x.CreatedOn).AsQueryable();
                //var items = await itemsQueryable.ToListAsync();
                //return items;
            }
            catch (Exception ex)
            {
                return new List<EquipmentTransactionDetailViewModel>();
            }
        }

        public async Task<List<EquipmentTransactionDetailViewModel>> GetGroupedTransactionsByItemsForOrder(List<long> EquipmentId)
        {
            try
            {
                var itemsQueryable =
    (from t in _db.EquipmentTransactions.Where(x => EquipmentId.Contains(x.EquipmentId))
     join i in _db.Equipments on t.EquipmentId equals i.Id
     join sp in _db.Suppliers on t.SupplierId equals sp.Id

     // Left join for Location
     join l in _db.Locations on t.LocationId equals l.Id into locGroup
     from l in locGroup.DefaultIfEmpty()

         // Left join for Condition
     join cnd in _db.Conditions on t.ConditionId equals cnd.Id into cndGroup
     from cnd in cndGroup.DefaultIfEmpty()

         // Left join for CurrentStatus
     join cs in _db.CurrentStatus on t.CurrentStatusId equals cs.Id into csGroup
     from cs in csGroup.DefaultIfEmpty()

     select new
     {
         t.PoNo,
         t.EquipmentId,
         EquipmentName = i.Description,
         t.SupplierId,
         SupplierName = sp.Name,
         t.LocationId,
         LocationName = l.Name,
         ConditionId = t.ConditionId,
         ConditionName = cnd.Name,
         CurrentStatusId = t.CurrentStatusId,
         CurrentStatusName = cs.Name,
         t.ItemPrice,
         t.Quantity,
         i.HourlyRate,
         t.CreatedOn,
         t.PurchaseDate,
         t.AssetTag
     })
    .GroupBy(x => new
    {
        x.PoNo,
        x.EquipmentId,
        x.SupplierId,
        x.LocationId,
        x.ItemPrice,
        x.PurchaseDate,
        x.ConditionId,
        x.CurrentStatusId
    })
    .Select(x => new EquipmentTransactionDetailViewModel()
    {
        Equipment = new EquipmentDetailViewModel()
        {
            Id = x.Key.EquipmentId,
            HourlyRate = x.Max(m => m.HourlyRate),
            Description = x.Max(y => y.EquipmentName ?? "-")
        },
        Supplier = new SupplierBriefViewModel()
        {
            Id = x.Key.SupplierId,
            Name = x.Max(y => y.SupplierName ?? "-")
        },
        Location = new LocationBriefViewModel()
        {
            Id = x.Key.LocationId,
            Name = x.Max(y => y.LocationName ?? "-")
        },
        CurrentStatus = new CurrentStatusBriefViewModel()
        {
            Id = x.Key.CurrentStatusId,
            Name = x.Max(y => y.CurrentStatusName ?? "-")
        },
        Condition = new ConditionBriefViewModel()
        {
            Id = x.Key.ConditionId,
            Name = x.Max(y => y.ConditionName ?? "-")
        },
        ItemPrice = x.Key.ItemPrice,
        PONo = x.Key.PoNo,
        HourlyRate = x.Max(y => y.HourlyRate),
        Quantity = x.Sum(y => y.Quantity),
        CreatedOn = x.Max(y => y.CreatedOn),
        PurchaseDate = x.Max(y => y.PurchaseDate),
        AssetTag = x.Max(y => y.AssetTag ?? "-")
    })
    .OrderByDescending(x => x.CreatedOn)
    .AsQueryable();

                var items = await itemsQueryable.ToListAsync();
                return items;

            }
            catch (Exception ex)
            {
                return new List<EquipmentTransactionDetailViewModel>();
            }

        }

        public async Task<List<EquipmentTransactionDetailViewModel>> GetWorkOrderTransactions(string workOrderId)
        {
            try
            {
                var itemsQueryable = (from t in _db.EquipmentTransactions.Where(x => x.TransactionType == EquipmentTransactionTypeCatalog.Order || x.TransactionType == EquipmentTransactionTypeCatalog.Return)
                                      join e in _db.Equipments on t.EquipmentId equals e.Id
                                      join l in _db.Locations on t.LocationId equals l.Id
                                      join sp in _db.Suppliers on t.SupplierId equals sp.Id
                                      join o in _db.Orders on t.EntityId equals o.Id
                                      join wo in _db.WorkOrder on o.WorkOrderId equals wo.Id
                                      where wo.SystemGeneratedId.Equals(workOrderId)
                                      select new EquipmentTransactionDetailViewModel()
                                      {
                                          Equipment = new EquipmentDetailViewModel()
                                          {
                                              Id = e.Id,
                                              HourlyRate = e.HourlyRate,
                                              Description = e.Description
                                          },
                                          Supplier = new SupplierBriefViewModel()
                                          {
                                              Id = sp.Id,
                                              Name = sp.Name
                                          },
                                          Location = new LocationBriefViewModel()
                                          {
                                              Id = l.Id,
                                              Name = l.Name
                                          },
                                          TransactionType = t.TransactionType,
                                          ItemPrice = t.ItemPrice,//x.Average(x => x.ItemPrice)
                                          PONo = t.PoNo,
                                          Hours = t.Hours,
                                          HourlyRate = e.HourlyRate,//x.Average(x => x.ItemPrice)
                                          Quantity = t.Quantity,
                                          CreatedOn = t.CreatedOn,
                                          PurchaseDate = t.CreatedOn
                                      }).OrderByDescending(x => x.CreatedOn).AsQueryable();
                var items = await itemsQueryable.ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                return new List<EquipmentTransactionDetailViewModel>();
            }
        }

    }
}
