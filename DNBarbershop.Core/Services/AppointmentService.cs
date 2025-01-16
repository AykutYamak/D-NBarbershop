using DNBarbershop.Core.IService;
using DNBarbershop.Core.Validators;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DNBarbershop.Core.Service
{
    public class AppointmentService : IAppointmentService
    {
        
        private readonly IRepository<Appointment> _appointmentRepository;
        public AppointmentService(IRepository<Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public bool ValidateAppointment(Appointment appointment)
        {
            if (AppointmentValidator.AppointmentExists(appointment.Id))
            {
                return false;
            }
            if (!AppointmentValidator.UserExists(appointment.UserId))
            {
                return false;
            }
            if (!AppointmentValidator.ServiceExists(appointment.ServiceId))
            {
                return false;
            }
            if (!AppointmentValidator.BarberExists(appointment.BarberId))
            {
                return false;
            }
            return true;
        }
        public async Task Add(Appointment appointment)
        {
            if (ValidateAppointment(appointment))
            {
                await _appointmentRepository.Add(appointment);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task Delete(int id)
        {
            await _appointmentRepository.Delete(id);
        }

        public async Task DeleteAll()
        {
            await _appointmentRepository.DeleteAll();
        }

        public async Task<Appointment> Get(Expression<Func<Appointment, bool>> filter)
        {
            return await _appointmentRepository.Get(filter);
        }

        public async Task<IEnumerable<Appointment>> GetAll()
        {
            return await _appointmentRepository.GetAll();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDate(DateTime date)
        {
            Expression<Func<Appointment, bool>> filter = appointment => appointment.AppointmentDate == date.Date;
            return await _appointmentRepository.Find(filter);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByService(string service)
        {
            Expression<Func<Appointment, bool>> filter = appointment => appointment.Service.ServiceName == service;
            return await _appointmentRepository.Find(filter);
        }

        public async Task RemoveRange(IEnumerable<Appointment> entities)
        {
            await _appointmentRepository.RemoveRange(entities);
        }

        public async Task UpdateByUserName(string[] username)
        {
            Expression<Func<Appointment, bool>> filter = appointment => appointment.User.FirstName == username[0] && appointment.User.LastName == username[1];
            Appointment entity = _appointmentRepository.Get(filter).Result;
            await _appointmentRepository.Update(entity);
        }
    }
}
