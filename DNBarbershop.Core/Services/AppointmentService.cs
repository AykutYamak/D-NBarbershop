using DNBarbershop.Core.Validators;
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

namespace DNBarbershop.Core.Services
{
    public class AppointmentServices : IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        public AppointmentServices(IRepository<Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public bool ValidateAppointment(Appointment appointment)
        {
            if (!AppointmentValidator.ValidateInput(appointment.AppointmentDate))
            {
                return false;
            }
            if (!AppointmentValidator.AppointmentExists(appointment.Id))
            {
                return false;
            }
            if (!UserValidator.UserExists(appointment.User.Id))
            {
                return false;
            }
            if (!BarberValidator.BarberExists(appointment.BarberId))
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
        public async Task Delete(Guid id)
        {
            if (AppointmentValidator.AppointmentExists(id))
            {
                await _appointmentRepository.Delete(id);
            }
            else
            {
                throw new ArgumentException("This appointment doesn't exist.");
            }
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
            if (AppointmentValidator.AppointmentExists(_appointmentRepository.Get(filter).Result.Id))
            {
                return await _appointmentRepository.Get(filter);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task<IEnumerable<Appointment>> GetAll()
        {
            if (await _appointmentRepository.GetCount()<=0)
            {
                throw new ArgumentException("Nothing to get from here");
            }
            else
            {
                return await _appointmentRepository.GetAll();
            }
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDate(DateTime date)
        {
            Expression<Func<Appointment, bool>> filter = appointment => appointment.AppointmentDate == date.Date;
            return await _appointmentRepository.Find(filter);
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByService(string service)
        {
            Expression<Func<Appointment, bool>> filter = appointment => appointment.AppointmentServices.Any(s => s.Service.ServiceName == service);
            return await _appointmentRepository.Find(filter);
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
        public async Task Update(Guid id, Appointment appointment)
        {
            Expression<Func<Appointment, bool>> filter = appointment => appointment.Id == id;
            if (AppointmentValidator.AppointmentExists(_appointmentRepository.Get(filter).Result.Id))
            {
                Appointment entity = _appointmentRepository.Get(filter).Result;
                entity = appointment;
                await _appointmentRepository.Update(entity);
            }
            else
            {
                throw new ArgumentException("Appointment doesn't exist.");
            }
        }
    }
}
