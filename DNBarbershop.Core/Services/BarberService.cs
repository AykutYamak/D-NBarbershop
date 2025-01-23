using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Validators;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Services
{
    public class BarberService : IBarberService
    {
        private readonly IRepository<Barber> _barberRepository;
        public BarberService(IRepository<Barber> appointmentRepository)
        {
            _barberRepository = appointmentRepository;
        }
        public bool ValidateBarber(Barber barber)
        {
            if (!BarberValidator.ValidateInput(barber.ExperienceYears,barber.FirstName,barber.LastName))
            {
                return false;
            }
            if (!BarberValidator.BarberExists(barber.Id)) 
            {
                return false;
            }
            return true;
        }
        public async Task Add(Barber barber)
        {
            if (ValidateBarber(barber))
            {
                await _barberRepository.Add(barber);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task Delete(Guid id)
        {
            if (BarberValidator.BarberExists(id))
            {
                await _barberRepository.Delete(id);
            }
            else
            {
                throw new ArgumentException("This barber doesn't exist.");
            }
        }
        public async Task DeleteAll()
        {
            if (await _barberRepository.GetCount()<=0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else
            {
                await _barberRepository.DeleteAll();
            }
        }
        public async Task<Barber> Get(Expression<Func<Barber, bool>> filter)
        {
            if (BarberValidator.BarberExists(_barberRepository.Get(filter).Result.Id))
            {
                return await _barberRepository.Get(filter);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task<IEnumerable<Barber>> GetAll()
        {
            if (await _barberRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to get from here");
            }
            else
            {
                return await _barberRepository.GetAll();
            }
        }
        public async Task<IEnumerable<Barber>> GetBarberBySpeciality(string speciality)
        {
            Expression<Func<Barber, bool>> filter = barber => barber.Speciality.Type == speciality;
            return await _barberRepository.Find(filter);
        }
        public async Task RemoveRange(IEnumerable<Barber> entities)
        {
            if (entities.Count() < 0)
            {
                throw new ArgumentException("Validation didn't pass.");
            }
            else
            {
                await _barberRepository.RemoveRange(entities);
            }
        }
        public async Task Update(Guid id, Barber barber)
        {
            Expression<Func<Barber, bool>> filter = barber => barber.Id == id;
            if (BarberValidator.BarberExists(_barberRepository.Get(filter).Result.Id))
            {
                Barber entity = _barberRepository.Get(filter).Result;
                entity = barber;
                await _barberRepository.Update(entity);
            }
            else 
            {
                throw new ArgumentException("Barber doesn't exist.");
            }
        }

        public async Task<IEnumerable<Barber>> GetBarbersWithExperienceAbove(int minExperienceYears)
        {
            Expression<Func<Barber, bool>> filter = barber => barber.ExperienceYears >= minExperienceYears;
            return await _barberRepository.Find(filter);
        }

        public async Task<IEnumerable<Barber>> SearchBarberByName(string name)
        {
            Expression<Func<Barber, bool>> filter = barber => barber.FirstName == name || barber.LastName == name;
            return await _barberRepository.Find(filter);
        }
    }
}
