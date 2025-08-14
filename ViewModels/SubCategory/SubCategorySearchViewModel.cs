using Enums;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.CRUD.Interfaces;

namespace ViewModels
{
    public class SubCategorySearchViewModel : BaseSearchModel, ISaveSearch
    {
        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; }
        public string SearchView { get; set; }
        public string? Name { get; set; }
    }
}
