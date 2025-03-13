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
            return await barbers.Include(b => b.Speciality).ToListAsync();
        }
    }
}
