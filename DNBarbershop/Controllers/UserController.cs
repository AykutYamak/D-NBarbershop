using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    //
    public class UserController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberService _barberService;
        public UserController(IAppointmentService appointmentService,IBarberService barberService,IUserService userService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _userService = userService;
            _roleManager = roleManager;
            _userManager = userManager;
            _appointmentService = appointmentService;
            _barberService = barberService;
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

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (result.Succeeded)
            {
                TempData["success"] = $"Успешно направихте {user.FirstName} {user.LastName} админ!";
                return RedirectToAction("Index");
            }

            return BadRequest("Failed to assign admin role");
        }
       
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _userService.DeleteStringId(id);
                TempData["success"] = "Успешно изтрит потребител.";
                return RedirectToAction("Index");
            }
            return View();
        }
        //User's View
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(AppointmentFilterViewModel? filter)
        {

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }

            var model = new AppointmentFilterViewModel
            {
                UserId = currentUser.Id,
                Appointments = currentUser.Appointments.ToList(),
            };
            return View(model);
        }
    }
}
