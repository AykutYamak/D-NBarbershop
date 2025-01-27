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
    public class WorkScheduleValidator
    {
        private static IRepository<WorkSchedule> _repository;
        public static bool ValidateInput(DateTime date)
        {
            if (date > DateTime.Now)
            {
                return false;
            }
            return true;
        }
        public static bool WorkScheduleExists(Guid id)
        {
            Expression<Func<WorkSchedule, bool>> filter = w => w.Id == id;
            if (_repository.Get(filter).Result == null)
            {
                return false;
            }
            return true;
        }
    }
}
