using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.OrderRepository
{
    public class OrderRepository<T> : IOrderRepository<T> where T : class
    {
        private ApplicationDbContext db;
        internal DbSet<Order> orders;
        public OrderRepository(ApplicationDbContext _db)
        {
            db = _db;
            orders = db.orders;
        }

        public async Task<decimal> GetTotalAmount(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetOrderStatus(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
