using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly UserManager<User> _userManager;
        public ServiceController(IServiceService serviceService, UserManager<User> userManager)
        {
            _serviceService = serviceService;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]

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
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            var model = new ServiceCreateViewModel();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(ServiceCreateViewModel model,int DurationHours,int DurationMinutes)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(model.ServiceName) || string.IsNullOrEmpty(model.Description) || model.Price == 0)
            {
                TempData["error"] = "Невалидни данни!";
                return RedirectToAction("Add", "Service", null);
            }
            if (DurationMinutes!=15 && DurationMinutes!=30 && DurationMinutes!=0)
            {
                TempData["error"] = "Продължителността на услугата в минути трябва да бъде или 0 или 15 или 30";
                return RedirectToAction("Add", "Service", null);
            }

            if (DurationHours == 0 && DurationMinutes == 0)
            {
                TempData["error"] = "Невалидна продължителност.";
                return RedirectToAction("Add", "Service", null);
            }
           

            model.Duration = new TimeSpan(DurationHours, DurationMinutes, 0);
            var service = new Service
            {
                ServiceName = model.ServiceName,
                Description = model.Description,
                Price = model.Price,
                Duration = model.Duration
            };

            var existingServices = _serviceService.GetAll().ToList();
            foreach (var item in existingServices)
            {
                if (item.ServiceName.ToLower() == service.ServiceName.ToLower() || item.Description.ToLower() == service.Description.ToLower())
                {
                    TempData["error"] = "Такава услуга вече съществува!";
                    return RedirectToAction("Add", "Service", null);
                }
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
            if (string.IsNullOrEmpty(service.ServiceName) || string.IsNullOrEmpty(service.Description) || service.Price == 0)
            {
                TempData["error"] = "Невалидни данни!";
                return RedirectToAction("Edit", "Service", null);
            }
            if (DurationMinutes != 15 && DurationMinutes != 30 && DurationMinutes != 0)
            {
                TempData["error"] = "Продължителността на услугата в минути трябва да бъде или 0 или 15 или 30";
                return RedirectToAction("Edit", "Service", null);
            }

            if (DurationHours == 0 && DurationMinutes == 0)
            {
                TempData["error"] = "Невалидна продължителност.";
                return RedirectToAction("Edit", "Service", null);
            }
            

            var serviceWithSameName = await _serviceService.Get(s => s.ServiceName.ToLower() == service.ServiceName.ToLower() && s.Id != service.Id);
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

        //User View Action
        public async Task<IActionResult> ServiceDetails(ServiceFilterViewModel? filter) 
        {
            var list = _serviceService.GetAll();
            var query = list.AsQueryable();
            if (filter.MaxPrice != null)
            {
                query = query.Where(b => b.Price <= filter.MaxPrice);
            }
            var model = new ServiceFilterViewModel
            {
                MaxPrice = filter.MaxPrice,
                Services = query.ToList()
            };
            return View(model);
        } 

    }
}
