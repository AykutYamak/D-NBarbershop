using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.Validators
{
    internal class SpecialityValidator
    {
        private static IRepository<Speciality> _repository;
        public static bool ValidateInput(string type)
        {
            if (type==null || type.Length<=0)
            {
                return false;
            }
            return true;
        }
        public static bool SpecialityExists(Guid id)
        {
            Expression<Func<Speciality, bool>> filter = s => s.Id == id;
            if (_repository.Get(filter).Result == null)
            {
                return false;
            }
            return true;
        }
    }
}
