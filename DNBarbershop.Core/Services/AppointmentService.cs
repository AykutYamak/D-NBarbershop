using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DNBarbershop.Core.IServices;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DNBarbershop.DataAccess.AppointmentRepository;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IAppointmentRepository<Appointment> _appointmentRepo;
        public AppointmentService(IRepository<Appointment> appointmentRepository, IAppointmentRepository<Appointment> appointmentRepo)
        {
            _appointmentRepository = appointmentRepository;
            _appointmentRepo = appointmentRepo;
        }
        public async Task Add(Appointment appointment)
        {
              await _appointmentRepository.Add(appointment);
        }
        public async Task Delete(Guid id)
        {
              await _appointmentRepository.Delete(id);
        }
        public async Task DeleteAll()
        {
            if (await _appointmentRepository.GetCount()<=0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else
            {
                await _appointmentRepository.DeleteAll();
            }
        }
        public async Task<Appointment> Get(Expression<Func<Appointment, bool>> filter)
        {
            return await _appointmentRepository.Get(filter);
        }
        public async Task<Appointment> GetWithRels(Expression<Func<Appointment, bool>> filter)
        {

            return await _appointmentRepo.GetWithRels(filter);
        }

        public IQueryable<Appointment> GetAll()
        {
                return _appointmentRepository.GetAll();
        }
        
        public async Task RemoveRange(IEnumerable<Appointment> entities)
        {
            if (entities.Count()<0)
            {
                throw new ArgumentException("Validation didn't pass.");
            }
            else
            {
                await _appointmentRepository.RemoveRange(entities);
            }
        }
        public async Task Update(Appointment appointment)
        {
            await _appointmentRepository.Update(appointment);
        }

        public async Task<bool> IsTimeSlotAvailable(Guid barberId, DateTime date, TimeSpan startTime, int durationMinutes)
        {
            var endTime = startTime.Add(TimeSpan.FromMinutes(durationMinutes));

            var existingAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.BarberId == barberId && a.AppointmentDate == date)
                .Include(a => a.AppointmentServices)
                .ThenInclude(ap => ap.Service)
                .ToListAsync();

            foreach (var appointment in existingAppointments)
            {
                var appointmentDuration = appointment.AppointmentServices.Sum(s => s.Service.Duration.Hours * 60 + s.Service.Duration.Minutes);
                var appointmentEnd = appointment.AppointmentTime.Add(TimeSpan.FromMinutes(appointmentDuration));

                if (startTime < appointmentEnd && endTime > appointment.AppointmentTime)
                    return false;
            }

            return endTime <= TimeSpan.FromHours(20,30);
        }


        public async Task<List<string>> GenerateTimeSlots(TimeSpan start, TimeSpan end, TimeSpan interval)
        {
            var slots = new List<string>();
            for (var time = start; time < end; time += interval)
            {
                slots.Add(time.ToString(@"hh\:mm"));
            }
            return slots;
        }
    }
}
