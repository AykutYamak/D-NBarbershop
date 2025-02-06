using DNBarbershop.Core.GlobalServiceFolder;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IServices
{
    public interface IAppointmentServiceService : IGlobalService<AppointmentServices>
    {
        Task Delete(Guid id, Guid serviceId);
    }
}
