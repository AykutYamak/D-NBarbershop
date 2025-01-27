using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.BarberRepository;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DNBarbershop.Controllers
{
    public class BarberController : Controller
    {
        IBarberService barberService;
        public BarberController(IBarberService _barberService)
        {
                barberService= _barberService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await barberService.GetAll();
            return View(list);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Barber barber)
        {
            if (ModelState.IsValid)
            {
                await barberService.Add(barber);
                return RedirectToAction("ListBarbers");
            }
            return View(barber);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var barber = barberService.Get(b=>b.Id == id);
            return View(barber);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Barber barber)
        {
            if (ModelState.IsValid)
            {
                await barberService.Update(barber.Id, barber);
                return RedirectToAction("Index");
            }
            return View(barber);
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
