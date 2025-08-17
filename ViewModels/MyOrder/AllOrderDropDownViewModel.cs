using Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.MyOrder
{
    public class AllOrderDropDownViewModel
    {
        public List<CompanyInformation> Customers { get; set; }
        public List<Models.CustomerProject> Projects { get; set; }
    }

}   
