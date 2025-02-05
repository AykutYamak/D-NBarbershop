using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Services;
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
        public async Task<IActionResult> Index(ServiceViewModel? model)
        {
            var list = _serviceService.GetAll();
            var service = new ServiceViewModel
            {
                Id = model.Id,
                ServiceName = model.ServiceName,
                Services = list.ToList(),
                Description = model.Description,
                Price = model.Price,
                Duration = model.Duration,
                Feedbacks = model.Feedbacks,
                AppointmentServices = model.AppointmentServices
            };
            return View(service);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new ServiceCreateViewModel();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(ServiceCreateViewModel model,int DurationHours,int DurationMinutes)
        {
            model.Duration = new TimeSpan(DurationHours, DurationMinutes, 0);
            var service = new Service
            {
                ServiceName = model.ServiceName,
                Description = model.Description,
                Price = model.Price,
                Duration = model.Duration
            };
            await _serviceService.Add(service);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var service = await _serviceService.Get(s=>s.Id == id);
            if (service == null)
            {
                return NotFound();
            }
            var services = _serviceService.GetAll();
            var model = new ServiceEditViewModel
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price,
                Duration = service.Duration
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(ServiceEditViewModel service,int DurationHours,int DurationMinutes)
        {
            service.Duration = new TimeSpan(DurationHours, DurationMinutes, 0);
            var model = new Service
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price,
                Duration = service.Duration
            };
            await _serviceService.Update(model);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
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
