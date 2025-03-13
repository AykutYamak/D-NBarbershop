using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.BarberRepository
{
    public interface IBarberRepository<T> where T : class
    {
        Task<IEnumerable<Barber>> GetAll();
    }
}
