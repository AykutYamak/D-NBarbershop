using System;
using System.Linq;
using System.Threading.Tasks;
using DNBarbershop.Controllers;
using DNBarbershop.Core.IService;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Barbers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using DNBarbershop.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DNBarbershop.Tests.UnitTests
{
    [TestFixture]
    public class BarberControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;
        private ApplicationDbContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IBarberService> _barberServiceMock;
        private Mock<IFeedbackService> _feedbackServiceMock;
        private Mock<ISpecialityService> _specialityServiceMock;
        private BarberController _barberController;
        private ITempDataDictionary _tempDataDictionaryMock;

        [SetUp]
        public void Setup()
        {
            // Create in-memory database
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DNBarbershopInMemoryDb" + Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(_dbOptions);
            _context.Database.EnsureCreated();
            DbSeeder.SeedDatabase(_context);


            // Mock UserManager
            //var store = new Mock<IUserStore<User>>();

            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            //_userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            // Mock Services
            _barberServiceMock = new Mock<IBarberService>();
            _feedbackServiceMock = new Mock<IFeedbackService>();
            _specialityServiceMock = new Mock<ISpecialityService>();

            // Setup mocked services to return seeded data
            _barberServiceMock.Setup(x => x.GetAll())
                .Returns(_context.barbers);
            _specialityServiceMock.Setup(x => x.GetAll())
                .Returns(_context.speciality);
            _feedbackServiceMock.Setup(x => x.GetAll())
                .Returns(_context.feedbacks);

            // Mocking services' methods
            _barberServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Barber, bool>>>()))
                .ReturnsAsync((Expression<Func<Barber, bool>> predicate) =>
                    _context.barbers.FirstOrDefault(predicate));

            _specialityServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Speciality, bool>>>()))
                .Returns<Func<Speciality, bool>>(predicate => Task.FromResult(_context.speciality.FirstOrDefault(predicate)));

            // Create controller
            _barberController = new BarberController(
                _userManagerMock.Object,
                _feedbackServiceMock.Object,
                _barberServiceMock.Object,
                _specialityServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _barberController.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_WithNoFilter_ReturnsAllBarbers()
        {
            // Arrange
            var filter = new BarberFilterViewModel();

            // Act
            var result = await _barberController.Index(filter) as ViewResult;
            var model = result?.Model as BarberFilterViewModel;

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That(!model.Equals(null));
            Assert.That(1.Equals(model.Barbers.Count));
        }

        [Test]
        public async Task Index_WithExperienceFilter_ReturnsFilteredBarbers()
        {
            // Arrange
            var filter = new BarberFilterViewModel { MinExperienceYears = 5 };

            // Act
            var result = await _barberController.Index(filter) as ViewResult;
            var model = result?.Model as BarberFilterViewModel;

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That(!model.Equals(null));
            Assert.That(1.Equals(model.Barbers.Count));
        }

        [Test]
        public async Task Add_ValidBarber_AddsSuccessfully()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(new User { Id = Guid.NewGuid().ToString() });

            var barberModel = new BarberCreateViewModel
            {
                FirstName = "Петър",
                LastName = "Петров",
                ExperienceYears = 3,
                ProfilePictureUrl = "https://example.com/barber.jpg",
                SelectedSpecialityId = _context.speciality.First().Id
            };

            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["success"]).Returns("Успешно добавен бръснар.");
            _barberController.TempData = tempData.Object;

            // Act
            var result = await _barberController.Add(barberModel) as RedirectToActionResult;

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That("Index".Equals(result.ActionName.ToString()));
            _barberServiceMock.Verify(x => x.Add(It.IsAny<Barber>()), Times.Once);
        }

        [Test]
        public async Task Add_InvalidBarber_ReturnsRedirectToAdd()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(new User { Id = Guid.NewGuid().ToString() });

            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["error"]).Returns("Невалидни данни!");

            // Assign to controller
            _barberController.TempData = tempData.Object;

            var barberModel = new BarberCreateViewModel
            {
                FirstName = "", // Invalid input
                LastName = "Петров",
                ExperienceYears = 40, // Invalid experience
                ProfilePictureUrl = "https://example.com/barber.jpg",
                SelectedSpecialityId = Guid.Empty
            };

            // Act
            var result = await _barberController.Add(barberModel) as RedirectToActionResult;

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That("Add".Equals(result.ActionName.ToString()));
            Assert.That("Barber".Equals(result.ControllerName.ToString()));
        }

        [Test]
        public async Task Edit_ValidBarber_UpdatesSuccessfully()
        {
            // Arrange
            var existingBarber = _context.barbers.First();
            var barberModel = new BarberEditViewModel
            {
                Id = existingBarber.Id,
                FirstName = "Иван",
                LastName = "Новев",
                ExperienceYears = 7,
                ProfilePictureUrl = existingBarber.ProfilePictureUrl,
                SelectedSpecialityId = existingBarber.SpecialityId
            };


            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["success"]).Returns("Упсешно редактиран бръснар.");

            // Assign to controller
            _barberController.TempData = tempData.Object;

            // Act
            var result = await _barberController.Edit(barberModel) as RedirectToActionResult;

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That("Index".Equals(result.ActionName.ToString()));
            _barberServiceMock.Verify(x => x.Update(It.IsAny<Barber>()), Times.Once);
        }

        [Test]
        public async Task Details_ExistingBarber_ReturnsBarberDetails()
        {
            // Arrange
            var existingBarber = _context.barbers.First();
            var feedbacks = _context.feedbacks.Select(x=>x).Where(x=>x.BarberId == existingBarber.Id).ToList();
            var speciality = _context.speciality.First();
            existingBarber.SpecialityId = speciality.Id;
            existingBarber.Feedbacks = feedbacks;
            // Act
            var result = await _barberController.Details(existingBarber.Id) as ViewResult;
            var model = result?.Model as SingleBarberViewModel;

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That(!model.Equals(null));
            Assert.That(existingBarber.FirstName.Equals(model.FirstName));
            Assert.That(existingBarber.LastName.Equals(model.LastName));
        }

        [Test]
        public async Task Delete_ExistingBarber_DeletesSuccessfully()
        {
            // Arrange
            var existingBarber = _context.barbers.First();

            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["success"]).Returns("Упсешно изтрит бръснар.");

            // Assign to controller
            _barberController.TempData = tempData.Object;

            // Act
            var result = await _barberController.Delete(existingBarber.Id) as RedirectToActionResult;

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That("Index".Equals(result.ActionName.ToString()));
            _barberServiceMock.Verify(x => x.Delete(existingBarber.Id), Times.Once);
        }
    }
}