using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.GlobalServiceFolder
{
    public interface IGlobalService<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(Expression<Func<T, bool>> filter);
        Task Add(T entity);
        Task Delete(Guid id);
        Task RemoveRange(IEnumerable<T> entities);
        Task UpdateByName(Guid id, T entity);
        Task DeleteAll();
    }
}
