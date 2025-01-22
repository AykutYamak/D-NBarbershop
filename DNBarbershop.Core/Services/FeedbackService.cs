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
            await _feedbackRepository.DeleteAll();
        }
        public async Task<Feedback> Get(Expression<Func<Feedback, bool>> filter)
        {
            return await _feedbackRepository.Get(filter);
        }
        public async Task<IEnumerable<Feedback>> GetAll()
        {
            return await _feedbackRepository.GetAll();
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
            await _feedbackRepository.RemoveRange(entities);
        }
        public async Task UpdateByName(Guid id, Feedback feedback)
        {
            Expression<Func<Feedback, bool>> filter = feedback => feedback.Id == id;
            Feedback entity = _feedbackRepository.Get(filter).Result;
            entity = feedback;
            await _feedbackRepository.Update(entity);
        }
    }
}
