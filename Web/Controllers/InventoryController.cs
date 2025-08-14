using AutoMapper;
using Repositories.Common;
using ViewModels.DataTable;
using ViewModels;
using Web.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;
using ViewModels.Shared.Notes;
using Pagination;

namespace Web.Controllers
{
    public class InventoryController : CrudBaseController<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel, InventoryDetailViewModel, InventoryBriefViewModel, InventorySearchViewModel>
    {
        private readonly IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> _service;
        private readonly ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _transactionService;
        private readonly ILogger<InventoryController> _logger;
        public InventoryController(
            IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> service,
            ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> transactionService,
            ILogger<InventoryController> logger,
            IMapper mapper) : base(service, logger, mapper, "Inventory", "Inventory")
        {
            _service = service;
            _transactionService = transactionService;
            _logger = logger;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "<input type='checkbox' class='select-all-checkbox' onclick='selectAllCheckBoxChanged(this)'>",className="select-all-checkbox action text-right exclude-from-export", data = "Id"},
                new DataTableViewModel{title = "", data = "Id",format="expand",className="action dt-control expand exclude-form-export"},
                 new DataTableViewModel{title = "Image",data = "ImageUrl", format = "html", formatValue="image", className = "image-thumbnail action"},
                new DataTableViewModel{title = "MaterialID",data = "FormattedSystemGeneratedId", orderable = true, sortingColumn = "SystemGeneratedId"},
                //new DataTableViewModel{title = "Item #",data = "ItemNo", orderable = true},
                new DataTableViewModel{title = "Category",data = "Category.Name", orderable = true, filterId="categoryId", hasFilter = true},
                new DataTableViewModel{title = "SubCategory",data = "SubCategory.Name", orderable = true, filterId="sucategoryId", hasFilter = true},
                new DataTableViewModel{title = "Description",data = "FormattedDescription", orderable = true},
                new DataTableViewModel{title = "Manufacturer",data = "Manufacturer.Name", orderable = true, filterId="manufacturerId", hasFilter = true},
                //new DataTableViewModel{title = "MUTCD",data = "MUTCD.Name", orderable = true, sortingColumn = "MUTCD.Code", filterId="mutcdId", hasFilter = true},
                //new DataTableViewModel{title = "Image",data = "MUTCD.ImageUrl", format = "html",formatValue="image", className = "image-thumbnail action"},
                new DataTableViewModel{title = "PartNumber",data = "PartNumber", orderable = true, filterId="partnumberId", hasFilter = true},
                new DataTableViewModel{title = "UnitofMeasure",data = "UOM.Name", orderable = true, filterId="uomId", hasFilter = true},
                new DataTableViewModel{title = "Unit Cost",data = "UnitCost",className = "dt-currency", orderable = true},
                new DataTableViewModel{title = "StockQuantity",data = "Quantity", orderable = true},
                new DataTableViewModel{title = "ReorderLevel",data = "ReOrderLevel", orderable = true},
                //new DataTableViewModel{title = "Total Value",data = "TotalValue", className = "dt-currency" ,orderable = true},
                new DataTableViewModel{title = "Location",data = "Location.Name" ,orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action action-td text-right exclude-form-export"}

            };
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new();
            if (!User.IsInRole("Manager"))
            {
                result.ActionsList.Add(new DataTableActionViewModel() { Action = "AddShipment", Title = "Add", Href = "", Attr = new List<string> { "Id" }, Class = "add-to-cart" });
            }

            result.ActionsList.AddRange(new List<DataTableActionViewModel>{
                    new DataTableActionViewModel() { Action = "GetInventoryIssueHistory", Title = "History", Href = $"/Inventory/GetInventoryIssueHistory/{{Id}}" },
                    new DataTableActionViewModel() { Action = "GetNotes", Title = "Notes",Class="@HasNotesClass", Href = $"/Inventory/GetNotes/{{Id}}" },
                    new DataTableActionViewModel() { Action = "Update", Title = "Update", Href = $"/Inventory/Update/{{Id}}" },
                    //new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Inventory/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
            });
        }

        protected override async Task<CrudListViewModel> OverrideCrudListVM(CrudListViewModel vm)
        {
            var viewModel = await base.OverrideCrudListVM(vm);
            viewModel.SearchBarHtml = @"
                    <ul class=""temp-nav-tabs-action-buttons""> 
                       <li>
                        <button type=""button"" class="""" onclick="" loadCreateModalPanel('/Inventory/Create')"" data-bs-target=""#newSupModal"">
                              <svg width=""17"" height=""16"" viewBox=""0 0 17 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
							<path d=""M8.225 12.3498H8.925V8.3498H12.925V7.6498H8.925V3.6498H8.225V7.6498H4.225V8.3498H8.225V12.3498ZM2.375 15.6998C1.94167 15.6998 1.58333 15.5581 1.3 15.2748C1.01667 14.9915 0.875 14.6331 0.875 14.1998V1.7998C0.875 1.36647 1.01667 1.00814 1.3 0.724805C1.58333 0.441471 1.94167 0.299805 2.375 0.299805H14.775C15.2083 0.299805 15.5667 0.441471 15.85 0.724805C16.1333 1.00814 16.275 1.36647 16.275 1.7998V14.1998C16.275 14.6331 16.1333 14.9915 15.85 15.2748C15.5667 15.5581 15.2083 15.6998 14.775 15.6998H2.375ZM2.375 14.9998H14.775C14.975 14.9998 15.1583 14.9165 15.325 14.7498C15.4917 14.5831 15.575 14.3998 15.575 14.1998V1.7998C15.575 1.5998 15.4917 1.41647 15.325 1.2498C15.1583 1.08314 14.975 0.999805 14.775 0.999805H2.375C2.175 0.999805 1.99167 1.08314 1.825 1.2498C1.65833 1.41647 1.575 1.5998 1.575 1.7998V14.1998C1.575 14.3998 1.65833 14.5831 1.825 14.7498C1.99167 14.9165 2.175 14.9998 2.375 14.9998Z"" fill=""#F54C00""/>
						</svg>
                            Add New
                        </button>
                       </li>
                       <li>
                        <button type=""button"" class="""" onclick=""loadShipmentView('/Inventory/_AddShipments')"" data-bs-target=""#newSupModal"">
                           <svg width=""17"" height=""16"" viewBox=""0 0 17 16"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
							<path d=""M8.225 12.3498H8.925V8.3498H12.925V7.6498H8.925V3.6498H8.225V7.6498H4.225V8.3498H8.225V12.3498ZM2.375 15.6998C1.94167 15.6998 1.58333 15.5581 1.3 15.2748C1.01667 14.9915 0.875 14.6331 0.875 14.1998V1.7998C0.875 1.36647 1.01667 1.00814 1.3 0.724805C1.58333 0.441471 1.94167 0.299805 2.375 0.299805H14.775C15.2083 0.299805 15.5667 0.441471 15.85 0.724805C16.1333 1.00814 16.275 1.36647 16.275 1.7998V14.1998C16.275 14.6331 16.1333 14.9915 15.85 15.2748C15.5667 15.5581 15.2083 15.6998 14.775 15.6998H2.375ZM2.375 14.9998H14.775C14.975 14.9998 15.1583 14.9165 15.325 14.7498C15.4917 14.5831 15.575 14.3998 15.575 14.1998V1.7998C15.575 1.5998 15.4917 1.41647 15.325 1.2498C15.1583 1.08314 14.975 0.999805 14.775 0.999805H2.375C2.175 0.999805 1.99167 1.08314 1.825 1.2498C1.65833 1.41647 1.575 1.5998 1.575 1.7998V14.1998C1.575 14.3998 1.65833 14.5831 1.825 14.7498C1.99167 14.9165 2.175 14.9998 2.375 14.9998Z"" fill=""#F54C00""/>
						</svg>
                            Add Shipment
                        </button>
                       </li>
<li class=favoriteIcon>
                        <button type=""button"" >
                            <svg xmlns=""http://www.w3.org/2000/svg"" width=""16"" height=""16"" viewBox=""0 0 24 24"" fill=""none"" stroke=""#f54e00"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"" class=""lucide lucide-heart  ""><path d=""M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z""></path></svg>
                            Favorites Only
                        </button>
                    </li>
                
</ul>";
            viewModel.HideTitle = true;
            viewModel.HideTopSearchBar = true;
            viewModel.IsLayoutNull = true;
            viewModel.LoadDatatableScript = false;
            return viewModel;
        }

        public async Task<ActionResult> GetNotes(int id)
        {
            try
            {
                var notes = await _service.GetNotesByInventoryId(id);
                List<INotesViewModel> notesViewModels = notes.Cast<INotesViewModel>().ToList();
                ViewBag.InventoryId = id;
                return View("_Notes", notesViewModels);
            }
            catch (Exception ex) { _logger.LogError($"Inventory Notes method threw an exception, Message: {ex.Message}"); return RedirectToAction("Index"); }
        }

        public async Task<ActionResult> SaveNotes(InventoryNotesViewModel model)
        {
            var notes = await _service.SaveNotes(model);
            return Json(notes);
        }

        public async Task<PartialViewResult> GetExpandedData(long id)
        {
            var items = await _transactionService.GetGroupedTransactionsByItems(new List<long> { id });
            ViewBag.InventoryId = id;
            return PartialView("_TransactionItemRows", items);
        }

        public override ActionResult DataTableIndexView(CrudListViewModel vm)
        {
            return View("~/Views/Inventory/_Index.cshtml", vm);
        }

        public ActionResult _AddShipments(string inventoryids)
        {
            var shipmentmodel = new ShipmentGridViewModel();
            var ids = inventoryids.Split(",");
            foreach (var id in ids)
            {
                shipmentmodel.Shipments.Add(new ShipmentModifyViewModel { InventoryId = long.Parse(id) });
            }
            return View("_AddShipments", shipmentmodel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateShipments(ShipmentGridViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.CreateShipments(model);
                return Json(new { status = response });
            }
            var invalidFieldErrors = ModelState.Where(x => x.Value.Errors.Any())
              .ToDictionary(
                  kvp => kvp.Key,
                  kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
              );
            return Json(new { status = false, fieldErrors = invalidFieldErrors });

        }

        protected override async Task<JsonResult> ProcessSearchResult(InventorySearchViewModel searchModel, PaginatedResultModel<InventoryDetailViewModel> model)
        {
            var totalInventoryPrice = await _service.GetTotalInventoryPrice(searchModel);
            var result = ConvertToDataTableModel(model, searchModel);
            result.AdditionalData.Add("TotalPrice", totalInventoryPrice.ToString("C"));
            SetDatatableActions(result);
            var jsonResult = Json(result);
            return jsonResult;
        }

        [HttpGet]
        public async Task<PartialViewResult> GetInventoryIssueHistory(int Id, string lotNo = "", int locationId = 0, int sourceId = 0)
        {

            try
            {
                var transactions = await _service.GetInventoryIssueHistory(Id, lotNo, locationId, sourceId);
                return PartialView("_InventoryIssuedHistory", transactions);
            }
            catch (Exception ex)
            {
                return PartialView("_InventoryIssuedHistory", new List<TransactionHistoryViewModel>());
            }

        }


        public override async Task<ActionResult> Create(InventoryModifyViewModel model)
        {
            if (!await IsItemNoUnique(model.Id, model.ItemNo))
            {
                ModelState.AddModelError("ItemNo", "Item # already in use.");
            }
            return await base.Create(model);
        }

        public async override Task<ActionResult> Update(InventoryModifyViewModel model)
        {
            if (!await IsItemNoUnique(model.Id, model.ItemNo))
            {
                ModelState.AddModelError("ItemNo", "Item # already in use.");
            }
            return await base.Update(model);
        }

        public async Task<bool> IsItemNoUnique(long id, string itemNo)
        {
            return await _service.IsItemNoUnique(id, itemNo);
        }
    }
}

