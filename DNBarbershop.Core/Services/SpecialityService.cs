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
                return await _specialityRepository.Get(filter);
      
        }
        public IQueryable<Speciality> GetAll()
        {
                return _specialityRepository.GetAll();
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
        public async Task Update(Speciality speciality)
        {
            await _specialityRepository.Update(speciality);
        }
    }
}
