using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _prodRepository;
        public ProductService(IRepository<Product> prodRepository)
        {
            _prodRepository = prodRepository;
        }
        public Task Add(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAll()
        {
            throw new NotImplementedException();
        }

        public Task<Product> Get(Expression<Func<Product, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAll()
        {
            //throw new NotImplementedException();

            return _prodRepository.GetAll();
        }

        public Task<IEnumerable<Product>> GetProductsFromCategory(string category)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetProductsOverPrice(decimal price)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetProductsUnderPrice(decimal price)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<Product> entities)
        {
            throw new NotImplementedException();
        }

        public Task Update(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
