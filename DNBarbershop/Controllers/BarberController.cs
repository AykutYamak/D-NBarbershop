using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.DataAccess.BarberRepository;
using DNBarbershop.Models;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Barbers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DNBarbershop.Controllers
{

    public class BarberController : Controller
    {
        private readonly IBarberService _barberService;
        private readonly ISpecialityService _specialityService;
        public BarberController(IBarberService barberService, ISpecialityService specialityService)
        {
            _barberService = barberService;
            _specialityService = specialityService;
        }
        //Admin View Actions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(BarberFilterViewModel? filter)
        {
            var list = _barberService.GetAll();
            var query = list.AsQueryable();
            if (filter.MinExperienceYears!=null)
            {
                query = query.Where(b => b.ExperienceYears >= filter.MinExperienceYears);
            }
            if (filter.SpecialityId!=null)
            {
                query = query.Where(b => b.SpecialityId == filter.SpecialityId.Value);
            }
            var model = new BarberFilterViewModel
            {
                SpecialityId = filter.SpecialityId,
                MinExperienceYears = filter.MinExperienceYears,
                Specialities = new SelectList(_specialityService.GetAll(), "Id", "Type"),
                Barbers = query.Include(b => b.Speciality).ToList() 
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new BarberCreateViewModel();
            var specialities = _specialityService.GetAll();
            model.Specialities = specialities.Select(s => new SelectListItem { Value =s.Id.ToString(), Text = s.Type.ToString() }).ToList();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(BarberCreateViewModel barberModel)
        {
            var barber = new Barber
            {
                FirstName = barberModel.FirstName,
                LastName = barberModel.LastName,
                ExperienceYears = barberModel.ExperienceYears,
                ProfilePictureUrl = barberModel.ProfilePictureUrl,
                SpecialityId = barberModel.SelectedSpecialityId 
            };
            await _barberService.Add(barber);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var barber = await _barberService.Get(b=>b.Id == id);
            if (barber == null)
            {
                return NotFound();
            }
            var specialities = _specialityService.GetAll();
            var model = new BarberEditViewModel
            {
                Id = barber.Id,
                FirstName = barber.FirstName,
                LastName = barber.LastName,
                ExperienceYears = barber.ExperienceYears,
                ProfilePictureUrl = barber.ProfilePictureUrl,
                Specialities = specialities.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Type.ToString() }).ToList(),
                SelectedSpecialityId = barber.SpecialityId
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(BarberEditViewModel barberModel)
        {
            var model = new Barber
            {
                Id = barberModel.Id,
                FirstName = barberModel.FirstName,
                LastName = barberModel.LastName,
                ExperienceYears = barberModel.ExperienceYears,
                ProfilePictureUrl = barberModel.ProfilePictureUrl,
                SpecialityId = barberModel.SelectedSpecialityId
            };
            await _barberService.Update(model);
            return RedirectToAction("Index");

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _barberService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
        //User View Actions
        public async Task<IActionResult> UserIndex(BarberFilterViewModel? filter)
        {
            var list = _barberService.GetAll();
            var query = list.AsQueryable();
            if (filter.MinExperienceYears != null)
            {
                query = query.Where(b => b.ExperienceYears >= filter.MinExperienceYears);
            }
            if (filter.SpecialityId != null)
            {
                query = query.Where(b => b.SpecialityId == filter.SpecialityId.Value);
            }
            var model = new BarberFilterViewModel
            {
                SpecialityId = filter.SpecialityId,
                MinExperienceYears = filter.MinExperienceYears,
                Specialities = new SelectList(_specialityService.GetAll(), "Id", "Type"),
                Barbers = query.Include(b => b.Speciality).ToList()
            };
            return View(model);
        }
    }
}
