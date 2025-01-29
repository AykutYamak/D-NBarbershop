using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.GlobalServiceFolder
{
    public class GlobalService<T> : IGlobalService<T> where T : class
    {
        private readonly IRepository<T> _repository;
        public async Task Add(T entity)
        {
            await _repository.Add(entity);
        }
            
        public async Task Delete(Guid id)
        {
            await _repository.Delete(id);
        }

        public async Task DeleteAll()
        {
            await _repository.DeleteAll();
        }

        public async Task<T> Get(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            return await _repository.Get(filter);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task RemoveRange(IEnumerable<T> entities)
        {
            await _repository.RemoveRange(entities);
        }

        public async Task Update(T entity)
        {
            await _repository.Update(entity);
        }
    }
}
