using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.GlobalService;

namespace DNBarbershop.Core.IServices
{
    public interface IAppointmentService : IGlobalService
    {
        
        Task<IEnumerable<Appointment>> GetAppointmentsByDate(DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByService(string service);
    }
}
