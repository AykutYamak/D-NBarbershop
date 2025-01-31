using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(Expression<Func<T, bool>> filter);
        Task Add(T entity);
        Task Delete(Guid id);
        Task RemoveRange(IEnumerable<T> entities);
        Task Update(T entity);
        Task DeleteAll();
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> filter);
        Task<int> GetCount();
        Task DeleteStringId(Guid id);
    }
}
