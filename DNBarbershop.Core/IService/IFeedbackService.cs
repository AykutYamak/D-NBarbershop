using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IService
{
    public interface IFeedbackService
    {
        Task<IEnumerable<Feedback>> GetFeedbackForBarber(string[] barberName);
        Task<IEnumerable<Feedback>> GetFeedbackForService(string service);
    }
}
