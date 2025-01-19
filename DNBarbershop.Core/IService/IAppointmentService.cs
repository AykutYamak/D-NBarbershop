using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IService
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAll();
        Task<Appointment> Get(Expression<Func<Appointment, bool>> filter);
        Task Add(Appointment appointment);
        Task Delete(Guid id);
        Task RemoveRange(IEnumerable<Appointment> entities);
        Task UpdateByUserName(string[] username);
        Task DeleteAll();
        Task<IEnumerable<Appointment>> GetAppointmentsByDate(DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByService(string service);
    }
}
