using System.Linq;
using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using DNBarbershop.Models.ViewModels.Appointments;
using DNBarbershop.Models.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    
    public class UserController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberService _barberService;
        private readonly IFeedbackService _feedbackService;
        private readonly IMessageService _messageService;
        public UserController(IMessageService messageService,IFeedbackService feedbackService, IAppointmentService appointmentService, IBarberService barberService, IUserService userService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _userService = userService;
            _roleManager = roleManager;
            _userManager = userManager;
            _appointmentService = appointmentService;
            _barberService = barberService;
            _feedbackService = feedbackService;
            _messageService = messageService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<(User User, List<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add((user, roles.ToList()));
            }

            return View(userRoles);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["error"] = "Не е намерен такъв потребител.";
                return NotFound();
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("User"))
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    TempData["success"] = $"Успешно направихте {user.FirstName} {user.LastName} админ!";
                }
                if (roles.Contains("User"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "User");
                }
                return RedirectToAction("Index");

            }
            return BadRequest("Failed to assign admin role");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> MakeUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["error"] = "Не е намерен такъв потребител.";
                return NotFound();
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {

                var result = await _userManager.AddToRoleAsync(user, "User");

                if (result.Succeeded)
                {
                    TempData["success"] = $"Успешно направихте {user.FirstName} {user.LastName} потребител!";
                }
                if (roles.Contains("Admin"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                return RedirectToAction("Index");

            }
            return BadRequest("Failed to assign user role");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                List<Feedback> userFeedbacks = _feedbackService.GetAll().ToList();
                foreach (var item in userFeedbacks)
                {
                    if (item.UserId == id.ToString())
                    {
                        await _feedbackService.Delete(item.Id);
                    }
                }
                List<Message> userMessages = _messageService.GetAll().ToList();
                foreach (var item in userMessages)
                {
                    if (item.UserId == id.ToString())
                    {
                        await _messageService.Delete(item.Id);
                    }
                }
                await _userService.DeleteStringId(id);
                TempData["success"] = "Успешно изтрит потребител.";
                return RedirectToAction("Index");
            }
            return View();
        }
        //User's View
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(UserViewModel? user)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            var appointments = _appointmentService
                .GetAll()
                .Where(a => a.UserId == currentUser.Id)
                .Include(a => a.Barber) 
                .Include(a => a.AppointmentServices)
                    .ThenInclude(s => s.Service)
                .ToList();

            var sortedAppointments = appointments
                .OrderBy(a => GetStatusOrder(a.Status))
                .ThenBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToList();
            foreach (var item in appointments)
            {
                item.Status = GetEffectiveStatus(item);
            }
            var model = new UserViewModel
            {
                Id = currentUser.Id,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                Appointments = sortedAppointments
            };  

            return View(model);
        }
        int GetStatusOrder(AppointmentStatus status)
        {
            return status switch
            {
                AppointmentStatus.Scheduled => 0,
                AppointmentStatus.Cancelled => 1,
                AppointmentStatus.Completed => 2
            };
        }

        private AppointmentStatus GetEffectiveStatus(Appointment appointment)
        {
            if (appointment.Status == AppointmentStatus.Cancelled)
                return AppointmentStatus.Cancelled;

            var dateTime = appointment.AppointmentDate.Date + appointment.AppointmentTime;
            if (dateTime <= DateTime.Now)
                return AppointmentStatus.Completed;

            return AppointmentStatus.Scheduled;
        }
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var model = new UserEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
            return View(model);
        }
        [Authorize(Roles = "User,Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel userModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["error"] = "Потребителят не е намерен.";
                return NotFound();
            }
         
            currentUser.FirstName = userModel.FirstName;
            currentUser.LastName = userModel.LastName;
            currentUser.PhoneNumber = userModel.PhoneNumber;

            var result = await _userManager.UpdateAsync(currentUser);

            if (result.Succeeded)
            {
                TempData["success"] = "Успешно редактирахте профила си.";
                return RedirectToAction("Details");
            }

            return View(userModel);
        }

    }
}
