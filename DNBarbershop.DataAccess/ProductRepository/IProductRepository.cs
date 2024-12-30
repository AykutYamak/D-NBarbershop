using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.ProductRepository
{
    public interface IProductRepository<T> where T : class
    {
        Task<IEnumerable<Product>> GetProductsUnderPrice(decimal price);
        Task<IEnumerable<Product>> GetProductsOverPrice(decimal price);
        Task<IEnumerable<Product>> GetProductsFromCategory(string category);
    }
}
