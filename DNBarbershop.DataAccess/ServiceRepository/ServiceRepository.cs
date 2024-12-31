using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.ServiceRepository
{
    public class ServiceRepository<T> : IServiceRepository<T> where T : class
    {
        private ApplicationDbContext db;
        internal DbSet<Service> services;
        public ServiceRepository(ApplicationDbContext _db)
        {
            db = _db;
            services = db.services;
        }

        public async Task<IEnumerable<Service>> SearchByName(string name)
        {
            return await services.Where(s => s.ServiceName == name).ToListAsync();
        }
    }
}
