using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.BarberRepository
{
    public class BarberRepository<T> : IBarberRepository<T> where T : class
    {
        private ApplicationDbContext db;
        internal DbSet<Barber> barbers;
        public BarberRepository(ApplicationDbContext _db)
        {
            db = _db;
            barbers = db.barbers;
        }

        public async Task<IEnumerable<Barber>> GetAll()
        {
            return await barbers.ToListAsync();
        }

        public async Task<IEnumerable<Barber>> GetBarbersWithExperienceAbove(int minExperienceYears)
        {
            return await barbers
                .Where(b => b.ExperienceYears >= minExperienceYears).ToListAsync();
        }

        public async Task<IEnumerable<Barber>> SearchBarberByName(string name)
        {
            return await barbers
                .Where(b => b.FirstName == name || b.LastName == name).ToListAsync();
        }

        public async Task<IEnumerable<Barber>> GetBarbersAvailableAt(DateTime date)
        {
            return await barbers
                .Where(b => b.WorkSchedules
                .Any(datetime => datetime.WorkDate == date.Date &&
                datetime.StartTime.Hour <= date.Hour &&
                datetime.StartTime.Minute <= date.Minute &&
                datetime.EndTime.Hour >= date.Hour &&
                datetime.EndTime.Minute >= date.Minute))
                .ToListAsync();
        }
    }
}
