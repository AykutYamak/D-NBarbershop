using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _serviceService.GetAll();
            return View(list);
        }
        public async Task<IActionResult> Add()
        {
            //var services = await _serviceService.GetAll();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Service service,int DurationHours,int DurationMinutes)
        {
            //if (ModelState.IsValid)
            //{
                service.Duration = new TimeSpan(DurationHours, DurationMinutes, 0);
                await _serviceService.Add(service);
                return RedirectToAction("Index");
            //}
            //return View(service);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var service = await _serviceService.Get(s=>s.Id == id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id,Service service,int DurationHours,int DurationMinutes)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                    var existingService = await _serviceService.Get(s=>s.Id==id);
                    if (existingService == null)
                    {
                        return NotFound();
                    }

                    existingService.ServiceName = service.ServiceName;
                    existingService.Description = service.Description;
                    existingService.Price = service.Price;
                    existingService.Duration = new TimeSpan(DurationHours, DurationMinutes, 0);

                    await _serviceService.Update(existingService);
                    return RedirectToAction("Index");
            }

            return View(service);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _serviceService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
