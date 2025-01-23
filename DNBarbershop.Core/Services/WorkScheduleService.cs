using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Validators;
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
        public bool ValidateWorkSchedule(WorkSchedule workSchedule)
        {
            if (!WorkScheduleValidator.WorkScheduleExists(workSchedule.Id))
            {
                return false;
            }
            if (!WorkScheduleValidator.ValidateInput(workSchedule.WorkDate))
            {
                return false;
            }
            return true;
        }
        public async Task Add(WorkSchedule workSchedule)
        {
            if (ValidateWorkSchedule(workSchedule))
            {
                await _workScheduleRepository.Add(workSchedule);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task Delete(Guid id)
        {
            if (WorkScheduleValidator.WorkScheduleExists(id))
            {
                await _workScheduleRepository.Delete(id);
            }
            else
            {
                throw new ArgumentException("This work schedule doesn't exist.");
            }
        }
        public async Task DeleteAll()
        {
            if (await _workScheduleRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to delete here.");
            }
            else
            {
                await _workScheduleRepository.DeleteAll();
            }
        }
        public async Task<WorkSchedule> Get(Expression<Func<WorkSchedule, bool>> filter)
        {
            if (WorkScheduleValidator.WorkScheduleExists(_workScheduleRepository.Get(filter).Result.Id))
            {
                return await _workScheduleRepository.Get(filter);
            }
            else
            {
                throw new ArgumentException("Validation didn't pass.");
            }
        }
        public async Task<IEnumerable<WorkSchedule>> GetAll()
        {
            if (await _workScheduleRepository.GetCount() <= 0)
            {
                throw new ArgumentException("Nothing to get from here.");
            }
            else
            {
                return await _workScheduleRepository.GetAll();
            }
        }
        public async Task RemoveRange(IEnumerable<WorkSchedule> entities)
        {
            if (entities.Count() <= 0)
            {
                throw new ArgumentException("Validation didn't pass.");
            }
            else
            {
                await _workScheduleRepository.RemoveRange(entities);
            }
        }
        public async Task Update(Guid id, WorkSchedule workSchedule)
        {
            Expression<Func<WorkSchedule, bool>> filter = workSchedule => workSchedule.Id == id;
            if (WorkScheduleValidator.WorkScheduleExists(_workScheduleRepository.Get(filter).Result.Id))
            {
                WorkSchedule entity = _workScheduleRepository.Get(filter).Result;
                entity = workSchedule;
                await _workScheduleRepository.Update(entity);
            }
            else 
            {
                throw new ArgumentException("Work schedule doesn't exist.");
            }

        }
    }
}
