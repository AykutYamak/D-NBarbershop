using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IService
{
    public interface IBarberService
    {
        Task<IEnumerable<Barber>> GetBarberBySpeciality(string speciality);
      
    }
}
