using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.AppointmentServiceRepository
{
    public interface IAppointmentServiceRepository<T> where T : class
    {
        Task Delete(Guid appointmentId, Guid serviceId);
        Task DeleteByAppointmentId(Guid appointmentId);

    }
}
