using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.DataAccess.BarberRepository;
using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.DataAccess.AppointmentServiceRepository
{
    public class AppointmentServiceRepository<T> : IAppointmentServiceRepository<T> where T : class
    {

        private ApplicationDbContext db;
        internal DbSet<AppointmentServices> dbSet;

        public AppointmentServiceRepository(ApplicationDbContext _db)
        {
            db = _db;
            dbSet = db.appointmentServices;
        }

        public async Task Delete(Guid appointmentId, Guid serviceId)
        {
            var entity = await dbSet.SingleOrDefaultAsync(e => e.AppointmentId == appointmentId && e.ServiceId == serviceId);
            if (entity == null)
            {
                throw new ArgumentException($"Entity with AppointmentID {appointmentId} and ServiceID {serviceId} not found.");
            }
            dbSet.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}
