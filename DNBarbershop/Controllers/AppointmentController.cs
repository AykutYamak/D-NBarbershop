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
        private async Task<bool> IsTimeSlotAvailable(Guid barberId, DateTime date, TimeSpan startTime, int durationMinutes)
        {
            var endTime = startTime.Add(TimeSpan.FromMinutes(durationMinutes));

            var existingAppointments = await _appointmentService.GetAll()
                .Where(a => a.BarberId == barberId && a.AppointmentDate == date)
                .Include(a => a.AppointmentServices)
                .ThenInclude(ap => ap.Service)
                .ToListAsync();

            foreach (var appointment in existingAppointments)
            {
                var appointmentDuration = appointment.AppointmentServices.Sum(s => s.Service.Duration.Hours * 60 + s.Service.Duration.Minutes);
                var appointmentEnd = appointment.AppointmentTime.Add(TimeSpan.FromMinutes(appointmentDuration));

                if (startTime < appointmentEnd && endTime > appointment.AppointmentTime)
                    return false;
            }

            return endTime <= TimeSpan.FromHours(20);
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

            var allSlots = await GenerateTimeSlots(startTime, endTime, interval);

            var existingAppointments = await _appointmentService.GetAll()
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

                if (slotEnd >= endTime)
                {
                    continue;
                }

                bool isConflict = bookedRanges.Any(range => slotStart < range.End && slotEnd > range.Start);

                if (!isConflict)
                {
                    availableSlots.Add(slot);
                }
            }

            return Json(availableSlots);
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

            var allSlots = await GenerateTimeSlots(TimeSpan.FromHours(8), TimeSpan.FromHours(20, 30, 0, 0, 0), TimeSpan.FromMinutes(30));

            var bookedSlots = _appointmentService
                .GetAll()
                .Where(a => a.AppointmentDate == DateTime.Today) // Adjust this to use the selected date
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm"))
                .ToList();

            var availableSlots = allSlots.Except(bookedSlots).ToList();

            ViewBag.TimeSlots = availableSlots.Select(ts => new SelectListItem
            {
                Value = ts,
                Text = ts
            }).ToList();

            var statuses = Enum.GetValues(typeof(AppointmentStatus));
            ViewBag.Statuses = statuses;
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
                Users = new SelectList(usersList, "Id", "FullName"),
                Appointments = appointments
            };
            foreach (var item in appointments)
            {
                if (item.AppointmentDate.Date < DateTime.Now.Date && item.AppointmentTime < DateTime.Now.TimeOfDay && item.Status != AppointmentStatus.Canceled)
                {
                    item.Status = AppointmentStatus.Completed;
                }
                else
                {
                    item.Status = AppointmentStatus.Scheduled;
                }
            }
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
                Status = AppointmentStatus.Scheduled
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
                //if (!ModelState.IsValid)
                //{

                //    await PopulateViewBags();
                //    TempData["error"] = "Неуспешно премината валидация.";

                //    return RedirectToAction("Index");
                //}

                var appointments = _appointmentService.GetAll();
                bool isAlreadyBooked = _appointmentService.GetAll().Any(a => a.BarberId == model.BarberId && a.AppointmentDate == model.AppointmentDate && a.AppointmentTime == model.AppointmentTime);
                if (isAlreadyBooked)
                {

                    await PopulateViewBags();
                    TempData["error"] = "Този час е вече резервиран.";
                    return RedirectToAction("Index");
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
                TempData["success"] = "Успешно резервиран час!";

                if (model.SelectedServiceIds == null || !model.SelectedServiceIds.Any())
                {
                    TempData["error"] = "Няма такава услуга!";
                    return RedirectToAction("Index");
                }

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                    if (!serviceExists)
                    {
                        TempData["error"] = "Няма такава услуга!";
                        return RedirectToAction("Index");
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

            if (model.UserId != currentUser.Id)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateViewBags();
                TempData["error"] = "Неуспешно премината валидация.";
                return View(model);
            }

            var appointment = await _appointmentService.GetWithRels(a => a.Id == model.Id);
            if (appointment == null)
            {
                TempData["error"] = "Резервацията не е намерена.";
                return RedirectToAction("Index");
            }
            var newAppointment = new Appointment
            {
                Id = model.Id,
                BarberId = appointment.BarberId,
                UserId = appointment.UserId,
                AppointmentDate = model.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Status = model.Status,
            };

            await _appointmentService.Update(newAppointment);
            TempData["success"] = "Успешно редактирана резервация.";

            //if (model.SelectedServiceIds == null || !model.SelectedServiceIds.Any())
            //{
            //    TempData["error"] = "Моля, изберете поне една услуга.";
            //    await PopulateViewBags();
            //    return View(model);
            //}

            await _appointmentServiceService.DeleteByAppointmentId(newAppointment.Id);

            foreach (var serviceId in model.SelectedServiceIds)
            {
                var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                if (!serviceExists)
                {
                    TempData["error"] = "Няма такава услуга.";
                    return RedirectToAction("Index");
                }
                var appointmentServiceEntity = new AppointmentServices
                {
                    AppointmentId = newAppointment.Id,
                    ServiceId = serviceId,
                };
                await _appointmentServiceService.Add(appointmentServiceEntity);
            }

            return RedirectToAction("Index");
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
                return Unauthorized();
            }

            model.UserId = currentUser.Id;

            // Calculate total duration
            int totalDurationMinutes = 0;
            foreach (var serviceId in model.SelectedServiceIds)
            {
                var service = await _serviceService.Get(s => s.Id == serviceId);
                totalDurationMinutes += service.Duration.Hours * 60 + service.Duration.Minutes;
            }

            var isAvailable = await IsTimeSlotAvailable(model.BarberId, model.AppointmentDate, model.AppointmentTime, totalDurationMinutes);
            if (!isAvailable)
            {
                TempData["error"] = "Този час не е свободен.";
                await PopulateViewBags();
                return RedirectToAction("MakeAppointment","Appointment",null);
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
                return RedirectToAction("Index");
            }



            await _appointmentService.Add(newAppointment);
            currentUser.Appointments.Add(newAppointment);

            foreach (var serviceId in model.SelectedServiceIds)
            {
                var serviceExists = _serviceService.GetAll().Any(s => s.Id == serviceId);
                if (!serviceExists)
                {
                    TempData["error"] = "Няма такава услуга!";
                    return RedirectToAction("Index");
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
