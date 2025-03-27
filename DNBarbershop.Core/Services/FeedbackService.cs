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

namespace DNBarbershop.Core.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<Feedback> _feedbackRepository;
        public FeedbackService(IRepository<Feedback> feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
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
            return _feedbackRepository.GetAll();
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
