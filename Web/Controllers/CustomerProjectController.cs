using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Pagination;

using Repositories.Common;

using Select2;
using Select2.Model;

using ViewModels;
using ViewModels.CustomerProject;
using ViewModels.DataTable;
using ViewModels.ProjectManager;
using ViewModels.Timesheet;

using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class CustomerProjectController : CrudBaseController<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel, CustomerProjectDetailViewModel, CustomerProjectBriefViewModel, CustomerProjectSearchViewModel>
    {

        private readonly ICustomerProjectService<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel> _service;
        private readonly ILogger<CustomerProjectController> _logger;
        public CustomerProjectController(ICustomerProjectService<CustomerProjectModifyViewModel, CustomerProjectModifyViewModel, CustomerProjectDetailViewModel> service,
            ILogger<CustomerProjectController> logger,
            IMapper mapper) : base(service, logger, mapper, "CustomerProject", "Customer Project")
        {
            _service = service;
            _logger = logger;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Customer",data = "CustomerName", orderable = true, sortingColumn = "Customer"},
                new DataTableViewModel{title = "Project Manager",data = "ProjectManagerName", orderable = true, sortingColumn = "ProjectManagerName"},
                new DataTableViewModel{title = "Job Name",data = "JobName", orderable = true},
                new DataTableViewModel{title = "Job Code",data = "JobCode", orderable = true},
                new DataTableViewModel{title = "Project Start Date",data = "ProjectStartDate", orderable = true},
                new DataTableViewModel{title = "Project Start Date",data = "ProjectEndDate", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
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
            //var totalInventoryPrice = await _service.GetTotalInventoryPrice(searchModel);
            var result = ConvertToDataTableModel(model, searchModel);
            //result.AdditionalData.Add("TotalPrice", totalInventoryPrice.ToString("C"));
            SetDatatableActions(result);
            var jsonResult = Json(result);
            return jsonResult;
        }
    }
}
