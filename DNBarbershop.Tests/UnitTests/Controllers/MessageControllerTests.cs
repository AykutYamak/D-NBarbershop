using System;
using System.Linq;
using System.Threading.Tasks;
using DNBarbershop.Controllers;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using DNBarbershop.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq.Expressions;
using System.Security.Claims;

namespace DNBarbershop.Tests.UnitTests.Controllers
{
    [TestFixture]
    public class MessageControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;
        private ApplicationDbContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IMessageService> _messageServiceMock;
        private MessageController _messageController;

        [SetUp]
        public void Setup()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DNBarbershopInMemoryDb" + Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(_dbOptions);
            _context.Database.EnsureCreated();
            DbSeeder.SeedDatabase(_context);

            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            _messageServiceMock = new Mock<IMessageService>();

            _messageServiceMock.Setup(x => x.GetAll())
                .Returns(_context.messages);

            _messageController = new MessageController(
                _userManagerMock.Object,
                _messageServiceMock.Object
            );

            var httpContext = new DefaultHttpContext();
            _messageController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _messageController.TempData = new Mock<ITempDataDictionary>().Object;
        }

        [TearDown]
        public void TearDown()
        {
            _messageController.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void Index_WithNoFilter_ReturnsAllMessages()
        {
            var messageModel = new MessageViewModel();

            var result = _messageController.Index(messageModel) as ViewResult;
            var model = result?.Model as MessageViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Messages.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task Add_ValidMessage_AddsSuccessfully()
        {
            var user = new User { Id = Guid.NewGuid().ToString(), Email = "test@example.com" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var messageModel = new MessageCreateViewModel
            {
                Email = "test@example.com",
                Content = "Test message content"
            };

            var result = await _messageController.Add(messageModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("AboutUs"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
            _messageServiceMock.Verify(x => x.Add(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public async Task Add_InvalidEmail_ReturnsErrorRedirect()
        {
            var user = new User { Id = Guid.NewGuid().ToString(), Email = "test@example.com" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var messageModel = new MessageCreateViewModel
            {
                Email = "invalid-email",
                Content = "Test message content"
            };

            var result = await _messageController.Add(messageModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("AboutUs"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
            _messageServiceMock.Verify(x => x.Add(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public async Task Add_EmptyContent_ReturnsErrorRedirect()
        {
            var user = new User { Id = Guid.NewGuid().ToString(), Email = "test@example.com" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var messageModel = new MessageCreateViewModel
            {
                Email = "test@example.com",
                Content = ""
            };

            var result = await _messageController.Add(messageModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("AboutUs"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
            _messageServiceMock.Verify(x => x.Add(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public async Task MarkAsRead_ExistingMessage_MarksSuccessfully()
        {
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId, IsRead = false };

            _messageServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Message, bool>>>()))
                .ReturnsAsync(message);

            var result = await _messageController.MarkAsRead(messageId) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _messageServiceMock.Verify(x => x.Update(It.Is<Message>(m => m.IsRead == true)), Times.Once);
        }

        [Test]
        public async Task MarkAsRead_NonExistentMessage_ReturnsNotFound()
        {
            var messageId = Guid.NewGuid();
            _messageServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Message, bool>>>()))
                .ReturnsAsync((Message)null);

            var result = await _messageController.MarkAsRead(messageId) as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            _messageServiceMock.Verify(x => x.Update(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public async Task Edit_ValidMessage_UpdatesSuccessfully()
        {
            var messageModel = new MessageEditViewModel
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                Content = "Updated message content",
                Email = "updated@example.com",
                Date = DateTime.UtcNow
            };

            var result = await _messageController.Edit(messageModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _messageServiceMock.Verify(x => x.Update(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public async Task Delete_ExistingMessage_DeletesSuccessfully()
        {
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId };

            _messageServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Message, bool>>>()))
                .ReturnsAsync(message);

            var result = await _messageController.Delete(messageId) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _messageServiceMock.Verify(x => x.Delete(messageId), Times.Once);
        }

        [Test]
        public async Task Delete_NonExistentMessage_ReturnsNotFound()
        {
            var messageId = Guid.NewGuid();
            _messageServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Message, bool>>>()))
                .ReturnsAsync((Message)null);

            var result = await _messageController.Delete(messageId) as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            _messageServiceMock.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
        }
    }
}