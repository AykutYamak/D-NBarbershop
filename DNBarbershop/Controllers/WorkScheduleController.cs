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
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        public async Task<bool> IsDayOfWeekAlreadyScheduled(Guid barberId, DayOfWeek dayOfWeek)
        {
            var schedules = await _workScheduleService.GetAll().Where(ws => ws.BarberId == barberId).ToListAsync();
            var exists = schedules.Any(ws => ws.DayOfWeek == dayOfWeek);
            return exists;
        }

        public async Task<IActionResult> Index(WorkScheduleFilterViewModel? filter)
        {
            var list =  _workScheduleService.GetAll();
            var query = list.AsQueryable();
            if (filter?.BarberId != null)
            {
                query = query.Where(b => b.BarberId == filter.BarberId);
            }
            query = query.Include(b => b.Barber);

            var model = new WorkScheduleFilterViewModel
            {
                WorkSchedules = query.ToList(),
                BarberId = filter.BarberId,
                Barbers = new SelectList(_barberService.GetAll(), "Id","LastName")
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new WorkScheduleCreateViewModel();
            var barbers = _barberService.GetAll();
            model.Barbers = barbers.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FirstName.ToString() + " " + b.LastName.ToString()}).ToList();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(WorkScheduleCreateViewModel schedule)
        {
            if (await IsDayOfWeekAlreadyScheduled(schedule.BarberId, schedule.DayOfWeek))
            {
                TempData["error"] = "Този ден е вече създаден.";
                return RedirectToAction("Index");
            }

            if (schedule.StartTime >= schedule.EndTime)
            {
                TempData["error"] = "Началният час трябва да бъде по-рано от крайния.";
                return RedirectToAction("Index");
            }

            var workSchedule = new WorkSchedule
            {
                BarberId = schedule.BarberId,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            };
            await _workScheduleService.Add(workSchedule);
            TempData["success"] = "Успешно добавен ден от седмицата към графика.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var workSchedule = await _workScheduleService.Get(w => w.Id == id);
            if (workSchedule == null)
            {
                TempData["error"] = "Няма такъв график.";
                return NotFound();
            }
            var barbers = _barberService.GetAll();
            var model = new WorkScheduleEditViewModel
            {
                Id = workSchedule.Id,
                BarberId = workSchedule.BarberId,
                DayOfWeek = workSchedule.DayOfWeek,
                StartTime = workSchedule.StartTime,
                EndTime = workSchedule.EndTime,
                Barbers = barbers.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FirstName + " " + b.LastName }).ToList()
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, WorkScheduleEditViewModel schedule)
        {
            if (id != schedule.Id)
            {
                TempData["error"] = "Не е намерен такъв график.";
                return NotFound();
            }

            if(schedule.StartTime >= schedule.EndTime)
            {
                TempData["error"] = "Началният час трябва да бъде по-рано от крайния.";
                return RedirectToAction("Index");
            }


            var model = new WorkSchedule
            {
                Id = schedule.Id,
                BarberId = schedule.BarberId,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            };
            await _workScheduleService.Update(model);
            TempData["success"] = "Успешно редактиран график.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var workSchedule = await _workScheduleService.Get(w => w.Id == id);
            if (workSchedule == null)
            {
                TempData["error"] = "Графикът не съществува.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _workScheduleService.Delete(id);
                TempData["success"] = "Успешно изтрит график.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}

