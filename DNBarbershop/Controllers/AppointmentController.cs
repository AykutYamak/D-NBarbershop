using AspNetCoreHero.ToastNotification.Abstractions;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using DNBarbershop.Models.ViewModels.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly IAppointmentServiceService _appointmentServiceService;   
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberService _barberService;
        private readonly IServiceService _serviceService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        public AppointmentController(INotyfService notyf, 
            UserManager<User> userManager, 
            IAppointmentService appointmentService,
            IBarberService barberService, 
            IServiceService serviceService,
            IUserService userService,
            IAppointmentServiceService appointmentServiceService)
        {
            _appointmentService = appointmentService;
            _barberService = barberService;
            _serviceService = serviceService;
            _userService = userService;
            _userManager = userManager;
            _notyf = notyf;
            _appointmentServiceService = appointmentServiceService;
        }
        //Admin View Actions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(AppointmentFilterViewModel? filter)
        {
            var list = _appointmentService.GetAll();
            var query = list.AsQueryable();
            if (filter.UserId != null)
            {
                query = query.Where(a => a.UserId == filter.UserId);
            }
            if (filter.BarberId != null)
            {
                query = query.Where(a => a.BarberId == filter.BarberId);
            }

            var barbers = _barberService.GetAll();
            var barbersList = barbers.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();
            var users = _userService.GetAll();
            var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();
            var model = new AppointmentFilterViewModel
            {
                UserId = filter.UserId,
                BarberId = filter.BarberId,
                Barbers = new SelectList(barbersList, "Id", "FullName"),
                Users = new SelectList(usersList,"Id","FullName"),
                Appointments = query.Include(a => a.User).Include(a => a.Barber).ToList()
            };
            return View(model);
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
        [Authorize]
        public async Task<IActionResult> Add()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var model = new AppointmentCreateViewModel
            {
                UserId = currentUser.Id,
                Status = AppointmentStatus.Scheduled
            };

            var barbers = _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

            var services = _serviceService.GetAll();

            ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
            ViewBag.Services = new SelectList(services, "Id", "ServiceName");

            var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
            ViewBag.TimeSlots = timeSlots.Select(ts => new SelectListItem
            {
                Value = ts,
                Text = ts
            }).ToList();

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AppointmentCreateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            model.UserId = currentUser.Id;
            if (!ModelState.IsValid)
            {
                _notyf.Error("Моля попълнете всички полета.");
                var barbers = _barberService.GetAll();
                var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

                var services = _serviceService.GetAll();

                ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
                ViewBag.Services = new SelectList(services, "Id", "ServiceName");

                var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                ViewBag.TimeSlots = timeSlots.Select(ts => new SelectListItem
                {
                    Value = ts,
                    Text = ts
                }).ToList();
                return View(model);
            }

            var appointments = _appointmentService.GetAll();
            bool isAlreadyBooked = appointments.Any(a => a.AppointmentDate == model.AppointmentDate && a.AppointmentTime == model.AppointmentTime); 
            
            if (isAlreadyBooked)
            {
                _notyf.Error("Този час е вече запазен. Моля изберете друг час.");

                var barbers = _barberService.GetAll();
                var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

                var services = _serviceService.GetAll();

                ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
                ViewBag.Services = new SelectList(services, "Id", "ServiceName");

                var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                ViewBag.TimeSlots = timeSlots.Select(ts => new SelectListItem
                {
                    Value = ts,
                    Text = ts
                }).ToList();
                return View(model);
            }

            var newAppointment = new Appointment
            {
                Id = Guid.NewGuid(),
                BarberId = model.BarberId,
                UserId = model.UserId,
                AppointmentDate = model.AppointmentDate,
                AppointmentTime = model.AppointmentTime,
                Status = AppointmentStatus.Scheduled
            };
            
            await _appointmentService.Add(newAppointment);
            _notyf.Success("Успешно записахте час.");

            foreach (var serviceId in model.SelectedServiceIds)
            {
                var appointmentService = new AppointmentServices
                {
                    AppointmentId = newAppointment.Id,
                    ServiceId = serviceId
                };
                await _appointmentServiceService.Add(appointmentService);
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var appointment = await _appointmentService.Get(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            var barbers = _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

            var services = _serviceService.GetAll();

            var users = _userService.GetAll();
            var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();

            ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
            ViewBag.Services = new SelectList(services, "Id", "ServiceName");
            ViewBag.Users = new SelectList(usersList, "Id", "FullName");

            var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
            ViewBag.TimeSlots = new SelectList(timeSlots, "AppointmentTime");
            ViewBag.timeSlotList = timeSlots;

            return View(appointment);
        }
        [Authorize(Roles = "Admin")]
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
                ViewBag.Error = "Този час е вече запазен. Моля изберете друг час.";

                var barbers = _barberService.GetAll();
                var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

                var services = _serviceService.GetAll();
                
                var users = _userService.GetAll();
                var usersList = users.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();


                ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
                ViewBag.Services = new SelectList(services, "Id", "ServiceName");
                ViewBag.Users = new SelectList(usersList, "Id", "FullName");

                var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                ViewBag.TimeSlots = new SelectList(timeSlots, "AppointmentTime");
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
        [Authorize(Roles = "Admin")]
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
