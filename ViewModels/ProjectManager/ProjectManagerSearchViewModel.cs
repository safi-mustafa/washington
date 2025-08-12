using Enums;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.CRUD.Interfaces;

namespace ViewModels.ProjectManager
{
    public class ProjectManagerSearchViewModel : BaseSearchModel, ISaveSearch
    {
        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; }
        public string SearchView { get; set; }
    }
}
