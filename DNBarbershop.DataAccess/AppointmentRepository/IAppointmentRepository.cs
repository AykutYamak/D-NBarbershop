﻿
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.AppointmentRepository
{
    public interface IAppointmentRepository<T> where T : class
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByService(string service);
        Task<IEnumerable<Appointment>> GetAppointmentsByDate(DateTime date);
    }
}