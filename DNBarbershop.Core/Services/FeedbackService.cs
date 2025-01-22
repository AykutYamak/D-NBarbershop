using DNBarbershop.Core.GlobalService.IService;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.GlobalService.Service
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<Feedback> _feedbackRepository;
        public FeedbackService(IRepository<Feedback> feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
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
    }
}
