using System;
using System.Linq;
using System.Threading.Tasks;
using DNBarbershop.Controllers;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Feedbacks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using DNBarbershop.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;

namespace DNBarbershop.Tests.UnitTests.Controllers
{
    [TestFixture]
    public class FeedbackControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;
        private ApplicationDbContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IFeedbackService> _feedbackServiceMock;
        private Mock<IBarberService> _barberServiceMock;
        private Mock<IUserService> _userServiceMock;
        private FeedbackController _feedbackController;

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

            _feedbackServiceMock = new Mock<IFeedbackService>();
            _barberServiceMock = new Mock<IBarberService>();
            _userServiceMock = new Mock<IUserService>();

            // Setup mocks with default returns based on context
            _feedbackServiceMock.Setup(x => x.GetAll())
                .Returns(_context.feedbacks);
            _barberServiceMock.Setup(x => x.GetAll())
                .Returns(_context.barbers);
            _userServiceMock.Setup(x => x.GetAll())
                .Returns(_context.Users);

            _feedbackServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Feedback, bool>>>()))
                .ReturnsAsync((Expression<Func<Feedback, bool>> predicate) =>
                    _context.feedbacks.FirstOrDefault(predicate));

            _feedbackController = new FeedbackController(
                _userManagerMock.Object,
                _feedbackServiceMock.Object,
                _barberServiceMock.Object,
                _userServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _feedbackController.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void Index_WithNoFilter_ReturnsAllFeedbacks()
        {
            // Arrange
            var filter = new FeedbackFilterViewModel();

            // Setup a user and barbers for the test
            var user = new User { Id = Guid.NewGuid().ToString(), FirstName = "Test", LastName = "User" };
            var barber = _context.barbers.First();

            // Create some sample feedbacks
            var feedbacks = new List<Feedback>
            {
                new Feedback
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    User = user,
                    BarberId = barber.Id,
                    Barber = barber,
                    Rating = 4,
                    Comment = "Good service"
                }
            };

            _feedbackServiceMock.Setup(x => x.GetAll()).Returns(feedbacks.AsQueryable());

            // Act
            var result = _feedbackController.Index(filter) as ViewResult;
            var model = result?.Model as FeedbackFilterViewModel;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Feedbacks.Count, Is.EqualTo(1));
        }

        [Test]
        public void Index_WithBarberFilter_ReturnsFilteredFeedbacks()
        {
            // Arrange
            var barber = _context.barbers.First();
            var filter = new FeedbackFilterViewModel { BarberId = barber.Id };

            // Create some sample feedbacks
            var feedbacks = new List<Feedback>
            {
                new Feedback
                {
                    Id = Guid.NewGuid(),
                    BarberId = barber.Id,
                    Barber = barber,
                    Rating = 4,
                    Comment = "Good service"
                }
            };

            _feedbackServiceMock.Setup(x => x.GetAll()).Returns(feedbacks.AsQueryable());

            // Act
            var result = _feedbackController.Index(filter) as ViewResult;
            var model = result?.Model as FeedbackFilterViewModel;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Feedbacks.Count, Is.EqualTo(1));
            Assert.That(model.Feedbacks.First().BarberId, Is.EqualTo(barber.Id));
        }

        [Test]
        public async Task Add_ValidFeedback_AddsSuccessfully()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString() };
            var barber = _context.barbers.First();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var tempData = new Mock<ITempDataDictionary>();
            _feedbackController.TempData = tempData.Object;

            var feedbackModel = new FeedbackCreateViewModel
            {
                BarberId = barber.Id,
                Rating = 5,
                Comment = "Excellent service",
                FeedBackDate = DateTime.UtcNow
            };

            // Act
            var result = await _feedbackController.Add(feedbackModel) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName.ToString(), Is.EqualTo("Index"));
            _feedbackServiceMock.Verify(x => x.Add(It.IsAny<Feedback>()), Times.Once);
        }

        [Test]
        public async Task Add_InvalidBarber_ReturnsViewWithError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString() };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var tempData = new Mock<ITempDataDictionary>();
            _feedbackController.TempData = tempData.Object;

            var feedbackModel = new FeedbackCreateViewModel
            {
                BarberId = Guid.Empty, // Invalid barber ID
                Rating = 5,
                Comment = "Test Comment"
            };

            // Act
            var result = await _feedbackController.Add(feedbackModel) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Edit_ValidFeedback_UpdatesSuccessfully()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString() };
            var barber = _context.barbers.First();
            var existingFeedback = new Feedback
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BarberId = barber.Id,
                Rating = 3,
                Comment = "Initial comment"
            };

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _feedbackServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Feedback, bool>>>()))
                .ReturnsAsync(existingFeedback);

            var tempData = new Mock<ITempDataDictionary>();
            _feedbackController.TempData = tempData.Object;

            var feedbackModel = new FeedbackEditViewModel
            {
                Id = existingFeedback.Id,
                SelectedBarberId = barber.Id,
                Rating = 5,
                Comment = "Updated comment"
            };

            // Act
            var result = await _feedbackController.Edit(feedbackModel) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _feedbackServiceMock.Verify(x => x.Update(It.IsAny<Feedback>()), Times.Once);
        }

        [Test]
        public async Task Delete_ExistingFeedback_DeletesSuccessfully()
        {
            // Arrange
            var feedbackId = Guid.NewGuid();
            var existingFeedback = new Feedback
            {
                Id = feedbackId,
                Comment = "Test Feedback"
            };

            _feedbackServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Feedback, bool>>>()))
                .ReturnsAsync(existingFeedback);

            var tempData = new Mock<ITempDataDictionary>();
            _feedbackController.TempData = tempData.Object;

            // Act
            var result = await _feedbackController.Delete(feedbackId) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _feedbackServiceMock.Verify(x => x.Delete(feedbackId), Times.Once);
        }

        [Test]
        public async Task AddComment_ValidUserComment_AddsSuccessfully()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString() };
            var barber = _context.barbers.First();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var tempData = new Mock<ITempDataDictionary>();
            _feedbackController.TempData = tempData.Object;

            var feedbackModel = new FeedbackCreateViewModel
            {
                BarberId = barber.Id,
                Rating = 4,
                Comment = "Nice service",
                FeedBackDate = DateTime.UtcNow
            };

            // Act
            var result = await _feedbackController.AddComment(feedbackModel) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.ControllerName, Is.EqualTo("Barber"));
            _feedbackServiceMock.Verify(x => x.Add(It.IsAny<Feedback>()), Times.Once);
        }

        [Test]
        public async Task AddComment_InvalidComment_ReturnsRedirectWithError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString() };
            var barber = _context.barbers.First();

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var tempData = new Mock<ITempDataDictionary>();
            _feedbackController.TempData = tempData.Object;

            var feedbackModel = new FeedbackCreateViewModel
            {
                BarberId = barber.Id,
                Rating = 4,
                Comment = null  // Invalid comment
            };

            // Act
            var result = await _feedbackController.AddComment(feedbackModel) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.ControllerName, Is.EqualTo("Barber"));
            _feedbackServiceMock.Verify(x => x.Add(It.IsAny<Feedback>()), Times.Never);
        }
    }
}