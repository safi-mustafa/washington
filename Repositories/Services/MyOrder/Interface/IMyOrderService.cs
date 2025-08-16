using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ViewModels.MyOrder;

namespace Repositories.Services.MyOrder.Interface
{
    public interface IMyOrderService
    {
        Task<List<AllOrderViewModel>> GetAllOrders();
    }
}
