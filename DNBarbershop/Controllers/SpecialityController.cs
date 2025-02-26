using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Specialities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    [Authorize(Roles = "Admin")]

    public class SpecialityController : Controller
    {
        ISpecialityService _specialityService;
        public SpecialityController(IBarberService barberService,ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }

        //Admin View Actions
        public async Task<IActionResult> Index(SpecialityViewModel? model)
        {
            var query =  _specialityService.GetAll();
            var speciality = new SpecialityViewModel
            {
                Id = model?.Id ?? Guid.Empty,
                Type = model?.Type ?? string.Empty,
                Specialities = query.ToList(),
                Barbers = model?.Barbers
            };
            return View(speciality);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            var model = new SpecialityCreateViewModel();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(SpecialityCreateViewModel specialityModel)
        {
            var existingSpeciality = await _specialityService.Get(s => s.Type == specialityModel.Type);
            if (existingSpeciality != null)
            {
                TempData["error"] = "Ниво на специализиране с този тип вече съществува.";
                return RedirectToAction("Index");
            }

            var speciality = new Speciality
            {
                Type = specialityModel.Type
            };
            await _specialityService.Add(speciality);
            TempData["success"] = "Успешно добавено ниво на специализиране.";
            return RedirectToAction("Index");
            
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "Невалиден идентификатор на специализация.";
                return RedirectToAction("Index");
            }

            var speciality = await _specialityService.Get(s => s.Id == id);
            if (speciality == null)
            {
                TempData["error"] = "Няма намерено такова ниво на специализиране.";
                return NotFound();
            }

            var model = new SpecialityEditViewModel
            {
                Id = speciality.Id,
                Type = speciality.Type
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(SpecialityEditViewModel specialityModel)
        {
            if (false)
            {
                var speciality = await _specialityService.Get(s => s.Id == specialityModel.Id);
                if (speciality == null)
                {
                    TempData["error"] = "Няма намерено такова ниво на специализиране.";
                    return NotFound();
                }

                var duplicate = await _specialityService.Get(s => s.Type == specialityModel.Type && s.Id != specialityModel.Id);
                if (duplicate != null)
                {
                    TempData["error"] = "Ниво на специализиране с този тип вече съществува.";
                    return RedirectToAction("Index");
                }
            }

            var model = new Speciality
            {
                Id = specialityModel.Id,
                Type = specialityModel.Type
            };
            await _specialityService.Update(model);
            TempData["success"] = "Успешно редактирано ниво на специализиране.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "Невалиден идентификатор на специализация.";
                return RedirectToAction("Index");
            }

            var speciality = await _specialityService.Get(s => s.Id == id);
            if (speciality == null)
            {
                TempData["error"] = "Няма намерено такова ниво на специализиране.";
                return NotFound();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    await _specialityService.Delete(id);
                    TempData["success"] = "Успешно изтрито ниво на специализиране.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Възникна грешка при изтриването на нивото на специализиране.";
            }
            return RedirectToAction("Index");
        }
        //User View Actions

    }
}
