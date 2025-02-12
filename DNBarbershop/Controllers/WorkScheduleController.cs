using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.DataAccess;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Barbers;
using DNBarbershop.Models.ViewModels.Specialities;
using DNBarbershop.Models.ViewModels.WorkSchedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DNBarbershop.Controllers
{
    public class WorkScheduleController : Controller
    {
        private readonly IBarberService _barberService;
        private readonly IWorkScheduleService _workScheduleService;

        public WorkScheduleController(IWorkScheduleService workScheduleService, IBarberService barberService)
        {
            _workScheduleService = workScheduleService;
            _barberService = barberService;
        }
        public async Task<IActionResult> Index(WorkScheduleFilterViewModel? filter)
        {
            var list =  _workScheduleService.GetAll();
            var query = list.AsQueryable();
            if (filter.BarberId != null)
            {
                query = query.Include(b => b.BarberId == filter.BarberId);
            }
            var model = new WorkScheduleFilterViewModel
            {
                WorkSchedules = query.Include(b => b.Barber).ToList(),
                BarberId = filter.BarberId,
                Barbers = new SelectList(_barberService.GetAll(), "Id", "FirstName")
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new WorkScheduleCreateViewModel();
            var barbers = _barberService.GetAll();
            model.Barbers = barbers.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FirstName.ToString()}).ToList();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(WorkScheduleCreateViewModel schedule)
        {
            var workSchedule = new WorkSchedule
            {
                BarberId = schedule.BarberId,
                WorkDate = schedule.WorkDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            };
            await _workScheduleService.Add(workSchedule);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var workSchedule = await _workScheduleService.Get(w => w.Id == id);
            if (workSchedule == null)
            {
                return NotFound();
            }
            var barbers = _barberService.GetAll();
            var model = new WorkScheduleEditViewModel
            {
                Id = workSchedule.Id,
                BarberId = workSchedule.BarberId,
                WorkDate = workSchedule.WorkDate,
                StartTime = workSchedule.StartTime,
                EndTime = workSchedule.EndTime,
                Barbers = barbers.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FirstName + " " + b.LastName }).ToList()
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, WorkSchedule schedule)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }
            
            var model = new WorkSchedule
            {
                Id = schedule.Id,
                BarberId = schedule.BarberId,
                WorkDate = schedule.WorkDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            };
            await _workScheduleService.Update(model);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _workScheduleService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

