using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.BarberRepository;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DNBarbershop.Controllers
{
    public class BarberController : Controller
    {
        IBarberService barberService;
        ISpecialityService specialityService;
        public BarberController(IBarberService _barberService, ISpecialityService _specialityService)
        {
                barberService= _barberService;
                specialityService =  _specialityService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await barberService.GetAll();
            return View(list);
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
