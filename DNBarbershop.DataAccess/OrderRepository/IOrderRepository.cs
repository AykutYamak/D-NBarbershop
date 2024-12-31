using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.OrderRepository
{
    public interface IOrderRepository<T> where T : class
    {
        Task<decimal> GetTotalAmount(Order order);
        Task<string> GetOrderStatus(Order order);
    }
}
