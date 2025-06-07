using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
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
                if (_barberService.GetAll().Any(b => b.Id == filter.BarberId))
                {
                    query = query.Where(b => b.BarberId == filter.BarberId);
                }
                else
                {
                    TempData["error"] = "Невалиден бръснар.";
                }
            }

            var users = _userService.GetAll();
            var usersList = users.Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName }).ToList();

            var barbers = _barberService.GetAll();
            var barbersList = barbers.Select(b => new { b.Id, FullName = b.FirstName + " " + b.LastName }).ToList();

            var feedbacks = query
                            .Include(f => f.User)
                            .Include(f => f.Barber)
                            .OrderByDescending(f => f.FeedBackDate)
                            .ToList();
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
            
            if (!barbers.Any())
            {
                TempData["error"] = "Няма налични бръснари.";
                return RedirectToAction("Index");
            }
            
            model.Barbers = barbers.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.FirstName.ToString() + " " + b.LastName.ToString()}).ToList();
            return View(model);
        }
        [Authorize(Roles="Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(FeedbackCreateViewModel feedbackModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/a.";
                return Unauthorized();
            }

            if (!_barberService.GetAll().Any(b => b.Id == feedbackModel.BarberId))
            {
                TempData["error"]= "Избраният бръснар не съществува.";
                return View(feedbackModel);
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
                    BarberId = feedbackModel.BarberId,
                    Rating = feedbackModel.Rating,
                    Comment = feedbackModel.Comment,
                    FeedBackDate = feedbackModel.FeedBackDate
                };
                await _feedbackService.Add(feedback);
                TempData["success"] = "Успешно добавен коментар.";
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
                TempData["error"] = "Не е намерен такъв коментар.";
                return NotFound();
            }

            if (feedback.UserId != currentUser.Id)
            {
                TempData["error"] = "Нямате право да редактирате този коментар.";
                return Forbid();
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
        [ValidateAntiForgeryToken]
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
            TempData["success"] = "Успешно редактиран коментар.";

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var feedback = await _feedbackService.Get(f => f.Id == Id);
            if (feedback == null)
            {
                TempData["error"] = "Отзивът не съществува.";
                return NotFound();
            }

            if (ModelState.IsValid) 
            {
                await _feedbackService.Delete(Id);
                TempData["success"] = "Успешно изтрит коментар";
                return RedirectToAction("Index");
            }
            return View();
        }
        //User Actions
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddComment()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "Не сте регистриран/а.";
                return Unauthorized();
            }
            var model = new FeedbackCreateViewModel();
            return View(model);
        }
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddComment(FeedbackCreateViewModel feedbackModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["error"] = "За да добавите коментар трябва да се регистрирате.";
                return RedirectToAction("Details", "Barber", new { id = feedbackModel.BarberId });
            }

            if (!_barberService.GetAll().Any(b => b.Id == feedbackModel.BarberId))
            {
                TempData["error"] = "Избраният бръснар не съществува.";
                return View(feedbackModel);
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
                    UserId = feedbackModel.UserId,
                    BarberId = feedbackModel.BarberId,
                    Rating = feedbackModel.Rating,
                    Comment = feedbackModel.Comment,
                    FeedBackDate = feedbackModel.FeedBackDate
                };
                if (feedback.Comment == null || string.IsNullOrEmpty(feedback.Comment))
                {
                    TempData["error"] = "Невалиден коментар.";
                    return RedirectToAction("Details", "Barber", new { id = feedbackModel.BarberId });
                }
                await _feedbackService.Add(feedback);
                TempData["success"] = "Успешно добавен коментар.";
            }
            return RedirectToAction("Details","Barber", new {id=feedbackModel.BarberId});
        }
    }
}

