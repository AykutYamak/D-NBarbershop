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
        private async Task<List<string>> GenerateTimeSlots(TimeSpan start, TimeSpan end, TimeSpan interval)
        {
            var slots = new List<string>();
            for (var time = start; time < end; time += interval)
            {
                slots.Add(time.ToString(@"hh\:mm"));
            }
            return slots;
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

            var appointments = query
            .Include(a => a.User)
            .Include(a => a.Barber)
            .Include(a => a.AppointmentServices) 
            .ThenInclude(ap => ap.Service)  
            .ToList();

            var model = new AppointmentFilterViewModel
            {
                UserId = filter.UserId,
                BarberId = filter.BarberId,
                Barbers = new SelectList(barbersList, "Id", "FullName"),
                Users = new SelectList(usersList,"Id","FullName"),
                Appointments = appointments
            };
            return View(model);
        }
        
        [Authorize(Roles ="Admin")]
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
            ViewBag.Services = services.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.ServiceName
            }).ToList();

            var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
            ViewBag.TimeSlots = timeSlots.Select(ts => new SelectListItem
            {
                Value = ts,
                Text = ts
            }).ToList();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AppointmentCreateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            model.UserId = currentUser.Id;

            if (model.UserId != currentUser.Id)
            {
                return BadRequest();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    _notyf.Error("Моля попълнете всички полета.");
                    var barbers = _barberService.GetAll();
                    var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

                    var services = _serviceService.GetAll();

                    ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
                    ViewBag.Services = services.Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.ServiceName
                    }).ToList();
                    var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                    ViewBag.TimeSlots = timeSlots.Select(ts => new SelectListItem
                    {
                        Value = ts,
                        Text = ts
                    }).ToList();
                    return View(model);
                }

                var appointments = _appointmentService.GetAll();
                bool isAlreadyBooked = _appointmentService.GetAll().Any(a => a.BarberId == model.BarberId && a.AppointmentDate == model.AppointmentDate && a.AppointmentTime == model.AppointmentTime);
                if (isAlreadyBooked)
                {
                    _notyf.Error("Този час е вече запазен. Моля изберете друг час.");

                    var barbers = _barberService.GetAll();
                    var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

                    var services = _serviceService.GetAll();

                    ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
                    ViewBag.Services = services.Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.ServiceName
                    }).ToList();
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

                if (model.SelectedServiceIds == null || !model.SelectedServiceIds.Any())
                {
                    _notyf.Error("Моля, изберете поне една услуга.");
                    return View(model);
                }

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                    if (!serviceExists)
                    {
                        _notyf.Error($"Услугата с ID {serviceId} не съществува.");
                        return View(model);
                    }

                    var appointmentService = new AppointmentServices
                    {
                        AppointmentId = newAppointment.Id,
                        ServiceId = serviceId
                    };
                    await _appointmentServiceService.Add(appointmentService);
                }
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var appointment = await _appointmentService.Get(a => a.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            
            if (currentUser == null)
            {
                return Unauthorized();
            }

            appointment.UserId = currentUser.Id;

            if (appointment.UserId != currentUser.Id)
            {
                return Unauthorized();
            }

            var barbers = _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

            var services = _serviceService.GetAll();

            ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
            ViewBag.Services = services.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.ServiceName
            }).ToList();
            var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
            ViewBag.TimeSlots = timeSlots.Select(ts => new SelectListItem
            {
            Value = ts,
            Text = ts
            }).ToList();

            var model = new AppointmentEditViewModel()
            {
                Id = appointment.Id,
                UserId = appointment.UserId,
                BarberId = appointment.BarberId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Status = AppointmentStatus.Scheduled,
                SelectedServiceIds = appointment.AppointmentServices.Select(asv => asv.ServiceId).ToList()
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentEditViewModel model)
       {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            model.UserId = currentUser.Id;

            if (model.UserId != currentUser.Id)
            {
                return BadRequest();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    _notyf.Error("Моля попълнете всички полета.");
                    var barbers = _barberService.GetAll();
                    var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

                    var services = _serviceService.GetAll();

                    ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
                    ViewBag.Services = services.Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.ServiceName
                    }).ToList();
                    var timeSlots = await GenerateTimeSlots(TimeSpan.FromHours(9), TimeSpan.FromHours(18), TimeSpan.FromMinutes(30));
                    ViewBag.TimeSlots = timeSlots.Select(ts => new SelectListItem
                    {
                        Value = ts,
                        Text = ts
                    }).ToList();
                    return View(model);
                }

                var appointments = _appointmentService.GetAll();
                bool isAlreadyBooked = _appointmentService.GetAll().Any(a => a.BarberId == model.BarberId && a.AppointmentDate == model.AppointmentDate && a.AppointmentTime == model.AppointmentTime);
                if (isAlreadyBooked)
                {
                    _notyf.Error("Този час е вече запазен. Моля изберете друг час.");

                    var barbers = _barberService.GetAll();
                    var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

                    var services = _serviceService.GetAll();

                    ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
                    ViewBag.Services = services.Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.ServiceName
                    }).ToList();
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
                    Id = model.Id,
                    BarberId = model.BarberId,
                    UserId = model.UserId,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    Status = AppointmentStatus.Scheduled,
                };

                if (model.SelectedServiceIds == null || !model.SelectedServiceIds.Any())
                {
                    _notyf.Error("Моля, изберете поне една услуга.");
                    return View(model);
                }

                await _appointmentServiceService.DeleteByAppointmentId(newAppointment.Id);

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                    if (!serviceExists)
                    {
                        _notyf.Error($"Услугата с ID {serviceId} не съществува.");
                        return View(model);
                    }
                    var appointmentServiceEntity = new AppointmentServices
                    {
                        AppointmentId = newAppointment.Id,
                        ServiceId = serviceId,
                    };
                    await _appointmentServiceService.Add(appointmentServiceEntity);
                }

                _notyf.Success("Успешно редактирахте час.");

                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id,Guid serviceId)
        {
            if(ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    await _appointmentServiceService.Delete(id, serviceId);
                    await _appointmentService.Delete(id);
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
