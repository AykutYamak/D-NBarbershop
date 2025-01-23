using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.DataAccess.AppointmentRepository
{
    public class AppointmentRepository<T> : IAppointmentRepository<T> where T : class
    {
        
    }
}
