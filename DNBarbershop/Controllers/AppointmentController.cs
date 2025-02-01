using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
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
        
        private async Task<List<string>> GenerateTimeSlots(TimeSpan start, TimeSpan end, TimeSpan interval)
        {
            var slots = new List<string>();
            for (var time = start; time < end; time += interval)
            {
                slots.Add(time.ToString(@"hh\:mm"));
            }
            return slots;
        }
        public async Task<IActionResult> Add()
        {
            var barbers = await _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

            var services = await _serviceService.GetAll();
            var servicesList = services.Select(s => new { s.Id, s.ServiceName }).ToList();

            var users = await _userService.GetAll();
            var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();

            ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
            ViewBag.Services = new SelectList(servicesList, "Id", "ServiceName");
            ViewBag.Users = new SelectList(usersList, "Id", "FullName");
            var timeSlots = GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
            ViewBag.TimeSlots = new SelectList(await timeSlots ?? new List<string>());
            ViewBag.timeSlotList = await timeSlots;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Appointment appointment,DateTime AppointmentDate, TimeSpan AppointmentTime)
        {
            var appointments = await _appointmentService.GetAll();
            bool isAlreadyBooked = appointments.Any(a => a.AppointmentDate == AppointmentDate && a.AppointmentTime == AppointmentTime);

            if (isAlreadyBooked)
            {
                ViewBag.Error = "This time slot is already booked. Please choose another time.";
                ViewBag.TimeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                var timeSlots = GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                ViewBag.timeSlotList = await timeSlots;
                return View();
            }
            var existingBarber = await _barberService.Get(b=>b.Id == appointment.BarberId);
            var existingUser = await _userService.Get(u => u.Id == appointment.UserId);
            var newAppointment = new Appointment
            {
                UserId = appointment.UserId,
                User = existingUser,
                BarberId = appointment.BarberId,
                Barber = existingBarber,
                AppointmentServices = appointment.AppointmentServices,
                Status = AppointmentStatus.Scheduled,
                AppointmentDate = AppointmentDate,
                AppointmentTime = AppointmentTime
            };

            await _appointmentService.Add(newAppointment);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var appointment = await _appointmentService.Get(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            var barbers = await _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();
            var services = await _serviceService.GetAll();
            var servicesList = services.Select(s => new { s.Id, s.ServiceName}).ToList();
            var users = await _userService.GetAll();
            var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();

            ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName", appointment.BarberId);
            ViewBag.Services = new SelectList(servicesList, "Id", "ServiceName");
            ViewBag.Users = new SelectList(usersList, "Id", "FullName", appointment.UserId);

            var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
            ViewBag.TimeSlots = new SelectList(timeSlots, appointment.AppointmentTime);
            ViewBag.timeSlotList = timeSlots;

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Appointment appointment, DateTime AppointmentDate, TimeSpan AppointmentTime)
        {
            var existingAppointment = await _appointmentService.Get(a => a.Id == appointment.Id);
            if (existingAppointment == null)
            {
                return NotFound();
            }
            var Appointment = await _appointmentService.Get(a => a.AppointmentDate == AppointmentDate && a.AppointmentTime == AppointmentTime && a.Id != appointment.Id); 

            if (Appointment != null)
            {
                ViewBag.Error = "This time slot is already booked. Please choose another time.";

                var barbers = await _barberService.GetAll();
                var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();
                var services = await _serviceService.GetAll();
                var servicesList = services.Select(s => new { s.Id, s.ServiceName}).ToList();
                var users = await _userService.GetAll();
                var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();

                ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName", appointment.BarberId);
                ViewBag.Services = new MultiSelectList(servicesList, "Id", "Name", appointment.AppointmentServices?.Select(a => a.ServiceId));
                ViewBag.Users = new SelectList(usersList, "Id", "FullName", appointment.UserId);

                var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                ViewBag.TimeSlots = new SelectList(timeSlots, appointment.AppointmentTime);
                ViewBag.timeSlotList = timeSlots;

                return View(appointment);
            }

            existingAppointment.UserId = appointment.UserId;
            existingAppointment.BarberId = appointment.BarberId;
            existingAppointment.AppointmentServices = appointment.AppointmentServices;
            existingAppointment.AppointmentDate = AppointmentDate;
            existingAppointment.AppointmentTime = AppointmentTime;
            existingAppointment.Status = appointment.Status;

            await _appointmentService.Update(existingAppointment);
            return RedirectToAction("Index");
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
