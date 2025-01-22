using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
namespace DNBarbershop.Core.Services
{
    public class WorkScheduleService: IWorkScheduleService
    {
        private readonly IRepository<WorkSchedule> _workScheduleRepository;
        public WorkScheduleService(IRepository<WorkSchedule> workScheduleRepository)
        {
            _workScheduleRepository= workScheduleRepository;
        }
        public async Task Add(WorkSchedule workSchedule)
        {
            await _workScheduleRepository.Add(workSchedule);
        }
        public async Task Delete(Guid id)
        {
            await _workScheduleRepository.Delete(id);
        }
        public async Task DeleteAll()
        {
            await _workScheduleRepository.DeleteAll();
        }
        public async Task<WorkSchedule> Get(Expression<Func<WorkSchedule, bool>> filter)
        {
            return await _workScheduleRepository.Get(filter);
        }
        public async Task<IEnumerable<WorkSchedule>> GetAll()
        {
            return await _workScheduleRepository.GetAll();
        }
        public async Task RemoveRange(IEnumerable<WorkSchedule> entities)
        {
            await _workScheduleRepository.RemoveRange(entities);
        }
        public async Task UpdateByName(Guid id, WorkSchedule workSchedule)
        {
            Expression<Func<WorkSchedule, bool>> filter = workSchedule => workSchedule.Id == id;
            WorkSchedule entity = _workScheduleRepository.Get(filter).Result;
            
            await _workScheduleRepository.Update(entity);
        }
    }
}
