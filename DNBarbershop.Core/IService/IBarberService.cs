using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IService
{
    public interface IBarberService
    {
        Task<IEnumerable<Barber>> GetAll();
        Task<Barber> Get(Expression<Func<Barber, bool>> filter);
        Task Add(Barber appointment);
        Task Delete(int id);
        Task RemoveRange(IEnumerable<Barber> entities);
        Task UpdateByName(string firstName,string lastName);
        Task DeleteAll();
        Task<IEnumerable<Barber>> GetBarberBySpeciality(string speciality);
      
    }
}
