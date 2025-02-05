using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using DNBarbershop.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DNBarbershop.Core.Validators;

namespace DNBarbershop.Core.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<Feedback> _feedbackRepository;
        public FeedbackService(IRepository<Feedback> feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        /*public bool ValidateFeedback(Feedback feedback)
        {
            if (!FeedbackValidator.ValidateInput(feedback.Rating,feedback.FeedBackDate,feedback.Comment))
            {
                return false;
            }
            if (!FeedbackValidator.FeedbackExists(feedback.Id))
            {
                return false;
            }
            return true;
        }*/
        public async Task Add(Feedback feedback)
        {
                await _feedbackRepository.Add(feedback);
          
        }
        public async Task Delete(Guid id)
        {
                await _feedbackRepository.Delete(id);
            
        }
        public async Task DeleteAll()
        {
            if (await _feedbackRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else
            {
                await _feedbackRepository.DeleteAll();
            }
        }
        public async Task<Feedback> Get(Expression<Func<Feedback, bool>> filter)
        {
          
                return await _feedbackRepository.Get(filter);
          
        }
        public IQueryable<Feedback> GetAll()
        {
            if (_feedbackRepository.GetCount().Result <= 0)
            {
                throw new ArgumentException("Nothing to get from here");
            }
            else
            {
                return _feedbackRepository.GetAll();
            }

        }
        public async Task<IEnumerable<Feedback>> GetFeedbackForBarber(string[] barberName)
        {
            Expression<Func<Feedback,bool>> filter = feedback => feedback.Barber.FirstName.Equals(barberName[0]) && feedback.Barber.LastName.Equals(barberName[1]);
            return await _feedbackRepository.Find(filter);
        }
        public async Task<IEnumerable<Feedback>> GetFeedbackForService(string service)
        {
            Expression<Func<Feedback, bool>> filter = feedback => feedback.Service.ServiceName.Equals(service);
            return await _feedbackRepository.Find(filter);
        }
        public async Task RemoveRange(IEnumerable<Feedback> entities)
        {
            if (entities.Count() < 0)
            {
                throw new ArgumentException("Validation didn't pass.");
            }
            else
            {
                await _feedbackRepository.RemoveRange(entities);
            }
        }
        public async Task Update(Feedback feedback)
        {
                await _feedbackRepository.Update(feedback);   
        }
    }
}
