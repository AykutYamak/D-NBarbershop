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
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        public AppointmentService(IRepository<Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        /*public bool ValidateAppointment(Appointment appointment)
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
        }*/
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
        public IQueryable<Appointment> GetAll()
        {
                return _appointmentRepository.GetAll();
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
        public async Task Update(Appointment appointment)
        {
            await _appointmentRepository.Update(appointment);
        }
    }
}
