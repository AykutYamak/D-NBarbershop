using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.GlobalServiceFolder;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.IServices
{
    public interface IProductService:IGlobalService<Product>
    {
        Task<IEnumerable<Product>> GetProductsUnderPrice(decimal price);
        Task<IEnumerable<Product>> GetProductsOverPrice(decimal price);
        Task<IEnumerable<Product>> GetProductsFromCategory(string category);
    }
}
