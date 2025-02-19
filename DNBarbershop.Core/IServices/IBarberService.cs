using DNBarbershop.Core.GlobalServiceFolder;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IServices
{
    public interface IBarberService:IGlobalService<Barber>
    {
        Task<IEnumerable<Barber>> GetBarberBySpeciality(string speciality);
        Task<IEnumerable<Barber>> GetBarbersWithExperienceAbove(int minExperienceYears);
        Task<IEnumerable<Barber>> SearchBarberByName(string name);
        Task<IEnumerable<WorkSchedule>> GetWorkSchedules(Guid barberId);
    }
}
