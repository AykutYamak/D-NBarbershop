using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    public class ServiceController : Controller
    {
        ServiceService serviceService;
        public ServiceController(ServiceService _serviceService)
        {
            serviceService= _serviceService;
        }
        public IActionResult Index()
        {
            var list = serviceService.GetAll();
            return View(list);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Service service)
        {
            if (ModelState.IsValid)
            {
                await serviceService.Add(service);
                return RedirectToAction("ListBarbers");
            }
            return View(service);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var service = serviceService.Get(b => b.Id == id);
            return View(service);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                await serviceService.Update(service.Id, service);
                return RedirectToAction("Index");
            }
            return View(service);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await serviceService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
