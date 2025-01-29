using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    [Authorize(Roles = "Admin")]

    public class SpecialityController : Controller
    {
        ISpecialityService specialityService;
        public SpecialityController(ISpecialityService _specialityService)
        {
            specialityService = _specialityService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await specialityService.GetAll();
            return View(list);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Speciality speciality)
        {
            //if (ModelState.IsValid)
            //{
                await specialityService.Add(speciality);
                return RedirectToAction("Index");
            //}
            //return View(speciality);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var speciality = await specialityService.Get(b => b.Id == id);
            if (speciality == null)
            {
                return NotFound();
            }
            return View(speciality);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id,Speciality speciality)
        {
            if (id!=speciality.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var existingSpeciality = await specialityService.Get(s => s.Id == id);
                if (existingSpeciality == null)
                {
                    return NotFound();
                }
                existingSpeciality.Type = speciality.Type;
                
                await specialityService.Update(existingSpeciality);
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await specialityService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
