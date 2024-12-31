using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.ServiceRepository
{
    public interface IServiceRepository<T> where T : class
    {
        Task<IEnumerable<Service>> SearchByName(string name);
    }
}
