using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.GlobalServiceFolder;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.IServices
{
    public interface IUserService : IGlobalService<User>
    {
        Task DeleteStringId(Guid id);
    }
}
