using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class CurrentStatusController : CrudBaseController<CurrentStatusModifyViewModel, CurrentStatusModifyViewModel, CurrentStatusDetailViewModel, CurrentStatusDetailViewModel, CurrentStatusBriefViewModel, CurrentStatusSearchViewModel>
    {
        public CurrentStatusController(ICurrentStatusService<CurrentStatusModifyViewModel, CurrentStatusModifyViewModel, CurrentStatusDetailViewModel> service, ILogger<CurrentStatusController> logger, IMapper mapper) : base(service, logger, mapper, "Current Status", "CurrentStatus")
        {

        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                 new DataTableViewModel{title = "Color",data = "Color"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
    }
}

