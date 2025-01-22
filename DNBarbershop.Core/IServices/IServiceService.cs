using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.GlobalServiceFolder;

namespace DNBarbershop.Core.IServices
{
    public interface IServiceService:IGlobalService<DNBarbershop.Models.Entities.Service>
    {
        Task<IEnumerable<DNBarbershop.Models.Entities.Service>> GetServiceUnderPrice(decimal price);
    }
}
