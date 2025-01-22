using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Validators
{
    public class AppointmentValidator
    {
        private static IRepository<Appointment> _repo;
        public static bool ValidateInput(DateTime date)
        {
            if (date>DateTime.Now)
            {
                return false;
            }
            return true;
        }
        public static bool AppointmentExists(Guid id)
        {
            Expression<Func<Appointment, bool>> filter = a => a.Id == id;
            if (_repo.Get(filter)==null)
            {
                return false;
            }
            return true;
        }
    }
}
