using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
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
        public BarberService(IRepository<Barber> barberRepository)
        {
            _barberRepository = barberRepository;

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
                return await _barberRepository.Get(filter);
            
        }
        public IQueryable<Barber> GetAll()
        {
            return _barberRepository.GetAll();
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
        public async Task Update(Barber barber)
        {
            await _barberRepository.Update(barber);
        }


       
    }
}
