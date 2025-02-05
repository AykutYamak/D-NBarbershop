using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.BarberRepository;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Services
{
    internal class AppointmentServiceService:IAppointmentServiceService
    {

        private readonly IRepository<AppointmentServices> _Repository;

        public AppointmentServiceService(IRepository<AppointmentServices> Repository)
        {
                _Repository = Repository;
        }

        public async Task Add(AppointmentServices entity)
        {
            await _Repository.Add(entity);
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

        public Task Update(AppointmentServices entity)
        {
            throw new NotImplementedException();
        }
    }
}
