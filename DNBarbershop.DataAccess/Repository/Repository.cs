using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext _db)
        {
            db= _db;
            dbSet = db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public void Find(Expression<Func<T, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
