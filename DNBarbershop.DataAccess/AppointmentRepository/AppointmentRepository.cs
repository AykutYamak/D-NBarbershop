using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.AppointmentRepository
{
    public class AppointmentRepository<T> : IAppointmentRepository<T> where T : class
    {
        private ApplicationDbContext db;
        public DbSet<Appointment> dbSet;

        public AppointmentRepository(ApplicationDbContext _db)
        {
            db = _db;
            dbSet = db.Set<Appointment>();
        }
        public async Task<Appointment> GetWithRels(Expression<Func<Appointment, bool>> filter)
        {
            return await dbSet.Include(x => x.AppointmentServices).Include(x => x.Barber).FirstOrDefaultAsync(filter);
        }
    }
}
