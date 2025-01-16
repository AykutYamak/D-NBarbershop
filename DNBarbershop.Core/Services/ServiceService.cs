using DNBarbershop.Core.IService;
using DNBarbershop.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Service
{
    public class ServiceService : IServiceService
    {
        private readonly IRepository<DNBarbershop.Models.Entities.Service> _serviceRepository;
        public ServiceService(IRepository<DNBarbershop.Models.Entities.Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        public async Task<IEnumerable<Models.Entities.Service>> GetServiceUnderPrice(decimal price)
        {
            Expression<Func<Models.Entities.Service, bool>> filter = service => service.Price < price;
            return await _serviceRepository.Find(filter);
        }
    }
}
