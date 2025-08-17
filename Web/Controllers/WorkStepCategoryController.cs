using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using Select2.Model;
using ViewModels;
using ViewModels.DataTable;
using ViewModels.WorkStepCategory;

using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class WorkStepCategoryController : CrudBaseController<WorkStepCategoryModifyViewModel, WorkStepCategoryModifyViewModel, WorkStepCategoryDetailViewModel, WorkStepCategoryDetailViewModel, WorkStepCategoryBriefViewModel, WorkStepCategorySearchViewModel>
    {
        private readonly IWorkStepCategoryService<WorkStepCategoryModifyViewModel, WorkStepCategoryModifyViewModel, WorkStepCategoryDetailViewModel> _service;

        public WorkStepCategoryController(IWorkStepCategoryService<WorkStepCategoryModifyViewModel, WorkStepCategoryModifyViewModel, WorkStepCategoryDetailViewModel> service, ILogger<WorkStepCategoryController> logger, IMapper mapper) : base(service, logger, mapper, "WorkStepCategory", "WorkStepCategory")
        {
            _service = service;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name", orderable = true},
                new DataTableViewModel{title = "STRate",data = "STRate",className="dt-currency", orderable = true},
                new DataTableViewModel{title = "OTRate",data = "OTRate",className="dt-currency", orderable = true},
                 new DataTableViewModel{title = "DTRate",data = "DTRate",className="dt-currency", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        //public async override Task<IRepositoryResponse> GetResponse(WorkStepCategorySearchViewModel svm)
        //{
        //    return await _service.GetWorkStepCategorysForSelect2<WorkStepCategoryBriefViewModel>(svm);
        //}
    }
}

