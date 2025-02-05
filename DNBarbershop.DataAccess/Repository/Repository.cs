using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext db;
        public DbSet<T> dbSet;
        public Repository(ApplicationDbContext _db)
        {
            db= _db;
            dbSet = db.Set<T>();
        }
        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAll()
        {
            dbSet.RemoveRange(dbSet);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> filter)
        {
            return await dbSet.Where(filter).ToListAsync();
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            return await dbSet.FirstOrDefaultAsync(filter);
        }
        public IQueryable<T> GetAll()
        {
            return dbSet.AsQueryable();           
        }
        public async Task Delete(Guid id)
        {
            var entity = dbSet.Find(id);
            if (entity == null) 
            {
                throw new ArgumentException($"Entity with ID {id} not found.");
            }
            dbSet.Remove(entity);
            await db.SaveChangesAsync();
        }
        public async Task DeleteStringId(Guid id)
        {
            var entity = dbSet.Find(id.ToString());
            if (entity == null)
            {
                throw new ArgumentException($"Entity with ID {id} not found.");
            }
            dbSet.Remove(entity);
            await db.SaveChangesAsync();
        }
        public async Task RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            await db.SaveChangesAsync();
        }
        public async Task Update(T entity)
        {
            dbSet.Update(entity);
            await db.SaveChangesAsync();
        }
        public async Task<int> GetCount()
        {
            return await dbSet.CountAsync();
        }
    }
}
