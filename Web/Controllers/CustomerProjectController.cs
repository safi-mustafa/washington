using AutoMapper;

using Centangle.Common.ResponseHelpers.Models;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Pagination;

using Repositories.Common;

using Select2;
using Select2.Model;

using System.Net;

using ViewModels;
using ViewModels.CustomerProject;
using ViewModels.DataTable;
using ViewModels.ProjectManager;
using ViewModels.Shared.Notes;
using ViewModels.Timesheet;

using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class CustomerProjectController : CrudBaseController<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel, CustomerProjectDetailViewModel, CustomerProjectBriefViewModel, CustomerProjectSearchViewModel>
    {

        private readonly ICustomerProjectService<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel> _service;
        private readonly ILogger<CustomerProjectController> _logger;
        private readonly IMapper _mapper;
        public CustomerProjectController(ICustomerProjectService<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel> service,
            ILogger<CustomerProjectController> logger,
            IMapper mapper) : base(service, logger, mapper, "CustomerProject", "Customer Project")
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Customer",data = "CustomerName", orderable = true, sortingColumn = "Customer"},
                new DataTableViewModel{title = "Project Manager",data = "ProjectManagerName", orderable = true, sortingColumn = "ProjectManagerName"},
                new DataTableViewModel{title = "Job Name",data = "JobName", orderable = true},
                new DataTableViewModel{title = "Purchase Order Number",data = "PurchaseOrderNumber", orderable = true},
                new DataTableViewModel{title = "Project Start Date",data = "ProjectStartDate", orderable = true},
                new DataTableViewModel{title = "Project Start Date",data = "ProjectEndDate", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                    new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/CustomerProject/Update/{{Id}}"},
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/CustomerProject/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
                      new DataTableActionViewModel() { Action = "GetNotesByCustomerProjectId", Title = "Notes", Href = $"/CustomerProject/GetNotesByCustomerProjectId/{{Id}}" },
            };

            //result.ActionsList = new();
            //result.ActionsList.AddRange(new List<DataTableActionViewModel>()
            //{
            //        new DataTableActionViewModel() { Action = "GetEquipmentIssueHistory", Title = "History", Href = $"/Equipment/GetEquipmentIssueHistory/{{Id}}" },
            //        new DataTableActionViewModel() {Action="GetNotes",Title="Notes",Class="@HasNotesClass",Href=$"/Equipment/GetNotes/{{Id}}"},
            //        new DataTableActionViewModel() {Action="Update",Title="Update",Href=$"/Equipment/Update/{{Id}}"},
            //        //new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Equipment/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
            //});
        }

        public override async Task<ActionResult> Create(CustomerProjectModifyViewModel model)
        {

            model.CustomerId = (int?)model.CustomerProject.Id;
            model.ProjectManagerId = (int?)model.ProjectManager.Id;

            if (ModelState.IsValid)
            {
                return await base.Create(model);
            }
            var invalidFieldErrors = ModelState.Where(x => x.Value.Errors.Any())
              .ToDictionary(
                  kvp => kvp.Key,
                  kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
              );
            return Json(new { status = false, fieldErrors = invalidFieldErrors });
        }
        public async Task<JsonResult> CustomerSelect2DropDown(string prefix, int pageSize, int pageNumber, string customParams)
        {
            try
            {
                var svm = JsonConvert.DeserializeObject<CustomerProjectSearchViewModel>(customParams);
                svm.PerPage = pageSize;
                svm.CalculateTotal = true;
                svm.CurrentPage = pageNumber;
                svm.Search = new DataTableSearchViewModel() { value = prefix };
                var items = await _service.GetCustomerDropdown<CustomerProjectBriefViewModel>(svm);
                var select2List = GetSelect2List(items);
                return Json(new Select2Repository().GetSelect2PagedResult(pageSize, pageNumber, select2List, items));
            }
            catch (Exception ex)
            {
                _logger.LogError($"CustomerSelect2DropDown method threw an exception, Message: {ex.Message}");
                return null;
            }
        }
        public async Task<JsonResult> ProjectSelect2DropDown(string prefix, int pageSize, int pageNumber, string customParams)
        {
            try
            {
                var svm = JsonConvert.DeserializeObject<ProjectManagerSearchViewModel>(customParams);
                svm.PerPage = pageSize;
                svm.CalculateTotal = true;
                svm.CurrentPage = pageNumber;
                svm.Search = new DataTableSearchViewModel() { value = prefix };
                var items = await _service.GetProjectDropdown<CustomerProjectBriefViewModel>(svm);
                var select2List = GetSelect2List(items);
                return Json(new Select2Repository().GetSelect2PagedResult(pageSize, pageNumber, select2List, items));
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectSelect2DropDown method threw an exception, Message: {ex.Message}");
                return null;
            }
        }

        public async override Task<ActionResult> Update(CustomerProjectModifyViewModel model)
        {
            model.CustomerId = (int?)model.CustomerProject.Id;
            model.ProjectManagerId = (int?)model.ProjectManager.Id;

            if (ModelState.IsValid)
            {
                return await base.Update(model);
            }
            var invalidFieldErrors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
            return Json(new { status = false, fieldErrors = invalidFieldErrors });
        }

        protected override async Task<JsonResult> ProcessSearchResult(CustomerProjectSearchViewModel searchModel, PaginatedResultModel<CustomerProjectDetailViewModel> model)
        {
            var result = ConvertToDataTableModel(model, searchModel);
            SetDatatableActions(result);
            var jsonResult = Json(result);
            return jsonResult;
        }

        public override async Task<ActionResult> Update(int id)
        {
            try
            {
                var response = await _service.GetById(id);
                CustomerProjectModifyViewModel model = new();
                CustomerProjectDetailViewModel detailModel = new();
                if (response.Status == HttpStatusCode.OK)
                {
                    var parsedModel = response as RepositoryResponseWithModel<CustomerProjectDetailViewModel>;
                    detailModel = parsedModel.ReturnModel;
                    model = _mapper.Map<CustomerProjectModifyViewModel>(parsedModel.ReturnModel);
                }
                if (model != null)
                {
                    var companyInfo = await _service.GetCompanyInfoById((long)model.CustomerId);
                    var companyContactInfo = await _service.GetCompanyContactInfoById((long)model.ProjectManagerId);

                    model.CustomerProject = new CustomerProjectBriefViewModel()
                    {
                        Id = companyInfo.Id,
                        Name = companyInfo.CompanyName,
                        Select2Text = companyInfo.CompanyName,
                    };

                    model.ProjectManager = new ProjectManagerBriefViewModel()
                    {
                        Id = companyContactInfo?.Id,
                        ContactPersonName = companyContactInfo?.ContactPersonName,
                        Select2Text = companyContactInfo?.ContactPersonName,
                    };

                    var updateVM = GetUpdateViewModel("Update", model);
                    updateVM = await OverrideUpdateViewModel(updateVM);
                    return UpdateView(updateVM);
                }
                else
                {

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> SaveCustomerProjectNotes(CustomerProjectNotesViewModel model)
        {
            var notes = await _service.SaveCustomerProjectNotes(model);
            return Json(notes);
        }


        [HttpGet]
        public async Task<ActionResult> GetNotesByCustomerProjectId(int Id)
        {
            try
            {
                var notes = await _service.GetNotesByCustomerProjectId(Id);
                List<INotesViewModel> notesViewModels = notes.Cast<INotesViewModel>().ToList();
                ViewBag.CPId = Id;
                return View("_CustomerProjectNotes", notesViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Customer Project Notes Notes method threw an exception, Message: {ex.Message}"); return RedirectToAction("Index");
            }
        }
    }
}
