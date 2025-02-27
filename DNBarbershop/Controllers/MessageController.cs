using DNBarbershop.Core.IServices;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DNBarbershop.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly UserManager<User> _userManager;

        public MessageController(UserManager<User> userManager, IMessageService messageService)
        {
            _messageService = messageService;
            _userManager = userManager;
        }
        public IActionResult Index(MessageViewModel? model)
        {
            var list = _messageService.GetAll();
            var query = list.AsQueryable(); 
            var message = new MessageViewModel
            {
                Id = model.Id,
                Email = model.Email,
                Content = model.Content,
                Date = model.Date,
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Messages= query.ToList(),
                IsRead = query.Where(a=>a.Id == model.Id).Select(a=>a.IsRead).FirstOrDefault()
            };
            return View(message);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new MessageCreateViewModel();
            return View(model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(MessageCreateViewModel messageModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser==null)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Email = messageModel.Email,
                    UserId = null,
                    Content = messageModel.Content,
                    Date = DateTime.UtcNow,
                    IsRead = false
                };
                await _messageService.Add(message);
                TempData["success"] = "Успешно изпратено съобщение.";
            }
            else
            {
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Email = messageModel.Email,
                    UserId = currentUser.Id,
                    Content = messageModel.Content,
                    Date = DateTime.UtcNow,
                    IsRead = false
                };
                await _messageService.Add(message);
                TempData["success"] = "Успешно изпратено съобщение.";
            }
            return RedirectToAction("AboutUs","Home",null);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var message = await _messageService.Get(f => f.Id == id);
            if (message == null)
            {
                TempData["error"] = "Съобщението не съществува.";
                return NotFound();
            }
            message.IsRead = true;
            await _messageService.Update(message);
            TempData["success"] = "Съобщението е маркирано като прочетено.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(MessageEditViewModel messageModel)
        {
          
            var model = new Message
            {
                Id = messageModel.Id,
                UserId = messageModel.UserId,
                Content = messageModel.Content,
                Email = messageModel.Email,
                Date = messageModel.Date
            };
            await _messageService.Update(model);
            TempData["success"] = "Успешно редактирано съобщение.";

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var feedback = await _messageService.Get(f => f.Id == Id);
            if (feedback == null)
            {
                TempData["error"] = "Съобщението не съществува.";
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _messageService.Delete(Id);
                TempData["success"] = "Успешно изтрито съобщение.";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
