using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.ProductRepository
{
    public class ProductRepository<T> : IProductRepository<T> where T : class
    {
        private ApplicationDbContext db;
        internal DbSet<Product> products;
        public ProductRepository(ApplicationDbContext _db)
        {
            db = _db;
            products = db.products;
        }
        public async Task<IEnumerable<Product>> GetProductsFromCategory(string category)
        {
            return await products
                .Where(p => p.Category.Name == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsOverPrice(decimal price)
        {
            return await products
                .Where(p => p.Price > price)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsUnderPrice(decimal price)
        {
            return await products
                .Where(p => p.Price < price)
                .ToListAsync();
        }
    }
}
