﻿
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.AppointmentRepository
{
    public interface IAppointmentRepository<T> where T : class
    {
        Task<Appointment> GetWithRels(Expression<Func<Appointment, bool>> filter);
    }
}
