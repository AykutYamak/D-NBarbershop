using DNBarbershop.Core.GlobalService.IService;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.GlobalService.Service
{
    public class BarberService : IService.IBarberService
    {
        private readonly IRepository<Barber> _barberRepository;
        public BarberService(IRepository<Barber> appointmentRepository)
        {
            _barberRepository = appointmentRepository;
        }

        public async Task Add(Barber barber)
        {
            await _barberRepository.Add(barber);
        }

        public async Task Delete(Guid id)
        {
            await _barberRepository.Delete(id);
        }

        public async Task DeleteAll()
        {
            await _barberRepository.DeleteAll();
        }

        public async Task<Barber> Get(Expression<Func<Barber, bool>> filter)
        {
            return await _barberRepository.Get(filter);
        }

        public async Task<IEnumerable<Barber>> GetAll()
        {
            return await _barberRepository.GetAll();
        }

        public async Task<IEnumerable<Barber>> GetBarberBySpeciality(string speciality)
        {
            Expression<Func<Barber, bool>> filter = barber => barber.Speciality.Type == speciality;
            return await _barberRepository.Find(filter);
        }

        public async Task RemoveRange(IEnumerable<Barber> entities)
        {
            await _barberRepository.RemoveRange(entities);
        }

        public async Task UpdateByName(string firstName, string lastName)
        {
            Expression<Func<Barber, bool>> filter = barber => barber.FirstName == firstName && barber.LastName == lastName;
            Barber entity = _barberRepository.Get(filter).Result;
            await _barberRepository.Update(entity);
        }
    }
}
