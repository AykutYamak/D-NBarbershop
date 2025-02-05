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
        IBarberService _barberService;
        public SpecialityController(IBarberService barberService,ISpecialityService specialityService)
        {
            _specialityService = specialityService;
            _barberService = barberService;
        }

        //Admin View Actions
        public async Task<IActionResult> Index(SpecialityViewModel? model)
        {
            var query =  _specialityService.GetAll();
            var speciality = new SpecialityViewModel
            {
                Id = model.Id,
                Type = model.Type,
                Specialities = query.ToList(),
                Barbers = model.Barbers
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
        [HttpPost]
        public async Task<IActionResult> Add(SpecialityCreateViewModel specialityModel)
        {
            var speciality = new Speciality
            {
                Type = specialityModel.Type
            };
            await _specialityService.Add(speciality);
            return RedirectToAction("Index");
            
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            Speciality speciality = await _specialityService.Get(s => s.Id == id);
            if (speciality == null)
            {
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
        [HttpPost]
        public async Task<IActionResult> Edit(SpecialityEditViewModel specialityModel)
        {
            var model = new Speciality
            {
                Id = specialityModel.Id,
                Type = specialityModel.Type
            };
            await _specialityService.Update(model);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _specialityService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
        //User View Actions

    }
}
