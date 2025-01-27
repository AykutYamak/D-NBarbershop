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
    public class ServiceValidator
    {
        private static IRepository<Service> _repository;
        public static bool ValidateInput(string serviceName, decimal Price, TimeSpan Duration, string Description)
        {
            if (serviceName.Length == 0 || serviceName == null || Duration.Hours>2 || Duration.Minutes>60 || Duration.Seconds>60 || Duration.Minutes<0 || Duration.Hours<0 || Duration.Seconds<0 || Description.Length > 500 || Description == null) 
            {
                return false;
            }
            return true;
        }
        public static bool ServiceExists(Guid id)
        {
            Expression<Func<Service, bool>> filter = s => s.Id == id;
            if (_repository.Get(filter).Result == null)
            {
                return false;
            }
            return true;
        }
    }
}
