﻿using DNBarbershop.Core.GlobalServiceFolder;
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
        /*public bool ValidateService(Service service)
        {
            if (!ServiceValidator.ValidateInput(service.ServiceName,service.Price,service.Duration,service.Description))
            {
                return false;
            }
            if (!ServiceValidator.ServiceExists(service.Id))
            {
                return false;
            }
            return true;
        }*/
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
        public async Task<IEnumerable<Service>> GetAll()
        {
            if (await _serviceRepository.GetCount()<=0)
            {
                throw new ArgumentException("Nothing to get from here.");
            }
            else
            {
                return await _serviceRepository.GetAll();
            }
        }
        public async Task<IEnumerable<Service>> GetServiceUnderPrice(decimal price)
        {
            Expression<Func<Service, bool>> filter = service => service.Price < price;
            return await _serviceRepository.Find(filter);
        }
        public async Task RemoveRange(IEnumerable<Service> entities)
        {
            if (entities.Count() <= 0)
            {
                throw new ArgumentException("Validation didn't pass.");   
            }
            else
            {
                await _serviceRepository.RemoveRange(entities);
            }
        }
        public async Task Update(Guid id, Service service)
        {
            Expression<Func<Service, bool>> filter = service => service.Id== id;
         
                Service entity = _serviceRepository.Get(filter).Result;
                entity = service;
                await _serviceRepository.Update(entity);
          
        }
    }
}
