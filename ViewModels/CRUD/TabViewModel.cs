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

    public class ReportsCountViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalActiveRentals { get; set; }
        public int OverdueReturns { get; set; }
    }

    public class ActiveRentalsModel
    {
        public string Order { get; set; }
        public string Item { get; set; }
        public string Project { get; set; }
        public DateTime? DueDate { get; set; }
        public string DaysLeft { get; set; }
        public string DailyCost { get; set; }
    }
    public class ReportMasterViewModel
    {
        public TabViewModel TabData { get; set; }
        public ReportsCountViewModel ReportsCount { get; set; } // Type based on _reportsService.Orders()
        public List<ActiveRentalsModel> ActiveRentals { get; set; }
        public List<CustomerProjectsViewModel> CustomerProjects { get; set; }
    }

    public class CustomerProjectsViewModel
    {
        public string JobName { get; set; }
        public double? TotalCost { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public double? PercentageComplete { get; set; }
    }
}
