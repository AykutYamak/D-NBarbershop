using System;
using DNBarbershop.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IService
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetServiceUnderPrice(decimal price);
    }
}
