using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IService
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByDate(DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByService(string service);
    }
}
