using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.AppointmentServiceRepository;
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
    public class AppointmentServiceService : IAppointmentServiceService
    {

        private readonly IRepository<AppointmentServices> _Repository;
        private readonly IAppointmentServiceRepository<AppointmentServices> appointmentServiceRepository;

        public AppointmentServiceService(IRepository<AppointmentServices> Repository, IAppointmentServiceRepository<AppointmentServices> _appointmentServiceRepository)
        {
            _Repository = Repository;
            appointmentServiceRepository = _appointmentServiceRepository;
        }

        public async Task Add(AppointmentServices entity)
        {
            await _Repository.Add(entity);
        }
        public async Task Delete(Guid id, Guid serviceId)
        {
            await appointmentServiceRepository.Delete(id, serviceId);
        }
        public async Task DeleteByAppointmentId(Guid appointmentId)
        {
            await appointmentServiceRepository.DeleteByAppointmentId(appointmentId);
        }


        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAll()
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentServices> Get(Expression<Func<AppointmentServices, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public IQueryable<AppointmentServices> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<AppointmentServices> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Update(AppointmentServices entity)
        {
            await _Repository.Update(entity);
        }
        
    }
}
