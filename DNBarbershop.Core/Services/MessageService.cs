using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DNBarbershop.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<Message> _messageRepository;
        public MessageService(IRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task Add(Message message)
        {
            await _messageRepository.Add(message);

        }
        public async Task Delete(Guid id)
        {
            await _messageRepository.Delete(id);

        }
        public async Task DeleteAll()
        {
            if (await _messageRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else
            {
                await _messageRepository.DeleteAll();
            }
        }
        public async Task<Message> Get(Expression<Func<Message, bool>> filter)
        {

            return await _messageRepository.Get(filter);

        }
        public IQueryable<Message> GetAll()
        {
            return _messageRepository.GetAll();
        }

        public async Task RemoveRange(IEnumerable<Message> entities)
        {
            if (entities.Count() < 0)
            {
                throw new ArgumentException("Validation didn't pass.");
            }
            else
            {
                await _messageRepository.RemoveRange(entities);
            }
        }
        public async Task Update(Message message)
        {
            await _messageRepository.Update(message);
        }
    }
}
