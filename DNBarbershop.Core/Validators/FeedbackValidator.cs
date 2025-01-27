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
    public class FeedbackValidator
    {
        private static IRepository<Feedback> _repository;
        public static bool ValidateInput(int rating, DateTime date,string comment)
        {
            if (date > DateTime.Now || rating > 5 || rating < 0 || comment.Length > 1000)
            {
                return false;
            }
            return true;
        }
        public static bool FeedbackExists(Guid id)
        {
            Expression<Func<Feedback, bool>> filter = f => f.Id == id;
            if (_repository.Get(filter).Result == null)
            {
                return false;
            }
            return true;
        }
    }
}
