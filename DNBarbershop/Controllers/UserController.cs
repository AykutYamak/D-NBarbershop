using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var list = _userService.GetAll();
            return View(list);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userService.Get(s => s.Id.Equals(id));
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, User user)
        {
            if (!id.Equals(user.Id))
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _userService.Get(s => s.Id.Equals(id));
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.FirstName= user.FirstName;
                existingUser.LastName = user.LastName;
                await _userService.Update(existingUser);
                return RedirectToAction("Index");
            }

            return View(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                await _userService.DeleteStringId(id);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
