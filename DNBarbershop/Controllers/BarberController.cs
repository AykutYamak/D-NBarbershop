using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.BarberRepository;
using DNBarbershop.Models;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DNBarbershop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BarberController : Controller
    {
        IBarberService barberService;
        ISpecialityService specialityService;
        public BarberController(IBarberService _barberService, ISpecialityService _specialityService)
        {
                barberService= _barberService;
                specialityService =  _specialityService;
        }
        public async Task<IActionResult> Index(BarberViewModel? filter)
        {
            var list = await barberService.GetAll();
            var query = list.AsQueryable();
            if (filter.MinExperienceYears!=null)
            {
                query = query.Where(b => b.ExperienceYears >= filter.MinExperienceYears);
            }
            if (filter.SpecialityId!=null)
            {
                query = query.Where(b => b.SpecialityId == filter.SpecialityId.Value);
            }



            var model = new BarberViewModel
            {
                SpecialityId = filter.SpecialityId,
                MinExperienceYears = filter.MinExperienceYears,
                Specialities = new SelectList(await specialityService.GetAll(), "Id", "Type"),
                Barbers = query.Include(b => b.Speciality).ToList()
            };

            return View(model);
        }
        public async Task<IActionResult> Add()
        {
            var specialities = await specialityService.GetAll();
            ViewBag.Specialities = new SelectList(specialities, "Id", "Type");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Barber barber)
        {
            //if (ModelState.IsValid)
            //{
                await barberService.Add(barber);
                return RedirectToAction("Index");
            //}
            //return View();
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            Barber barber = await barberService.Get(b=>b.Id == id);
            var specialities = await specialityService.GetAll();
            ViewBag.Specialities = new SelectList(specialities, "Id", "Type");
            return View(barber);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Barber barber)
        {
            //if (ModelState.IsValid)
            //{
                await barberService.Update(barber);
                return RedirectToAction("Index");
            //}
            //return View(barber);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await barberService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
