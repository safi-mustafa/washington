namespace ViewModels.CRUD
{
    public class TabViewModel
    {
        public string ActiveTab { get; set; }
        public string CustomTitleHtml { get; set; }
        public string CustomPartialContentPath { get; set; }
        public string CustomNavHtml { get; set; }
        public string ContainerClassName { get; set; }
        public string Title { get; set; }
        public bool HideTitle { get; set; } = false;
        public string Id { get; set; }
        public string ContentId { get; set; }
        public List<TabItemViewModel> TabItems { get; set; }
        public bool HideTopSearchBar { get; set; } = false;
        public string AddNewCompanyProfileUrl { get; set; }
        public IEnumerable<CompanyProfileViewModel> CompanyProfiles { get; set; }

        public int SelectedTabIndex
        {
            get
            {
                var index = TabItems != null ? TabItems.FindIndex(x => x.Id == ActiveTab) : -1;
                if (index == -1)
                    return 0;
                return index;
            }
        }
    }

    public class TabItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string CreateUrl { get; set; }
        public string Params { get; set; }
        public string Prefix { get; set; }
        public string Postfix { get; set; }
        public bool HideTopSearchBar { get; set; } = false;
    }

    public class CompanyProfileViewModel
    {
        public string Name { get; set; }
        public string Industry { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public int ContactsCount { get; set; }
        public int JobSitesCount { get; set; }
        public int ProjectsCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ViewUrl { get; set; }
        public string EditUrl { get; set; }
        public string DeleteUrl { get; set; }
    }
}
