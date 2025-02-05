using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Core.Services
{
    public class UserService: IUserService
    {
        private readonly IRepository<User> _userRepository;
        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task Add(User entity)
        {
            await _userRepository.Add(entity);
        }

        public async Task Delete(Guid id)
        {
            await _userRepository.Delete(id);
        }

        public async Task DeleteStringId(Guid id)
        {
            await _userRepository.DeleteStringId(id);
        }

        public async Task DeleteAll()
        {
            if (await _userRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else
            {
                await _userRepository.DeleteAll();
            }
        }

        public async Task<User> Get(Expression<Func<User, bool>> filter)
        {
            return await _userRepository.Get(filter);
        }

        public IQueryable<User> GetAll()
        {
                return _userRepository.GetAll();
        }

        public async Task RemoveRange(IEnumerable<User> entities)
        {
            if (entities.Count() < 0)
            {
                throw new ArgumentException("Validation didn't pass.");
            }
            else
            {
                await _userRepository.RemoveRange(entities);
            }
        }

        public async Task Update(User entity)
        {
            await _userRepository.Update(entity);
        }
    }
}
