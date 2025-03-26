using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using DNBarbershop.Models.ViewModels.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace DNBarbershop.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentServiceService _appointmentServiceService;
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberService _barberService;
        private readonly IServiceService _serviceService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        public AppointmentController(
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
            _appointmentServiceService = appointmentServiceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(Guid barberId, DateTime appointmentDate, int totalDurationMinutes)
        {
            var startTime = TimeSpan.FromHours(8);
            var endTime = TimeSpan.FromHours(20,30);
            var interval = TimeSpan.FromMinutes(30);
            
            if (appointmentDate.DayOfWeek == DayOfWeek.Saturday)
            {
                startTime = TimeSpan.FromHours(10); 
                endTime = TimeSpan.FromHours(15,30);       
            }
            if (appointmentDate.Date == DateTime.Today.Date)
            {
                TimeSpan buffer = TimeSpan.FromMinutes(30);
                TimeSpan currentTime = DateTime.Now.TimeOfDay.Add(buffer);

                int totalMinutes = (int)currentTime.TotalMinutes;   
                int roundedMinutes = ((totalMinutes + 29) / 30) * 30; 
                TimeSpan earliestStart = TimeSpan.FromMinutes(roundedMinutes);

                startTime = earliestStart > startTime ? earliestStart : startTime;
            }

            var allSlots = new List<string>();
            if (startTime < endTime)
            {
                allSlots = await _appointmentService.GenerateTimeSlots(startTime, endTime, interval);
            }

            var existingAppointments = await _appointmentService
            .GetAll()
            .AsQueryable()
            .Where(a => a.BarberId == barberId && a.AppointmentDate == appointmentDate)
            .Include(a => a.AppointmentServices)
            .ThenInclude(ap => ap.Service)
            .ToListAsync();

            var bookedRanges = existingAppointments.Select(a =>
            {
                var duration = a.AppointmentServices.Sum(s => s.Service.Duration.Hours * 60 + s.Service.Duration.Minutes);
                var end = a.AppointmentTime.Add(TimeSpan.FromMinutes(duration));
                return (Start: a.AppointmentTime, End: end);
            }).ToList();

            var availableSlots = new List<string>();
            foreach (var slot in allSlots)
            {
                var slotStart = TimeSpan.Parse(slot);
                var slotEnd = slotStart.Add(TimeSpan.FromMinutes(totalDurationMinutes));

                if (slotEnd > endTime) continue;

                bool isConflict = bookedRanges.Any(range => slotStart < range.End && slotEnd > range.Start);
                if (!isConflict) availableSlots.Add(slot);
            }

            return Json(availableSlots);
        }
        private async Task PopulateViewBags()
        {
            var barbers = _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();
            var services = _serviceService.GetAll();

            ViewBag.Barbers = new SelectList(barbersList, "Id", "FullName");
            ViewBag.Services = services.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.ServiceName
            }).ToList();

            var allSlots = await _appointmentService.GenerateTimeSlots(TimeSpan.FromHours(8), TimeSpan.FromHours(20, 30, 0, 0, 0), TimeSpan.FromMinutes(30));

            var bookedSlots = _appointmentService
                .GetAll()
                .Where(a => a.AppointmentDate == DateTime.Today) 
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm"))
                .ToList();

            var availableSlots = allSlots.Except(bookedSlots).ToList();

            ViewBag.TimeSlots = availableSlots.Select(ts => new SelectListItem
            {
                Value = ts.ToString(),
                Text = ts.ToString()
            }).ToList();

            ViewBag.Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = AppointmentStatus.Scheduled.ToString()},
                new SelectListItem { Value = "2", Text = AppointmentStatus.Completed.ToString()},
                new SelectListItem { Value = "3", Text = AppointmentStatus.Cancelled.ToString()}
            };
        }
        //Admin View Actions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(AppointmentFilterViewModel? filter)
        {
            var list = _appointmentService.GetAll().OrderByDescending(x=>x.AppointmentDate);
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
            foreach (var item in appointments)
            {   
                if (item.AppointmentDate.Date <= DateTime.Now.Date && item.AppointmentTime < DateTime.Now.TimeOfDay && item.Status != AppointmentStatus.Cancelled)
                {
                    item.Status = AppointmentStatus.Completed;
                }
                else if (item.Status==AppointmentStatus.Cancelled)
                {
                    item.Status = AppointmentStatus.Cancelled;
                }
                else
                {
                    item.Status = AppointmentStatus.Scheduled;
                }
            }
            var model = new AppointmentFilterViewModel
            {
                UserId = filter.UserId,
                BarberId = filter.BarberId,
                Barbers = new SelectList(barbersList, "Id", "FullName"),
                Users = new SelectList(usersList, "Id", "FullName"),
                Appointments = appointments
            };
           
            return View(model);
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

            var model = new AppointmentCreateViewModel
            {
                UserId = currentUser.Id,
                Status = AppointmentStatus.Scheduled,
                Services = _serviceService.GetAll().ToList(),
                Barbers = _barberService.GetAll().ToList()
            };

            await PopulateViewBags();
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AppointmentCreateViewModel model)
        {

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            model.UserId = currentUser.Id;

            if (model.UserId != currentUser.Id)
            {
                return BadRequest();
            }
            else
            {
                int totalDurationMinutes = 0;
                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var service = await _serviceService.Get(s => s.Id == serviceId);
                    totalDurationMinutes += service.Duration.Hours * 60 + service.Duration.Minutes;
                }

                var isAvailable = await _appointmentService.IsTimeSlotAvailable(model.BarberId, model.AppointmentDate, model.AppointmentTime, totalDurationMinutes);
                if (!isAvailable)
                {
                    TempData["error"] = "Този час не е свободен.";
                    await PopulateViewBags();
                    return RedirectToAction("Add", "Appointment", null);
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

                if (model.SelectedServiceIds == null || !model.SelectedServiceIds.Any())
                {
                    TempData["error"] = "Изберете поне една услуга!";
                    return RedirectToAction("Add", "Appointment", null);
                }
                if (model.AppointmentTime <= DateTime.Now.TimeOfDay && model.AppointmentDate <= DateTime.Now.Date)
                {
                    TempData["error"] = "Изберете валиден час!";
                    await PopulateViewBags();
                    return RedirectToAction("Add","Appointment",null);
                }

                await _appointmentService.Add(newAppointment);
                currentUser.Appointments.Add(newAppointment);

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                    if (!serviceExists)
                    {
                        TempData["error"] = "Няма такава услуга!";
                        return RedirectToAction("Add", "Appointment", null);
                    }

                    var appointmentService = new AppointmentServices
                    {
                        AppointmentId = newAppointment.Id,
                        ServiceId = serviceId
                    };
                    await _appointmentServiceService.Add(appointmentService);
                }
                TempData["success"] = "Успешно резервиран час!";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var appointment = await _appointmentService.GetWithRels(a => a.Id == id);
            if (appointment == null)
            {
                TempData["error"] = "Резервацията не е намерена.";
                return RedirectToAction("Index");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            appointment.UserId = currentUser.Id;



            if (appointment.UserId != currentUser.Id)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            await PopulateViewBags();

            var model = new AppointmentEditViewModel()
            {
                Id = appointment.Id,
                UserId = appointment.UserId,
                BarberId = appointment.BarberId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Status = appointment.Status,
                Services = _serviceService.GetAll().ToList(),
                SelectedServiceIds = appointment.AppointmentServices.Select(asv => asv.ServiceId).ToList()
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentEditViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            model.UserId = currentUser.Id;

            var appointment = await _appointmentService.GetWithRels(a => a.Id == model.Id);
            if (appointment == null)
            {
                TempData["error"] = "Резервацията не е намерена.";
                return RedirectToAction("Index");
            }
            if (model.UserId != currentUser.Id)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return BadRequest();
            }
            else
            {
                int totalDurationMinutes = 0;
                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var service = await _serviceService.Get(s => s.Id == serviceId);
                    totalDurationMinutes += service.Duration.Hours * 60 + service.Duration.Minutes;
                }

                var isAvailable = await _appointmentService.IsTimeSlotAvailable(model.BarberId, model.AppointmentDate, model.AppointmentTime, totalDurationMinutes);
                if (!isAvailable)
                {
                    TempData["error"] = "Този час не е свободен.";
                    await PopulateViewBags();
                    return RedirectToAction("Edit", "Appointment", null);
                }
               
                var newAppointment = new Appointment
                {
                    Id = model.Id,
                    BarberId = appointment.BarberId,
                    UserId = model.UserId,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    Status = model.Status,
                };

                var existingAppointments = _appointmentService.GetAll().ToList();

                foreach (var item in existingAppointments)
                {
                    if (newAppointment.AppointmentDate == item.AppointmentDate && newAppointment.AppointmentTime == item.AppointmentTime && newAppointment.BarberId == item.BarberId && item.Id!=newAppointment.Id)
                    {
                        TempData["error"] = "Този час вече е зает!";
                        return RedirectToAction("Edit", "Appointment", null);
                    }
                }
                await _appointmentService.Update(newAppointment);

                await _appointmentServiceService.DeleteByAppointmentId(newAppointment.Id);

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                    if (!serviceExists)
                    {
                        TempData["error"] = "Няма такава услуга.";
                        return RedirectToAction("Edit", "Appointment", null);
                    }
                    var appointmentServiceEntity = new AppointmentServices
                    {
                        AppointmentId = newAppointment.Id,
                        ServiceId = serviceId,
                    };
                    await _appointmentServiceService.Add(appointmentServiceEntity);
                }
                TempData["success"] = "Успешно редактирана резервация.";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _appointmentServiceService.DeleteByAppointmentId(id);
                await _appointmentService.Delete(id);
                TempData["success"] = "Упсешно изтрита резервация.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //User's View
        [Authorize(Roles = "User,Admin")]

        public async Task<IActionResult> MakeAppointment()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            var model = new AppointmentCreateViewModel
            {
                UserId = currentUser.Id,
                Status = AppointmentStatus.Scheduled,
                Services = _serviceService.GetAll().ToList(),
                Barbers = _barberService.GetAll().ToList()
            };

            await PopulateViewBags();
            return View(model);
        }
        [Authorize(Roles = "User,Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> MakeAppointment(AppointmentCreateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            model.UserId = currentUser.Id;

            if (model.UserId != currentUser.Id)
            {
                return BadRequest();
            }
            else
            {
                // Calculate total duration
                int totalDurationMinutes = 0;
                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var service = await _serviceService.Get(s => s.Id == serviceId);
                    totalDurationMinutes += service.Duration.Hours * 60 + service.Duration.Minutes;
                }

                var isAvailable = await _appointmentService.IsTimeSlotAvailable(model.BarberId, model.AppointmentDate, model.AppointmentTime, totalDurationMinutes);
                if (!isAvailable)
                {
                    TempData["error"] = "Този час не е свободен.";
                    await PopulateViewBags();
                    return RedirectToAction("MakeAppointment", "Appointment", null);
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

                if (model.SelectedServiceIds == null || !model.SelectedServiceIds.Any())
                {
                    TempData["error"] = "Няма такава услуга!";
                    await PopulateViewBags();
                    return RedirectToAction("MakeAppointment", "Appointment", null);
                }
                if (model.AppointmentDate <= DateTime.Now.Date && model.AppointmentTime <= DateTime.Now.TimeOfDay)
                {
                    TempData["error"] = "Изберете валиден час!";
                    await PopulateViewBags();
                    return RedirectToAction("MakeAppointment", "Appointment", null);
                }

                await _appointmentService.Add(newAppointment);
                currentUser.Appointments.Add(newAppointment);

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                    if (!serviceExists)
                    {
                        TempData["error"] = "Няма такава услуга!";
                        await PopulateViewBags();
                        return RedirectToAction("MakeAppointment", "Appointment", null);
                    }

                    var appointmentService = new AppointmentServices
                    {
                        AppointmentId = newAppointment.Id,
                        ServiceId = serviceId
                    };
                    await _appointmentServiceService.Add(appointmentService);
                }
                TempData["success"] = "Успешно резервиран час!";
                return RedirectToAction("Details", "User", null);
            }
        }
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UserAppointmentDelete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _appointmentServiceService.DeleteByAppointmentId(id);
                await _appointmentService.Delete(id);
                TempData["success"] = "Упсешно изтрита резервация.";
            }
            return RedirectToAction("Details", "User", null);
        }

    }
}
