using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Validators
{
    public class BarberValidator
    {
        private static IRepository<Barber> _repository;
        public static bool ValidateInput(int ExperienceYears,string firstName, string lastName)
        {
            if (ExperienceYears < 0 || firstName==null || firstName.Length==0 || lastName == null || lastName.Length == 0)
            {
                return false;
            }
            return true;
        }
        public static bool BarberExists(Guid id)
        {
            Expression<Func<Barber, bool>> filter = a => a.Id == id;
            if (_repository.Get(filter).Result == null)
            {
                return false;
            }
            return true;
        }
    }
}
