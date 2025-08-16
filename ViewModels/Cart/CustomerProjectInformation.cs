using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Cart
{
    public class CustomerProjectInformation
    {
        public List<CompanyInformation> Companies { get; set; }
        public List<Models.CustomerProject> Projects { get; set; }
        public List<Models.WorkOrder> WorkOrders { get; set; }
    }
}
