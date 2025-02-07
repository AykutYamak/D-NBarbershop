using DNBarbershop.Core.GlobalServiceFolder;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.IServices
{
    public interface IFeedbackService: IGlobalService<Feedback>
    {
        Task<IEnumerable<Feedback>> GetFeedbackForBarber(string[] barberName);
    }
}
