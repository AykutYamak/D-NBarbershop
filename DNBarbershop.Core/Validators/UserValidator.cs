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
    class UserValidator
    {
        private static IRepository<User> _repo;
        public static bool ValidateInput(string FirstName, string LastName, string Email, string Password, string phoneNumber)
        {
            if (FirstName.Length == 0 || FirstName == null || LastName.Length == 0 || LastName == null || Email.Length == 0 || Email == null || Password == null || Password.Length == 0 || phoneNumber == null || phoneNumber.Length == 0)
            {
                return false;
            }
            return true;
        }
        public static bool UserExists(Guid id)
        {
            Expression<Func<User, bool>> filter = u => u.Id == id;
            if (_repo.Get(filter) == null)
            {
                return false;
            }
            return true;
        }
    }
}
