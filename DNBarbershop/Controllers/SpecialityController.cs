using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
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
            if (ModelState.IsValid)
            {
                await specialityService.Add(speciality);
                return RedirectToAction("Index");
            }
            return View(speciality);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var speciality = await specialityService.Get(b => b.Id == id);
            return View(speciality);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Speciality speciality)
        {
            if (ModelState.IsValid)
            {
                await specialityService.Update(speciality.Id, speciality);
                return RedirectToAction("Index");
            }
            return View(speciality);
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
