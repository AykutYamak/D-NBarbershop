using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.GlobalServiceFolder;

namespace DNBarbershop.Core.IServices
{
    public interface IAppointmentService : IGlobalService<Appointment>
    {
        Task<bool> IsTimeSlotAvailable(Guid barberId, DateTime date, TimeSpan startTime, int durationMinutes);
        Task<Appointment> GetWithRels(System.Linq.Expressions.Expression<Func<Appointment, bool>> filter);
        Task<List<string>> GenerateTimeSlots(TimeSpan start, TimeSpan end, TimeSpan interval);
    }
}
