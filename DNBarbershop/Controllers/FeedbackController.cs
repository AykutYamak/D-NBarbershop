using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using DNBarbershop.Models.ViewModels.Appointments;
using DNBarbershop.Models.ViewModels.Barbers;
using DNBarbershop.Models.ViewModels.Feedbacks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IBarberService _barberService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public FeedbackController(UserManager<User> userManager,IFeedbackService feedbackService, IBarberService barberService, IUserService userService)
        {
            _feedbackService = feedbackService;
            _barberService = barberService;
            _userService = userService;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index(FeedbackFilterViewModel? filter)
        {
            var list = _feedbackService.GetAll();
            var query = list.AsQueryable();
            if (filter.BarberId != null)
            {
                query = query.Where(b => b.BarberId == filter.BarberId);
            }

            var users = _userService.GetAll();
            var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();

            var barbers = _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

            var feedbacks = query.Include(u => u.User).Include(b => b.Barber).ToList();

            var model = new FeedbackFilterViewModel
            {
                BarberId = filter.BarberId,
                Barbers = new SelectList(barbersList, "Id", "FullName"),
                Feedbacks = feedbacks
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

            var model = new FeedbackCreateViewModel();

            var barbers = _barberService.GetAll();
            model.Barbers = barbers.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FirstName.ToString() + " " + b.LastName.ToString()}).ToList();
            return View(model);
        }
        [Authorize(Roles="Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(FeedbackCreateViewModel feedbackModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/a.";
                return Unauthorized();
            }

            feedbackModel.UserId = currentUser.Id;

            if (feedbackModel.UserId != currentUser.Id)
            {
                return BadRequest();
            }
            else
            {
                var feedback = new Feedback
                {
                    Id = Guid.NewGuid(),
                    UserId = currentUser.Id,
                    BarberId = feedbackModel.SelectedBarberId,
                    Rating = feedbackModel.Rating,
                    Comment = feedbackModel.Comment,
                    FeedBackDate = feedbackModel.FeedBackDate
                };
                await _feedbackService.Add(feedback);
                TempData["success"] = "Успешно добавен отзив.";
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/a.";
                return Unauthorized();
            }

            var feedback = await _feedbackService.Get(f => f.Id == id);
            if (feedback == null)
            {
                TempData["error"] = "Не е намерен такъв отзив.";
                return NotFound();
            }
            var barbers = _barberService.GetAll();
            var model = new FeedbackEditViewModel
            {
                Id = feedback.Id,
                UserId = currentUser.Id,
                Barbers = barbers.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FirstName.ToString() + " " + b.LastName.ToString() }).ToList(),
                SelectedBarberId = feedback.BarberId,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                FeedBackDate = DateTime.UtcNow
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(FeedbackEditViewModel feedbackModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/a.";
                return Unauthorized();
            }

            var model = new Feedback
            {
                Id = feedbackModel.Id,
                UserId = currentUser.Id,
                BarberId = feedbackModel.SelectedBarberId,
                Rating = feedbackModel.Rating,
                Comment = feedbackModel.Comment,
                FeedBackDate = DateTime.UtcNow
            };
            await _feedbackService.Update(model);
            TempData["success"] = "Успешно редактиран отзив.";

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            if (ModelState.IsValid) 
            {
                await _feedbackService.Delete(Id);
                TempData["success"] = "Успешно изтрит отзив";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}

