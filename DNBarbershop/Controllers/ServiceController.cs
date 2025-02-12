using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;

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
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(ServiceCreateViewModel model,int DurationHours,int DurationMinutes)
        {
            if ((DurationHours < 0 || DurationMinutes < 0 || DurationHours == 0 && DurationMinutes == 0))
            {
                TempData["error"] = "Продължителността трябва да бъде по-голяма от 0 минути.";
                return View(model); 
            }
            model.Duration = new TimeSpan(DurationHours, DurationMinutes, 0);
            var service = new Service
            {
                ServiceName = model.ServiceName,
                Description = model.Description,
                Price = model.Price,
                Duration = model.Duration
            };
            var existingService = await _serviceService.Get(s=>s.Id == model.Id);
            if (existingService!=null)
            {
                TempData["error"] = "Услуга с това име вече съществува.";
                return View(model);
            }
            await _serviceService.Add(service);
            TempData["success"] = "Успешно добавена услуга.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var service = await _serviceService.Get(s=>s.Id == id);
            if (service == null)
            {
                TempData["error"] = "Не е намерена такава услуга.";
                return NotFound();
            }
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
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(ServiceEditViewModel service,int DurationHours,int DurationMinutes)
        {
            if (DurationHours < 0 || DurationMinutes < 0 || (DurationHours == 0 && DurationMinutes == 0))
            {
                TempData["error"] = "Продължителността трябва да бъде по-голяма от 0 минути.";
                return View(service);
            }

            var serviceWithSameName = await _serviceService.Get(s => s.ServiceName == service.ServiceName && s.Id != service.Id);
            if (serviceWithSameName != null)
            {
                TempData["error"] = "Услуга с това име вече съществува.";
                return View(service);
            }

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
            TempData["success"] = "Успешно редактирана услуга.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["error"] = "Невалиден идентификатор на услугата.";
                return RedirectToAction("Index");
            }
            var service = await _serviceService.Get(s => s.Id == id);
            if (service == null)
            {
                TempData["error"] = "Не е намерена такава услуга.";
                return RedirectToAction("Index");
            }
            try
            {
                await _serviceService.Delete(id);
                TempData["success"] = "Успешно изтрита услуга.";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Възникна грешка при изтриването на услугата.";
            }
            return RedirectToAction("Index");
        }
    }
}
