﻿using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Barbers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace DNBarbershop.Controllers
{

    public class BarberController : Controller
    {
        private readonly IBarberService _barberService;
        private readonly IFeedbackService _feedbackService;
        private readonly ISpecialityService _specialityService;
        private readonly UserManager<User> _userManager;
        public BarberController(UserManager<User> userManager,IFeedbackService feedbackService, IBarberService barberService, ISpecialityService specialityService)
        {
            _barberService = barberService;
            _specialityService = specialityService;
            _feedbackService = feedbackService;
            _userManager = userManager;
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
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            var model = new BarberCreateViewModel();
            var specialities = _specialityService.GetAll();
            if (!specialities.Any())
            {
                TempData["error"] = "Няма налични специалности.";
                return RedirectToAction("Index");
            }
            model.Specialities = specialities.Select(s => new SelectListItem { Value =s.Id.ToString(), Text = s.Type.ToString() }).ToList();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(BarberCreateViewModel barberModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            var barber = new Barber
            {
                FirstName = barberModel.FirstName,
                LastName = barberModel.LastName,
                ExperienceYears = barberModel.ExperienceYears,
                ProfilePictureUrl = barberModel.ProfilePictureUrl,
                SpecialityId = barberModel.SelectedSpecialityId 
            };
            if (string.IsNullOrEmpty(barber.FirstName) || string.IsNullOrEmpty(barber.LastName) || barber.ExperienceYears < 0 || barber.ExperienceYears > 30 || barber.SpecialityId.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                TempData["error"] = "Невалидни данни!";
                return RedirectToAction("Add", "Barber", null);
            }
            var barbers = _barberService.GetAll().ToList();
            foreach (var item in barbers)
            {
                if (barber.ProfilePictureUrl == item.ProfilePictureUrl)
                {
                    TempData["error"] = "Бръснар с такава снимка вече съществува!";
                    return RedirectToAction("Add", "Barber", null);
                }
            }

            await _barberService.Add(barber);
            TempData["success"] = "Упсешно добавен бръснар.";

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var barber = await _barberService.Get(b=>b.Id == id);
            if (barber == null)
            {
                TempData["error"] = "Няма такъв бръснар.";
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
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(BarberEditViewModel barberModel)
        {
            if (false)
            {
                var barber = await _barberService.Get(b => b.Id == barberModel.Id);
                if (barber == null)
                {
                    TempData["error"] = "Няма такъв бръснар.";
                    return NotFound();
                }
            }
            var model = new Barber
            {
                Id = barberModel.Id,
                FirstName = barberModel.FirstName,
                LastName = barberModel.LastName,
                ExperienceYears = barberModel.ExperienceYears,
                ProfilePictureUrl = barberModel.ProfilePictureUrl,
                SpecialityId = barberModel.SelectedSpecialityId
            };
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || model.ExperienceYears < 0 || model.ExperienceYears > 30 || model.SpecialityId.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                TempData["error"] = "Невалидни данни!";
                return RedirectToAction("Edit", "Barber", null);
            }
            var barbers = _barberService.GetAll().ToList();
            foreach (var item in barbers)
            {
                if (model.ProfilePictureUrl == item.ProfilePictureUrl)
                {
                    TempData["error"] = "Бръснар с такава снимка вече съществува!";
                    return RedirectToAction("Edit", "Barber", null);
                }
            }
            await _barberService.Update(model);
            TempData["success"] = "Упсешно редактиран бръснар.";
            return RedirectToAction("Index");

        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "Невалиден идентификатор.";
                return RedirectToAction("Index");
            }

            var barber = await _barberService.Get(b => b.Id == id);
            if (barber == null)
            {
                TempData["error"] = "Няма такъв бръснар.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _barberService.Delete(id);
                TempData["success"] = "Упсешно изтрит бръснар.";
                return RedirectToAction("Index");
            }
            return View();
        }
        //User View Actions
        public async Task<IActionResult> UserIndex(BarberFilterViewModel? filter)
        {
            var list = _barberService.GetAll();
            var query = list.AsQueryable();
            if (filter?.MinExperienceYears != null)
            {
                query = query.Where(b => b.ExperienceYears >= filter.MinExperienceYears);
            }
            if (filter?.SpecialityId != null)
            {
                query = query.Where(b => b.SpecialityId == filter.SpecialityId.Value);
            }
            var model = new BarberFilterViewModel
            {
                SpecialityId = filter?.SpecialityId,
                MinExperienceYears = filter?.MinExperienceYears,
                Specialities = new SelectList(_specialityService.GetAll(), "Id", "Type"),
                Barbers = query.Include(b => b.Speciality).ToList()
            };
            return View(model);
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var barber = await _barberService.Get(b => b.Id == id);
            if (barber == null)
            {
                return NotFound();
            }
            var feedbacks = _feedbackService.GetAll().Where(f=>f.BarberId == id).Include(f => f.User).ToList();
            var model = new SingleBarberViewModel
            {
                Id = barber.Id,
                FirstName = barber.FirstName,
                LastName = barber.LastName,
                SpecialityId = barber.SpecialityId,
                Speciality = _specialityService.Get(s => s.Id == barber.SpecialityId).Result.Type,
                ExperienceYears = barber.ExperienceYears,
                ProfilePictureUrl = barber.ProfilePictureUrl,
                Feedbacks = feedbacks.OrderByDescending(c=>c.FeedBackDate).ToList()

            };
            return View(model);
        }
    }
}
