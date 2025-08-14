using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pagination;
using Repositories.Common;
using Repositories.Services.SubCategory.Interface;
using Select2;
using System.Net;
using ViewModels;
using ViewModels.DataTable;
using ViewModels.ProjectManager;

using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class SubCategoryController : CrudBaseController<
        SubCategoryModifyViewModel,
        SubCategoryModifyViewModel,
        SubCategoryDetailViewModel,
        SubCategoryDetailViewModel,
        SubCategoryBriefViewModel,
        SubCategorySearchViewModel>
    {

        private readonly ISubCategoryService<SubCategoryModifyViewModel, SubCategoryModifyViewModel, SubCategoryDetailViewModel> _service;
        private readonly ILogger<SubCategoryController> _logger;
        private readonly IMapper _mapper;

        public SubCategoryController(
            ISubCategoryService<SubCategoryModifyViewModel, SubCategoryModifyViewModel, SubCategoryDetailViewModel> service,
            ILogger<SubCategoryController> logger,
            IMapper mapper
        ) : base(service, logger, mapper, "SubCategory", "Sub Category")
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel { title = "Name", data = "Name", orderable = true },
                new DataTableViewModel { title = "Category", data = "CategoryName", orderable = true },
                new DataTableViewModel { title = "Action", data = null, className = "action text-right exclude-form-export" }
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

        public override async Task<ActionResult> Update(int id)
        {
            try
            {
                var response = await _service.GetById(id);
                SubCategoryModifyViewModel model = new();
                SubCategoryDetailViewModel detailModel = new();

                if (response.Status == HttpStatusCode.OK)
                {
                    var parsedModel = response as RepositoryResponseWithModel<SubCategoryDetailViewModel>;
                    detailModel = parsedModel.ReturnModel;
                    model = _mapper.Map<SubCategoryModifyViewModel>(parsedModel.ReturnModel);
        }

                if (model != null)
                {
                    // Get Subcategory Info
                    var subCategoryInfo = await _service.GetSubCategoryInfoById(model.Id);

                    // Set Category Info
                    model.Category = new CategoryBriefViewModel()
        {
                        Id = subCategoryInfo.Category.Id,
                        Name = subCategoryInfo.Category.Name,
                        Select2Text = subCategoryInfo.Category.Name
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
                _logger.LogError(ex, "Error updating subcategory");
                return RedirectToAction("Index");
            }
        }

    }
}

