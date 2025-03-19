using DNBarbershop.Core.GlobalServiceFolder;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Validators;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IRepository<Service> _serviceRepository;
        public ServiceService(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        public async Task Add(Service service)
        {
                await _serviceRepository.Add(service);
             
        }
        public async Task Delete(Guid id)
        {
            
                await _serviceRepository.Delete(id);
            
        }
        public async Task DeleteAll()
        {
            if (await _serviceRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else 
            {
                await _serviceRepository.DeleteAll();
            }
        }
        public async Task<Service> Get(Expression<Func<Service, bool>> filter)
        {
            
                return await _serviceRepository.Get(filter);
         
        }
        public IQueryable<Service> GetAll()
        {
            //if (await _serviceRepository.GetCount()<=0)
            //{
                //throw new ArgumentException("Nothing to get from here.");
            //}
            //else
            //{
                return _serviceRepository.GetAll();
            //}
        }
        public async Task<IEnumerable<Service>> GetServiceUnderPrice(decimal price)
        {
            Expression<Func<Service, bool>> filter = service => service.Price < price;
            return await _serviceRepository.Find(filter);
        }
        public async Task RemoveRange(IEnumerable<Service> entities)
        {
                await _serviceRepository.RemoveRange(entities);
        }
        public async Task Update(Service service)
        {
                await _serviceRepository.Update(service); 
        }
    }
}
