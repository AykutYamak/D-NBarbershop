using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.GlobalService.IService
{
    public interface IServiceService
    {
        Task<IEnumerable<DNBarbershop.Models.Entities.Service>> GetServiceUnderPrice(decimal price);
    }
}
