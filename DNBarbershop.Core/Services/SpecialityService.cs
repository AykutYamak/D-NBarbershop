using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.IService;
using DNBarbershop.Core.Validators;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.Services
{
    public class SpecialityService : ISpecialityService
    {
        private readonly IRepository<Speciality> _specialityRepository;
        public SpecialityService(IRepository<Speciality> specialityRepository)
        {
            _specialityRepository = specialityRepository;
        }
        public bool ValidateSpeciality(Speciality speciality)
        {
            if (!SpecialityValidator.ValidateInput(speciality.Type))
            {
                return false;
            }
            if (!SpecialityValidator.SpecialityExists(speciality.Id))
            {
                return false;
            }
            return true;
        }
        public async Task Add(Speciality speciality)
        {
            if (ValidateSpeciality(speciality))
            {
                await _specialityRepository.Add(speciality);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task Delete(Guid id)
        {
            if (SpecialityValidator.SpecialityExists(id))
            {
                await _specialityRepository.Delete(id);
            }
            else
            {
                throw new ArgumentException("This speciality doesn't exist.");
            }
        }
        public async Task DeleteAll()
        {
            if (await _specialityRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else
            {
                await _specialityRepository.DeleteAll();
            }
        }
        public async Task<Speciality> Get(Expression<Func<Speciality, bool>> filter)
        {
            if (SpecialityValidator.SpecialityExists(_specialityRepository.Get(filter).Result.Id))
            {
                return await _specialityRepository.Get(filter);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task<IEnumerable<Speciality>> GetAll()
        {
            if (await _specialityRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to get from here.");
            }
            else
            {
                return await _specialityRepository.GetAll();
            }
        }
        public async Task RemoveRange(IEnumerable<Speciality> entities)
        {
            if (entities.Count() <= 0)
            {
                throw new ArgumentException("Validation didn't pass.");
            }
            else
            {
                await _specialityRepository.RemoveRange(entities);
            }
        }
        public async Task Update(Guid id, Speciality speciality)
        {
            Expression<Func<Speciality, bool>> filter = speciality => speciality.Id == id;
            if (SpecialityValidator.SpecialityExists(_specialityRepository.Get(filter).Result.Id))
            {
                Speciality entity = _specialityRepository.Get(filter).Result;
                entity = speciality;
                await _specialityRepository.Update(entity);
            }
            else 
            {
                throw new ArgumentException("Service doesn't exist.");
            }
        }
    }
}
