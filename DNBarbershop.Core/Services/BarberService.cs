using DNBarbershop.Core.IService;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Service
{
    public class BarberService : IBarberService
    {
        private readonly IRepository<Barber> _barberRepository;
        public BarberService(IRepository<Barber> appointmentRepository)
        {
            _barberRepository = appointmentRepository;
        }
        public async Task<IEnumerable<Barber>> GetBarberBySpeciality(string speciality)
        {
            Expression<Func<Barber, bool>> filter = barber => barber.Speciality.Type == speciality;
            return await _barberRepository.Find(filter);
        }
    }
}
