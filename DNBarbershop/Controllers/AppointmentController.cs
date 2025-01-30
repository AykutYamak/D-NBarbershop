using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    public class AppointmentController : Controller
    {
        IAppointmentService _appointmentService;
        IBarberService _barberService;
        IServiceService _serviceService;
        IUserService _userService;
        public AppointmentController(IAppointmentService appointmentService,IBarberService barberService, IServiceService serviceService,IUserService userService)
        {
            _appointmentService = appointmentService;
            _barberService = barberService;
            _serviceService = serviceService;
            _userService = userService;
        }   
        public async Task<IActionResult> Index()
        {
            
            var list = await _appointmentService.GetAll();
            return View(list);
        }
        private List<string> GenerateTimeSlots(TimeSpan start, TimeSpan end, TimeSpan interval)
        {
            var slots = new List<string>();
            for (var time = start; time < end; time += interval)
            {
                slots.Add(time.ToString(@"hh\:mm")); // Formats as "09:00", "09:30", etc.
            }
            return slots;
        }
        public async Task<IActionResult> Add()
        {
            var barbers = await _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();
            var services = await _serviceService.GetAll();
            var users = await _userService.GetAll();
            var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();
            ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
            ViewBag.Services = new SelectList(services, "Id", "ServiceName");
            ViewBag.Users = new SelectList(usersList, "Id", "FullName");
            var timeSlots = GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
            ViewBag.TimeSlots = new SelectList(timeSlots ?? new List<string>());
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Appointment appointment,DateTime selectedDate, TimeSpan selectedTime)
        {
            var appointments = await _appointmentService.GetAll();
            bool isAlreadyBooked = appointments.Any(a => a.AppointmentDate== selectedDate && a.AppointmentTime == selectedTime);

            if (isAlreadyBooked)
            {
                ViewBag.Error = "This time slot is already booked. Please choose another time.";
                ViewBag.TimeSlots = GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                return View();
            }

            var newAppointment = new Appointment
            {
                AppointmentDate = selectedDate,
                AppointmentTime = selectedTime
            };


            await _appointmentService.Add(newAppointment);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            Appointment appointment = await _appointmentService.Get(b => b.Id == id);
           
            
            return View(appointment);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Appointment appointment)
        {
            //if (ModelState.IsValid)
            //{
            await _appointmentService.Update(appointment);
            return RedirectToAction("Index");
            //}
            //return View(barber);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if(ModelState.IsValid)
            {
                await _appointmentService.Delete(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
