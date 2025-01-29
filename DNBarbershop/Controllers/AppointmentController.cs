using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNBarbershop.Controllers
{
    public class AppointmentController : Controller
    {
        IAppointmentService _appointmentService;
        IBarberService _barberService;
        public AppointmentController(IAppointmentService appointmentService,IBarberService barberService)
        {
            _appointmentService = appointmentService;
            _barberService = barberService;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _appointmentService.GetAll();
            return View(list);
        }
        public async Task<IActionResult> Add()
        {
            var barbers = await _barberService.GetAll();
            ViewBag.Barbers = new SelectList(barbers, "Id", "FirstName" + " " + "LastName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Appointment appointment)
        {
            await _appointmentService.Add(appointment);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            Appointment appointment = await _appointmentService.Get(a => a.Id == id);
            return View();
        }
    }
}
