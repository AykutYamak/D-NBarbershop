using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.AppointmentRepository
{
    public class AppointmentRepository<T> : IAppointmentRepository<T> where T : class
    {
        private ApplicationDbContext db;
        internal DbSet<Appointment> appointments;
        public AppointmentRepository(ApplicationDbContext _db)
        {
            db = _db;
            appointments = db.appointments;
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByDate(DateTime date)
        {
            return await appointments.Where(a => a.AppointmentDate == date).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByService(string service)
        {
            return await appointments.Where(a => a.Service.ServiceName == service).ToListAsync();
        }
    }
}
