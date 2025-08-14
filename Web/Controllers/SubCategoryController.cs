using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Pagination;

using Repositories.Common;

using Select2;

using ViewModels;
using ViewModels.DataTable;
using ViewModels.ProjectManager;

using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class SubCategoryController : CrudBaseController<SubCategoryModifyViewModel, SubCategoryModifyViewModel, SubCategoryDetailViewModel, SubCategoryDetailViewModel, SubCategoryBriefViewModel, SubCategorySearchViewModel>
    {
        private readonly ISubCategoryService<SubCategoryModifyViewModel, SubCategoryModifyViewModel, SubCategoryDetailViewModel> _service;

        private readonly ILogger<SubCategoryController> _logger;
        public SubCategoryController(ISubCategoryService<SubCategoryModifyViewModel, SubCategoryModifyViewModel, SubCategoryDetailViewModel> service, ILogger<SubCategoryController> logger, IMapper mapper) : base(service, logger, mapper, "Sub Category", "SubCategory", false)
        {
            _service = service;
            _logger = logger;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        public async Task<JsonResult> GetSubCategoryByIdSelect2DropDown(string prefix, int pageSize, int pageNumber, string customParams)
        {
            try
            {
                var svm = JsonConvert.DeserializeObject<SubCategorySearchViewModel>(customParams);
                svm.PerPage = pageSize;
                svm.CalculateTotal = true;
                svm.CurrentPage = pageNumber;
                svm.Search = new DataTableSearchViewModel() { value = prefix };
                var items = await _service.GetSubCategoryById<SubCategoryBriefViewModel>(svm);
                var select2List = GetSelect2List(items);
                return Json(new Select2Repository().GetSelect2PagedResult(pageSize, pageNumber, select2List, items));
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProjectSelect2DropDown method threw an exception, Message: {ex.Message}");
                return null;
            }
        }
    }
}

