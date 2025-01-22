using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.IService;
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
        public async Task Add(Speciality speciality)
        {
            await _specialityRepository.Add(speciality);
        }
        public async Task Delete(Guid id)
        {
            await _specialityRepository.Delete(id);
        }
        public async Task DeleteAll()
        {
            await _specialityRepository.DeleteAll();
        }
        public async Task<Speciality> Get(Expression<Func<Speciality, bool>> filter)
        {
            return await _specialityRepository.Get(filter);
        }
        public async Task<IEnumerable<Speciality>> GetAll()
        {
            return await _specialityRepository.GetAll();
        }
        public async Task RemoveRange(IEnumerable<Speciality> entities)
        {
            await _specialityRepository.RemoveRange(entities);
        }
        public async Task UpdateByName(Guid id, Speciality speciality)
        {
            Expression<Func<Speciality, bool>> filter = speciality => speciality.Id == id;
            Speciality entity = _specialityRepository.Get(filter).Result;
            entity = speciality;
            await _specialityRepository.Update(entity);
        }
    }
}
