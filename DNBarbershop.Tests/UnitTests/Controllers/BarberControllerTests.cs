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
using DNBarbershop.Core.Services;
using System.Reflection;

namespace DNBarbershop.Tests.UnitTests.Controllers
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

            _barberServiceMock = new Mock<IBarberService>();
            _feedbackServiceMock = new Mock<IFeedbackService>();
            _specialityServiceMock = new Mock<ISpecialityService>();

            _barberServiceMock.Setup(x => x.GetAll())
                .Returns(_context.barbers);
            _specialityServiceMock.Setup(x => x.GetAll())
                .Returns(_context.speciality);
            _feedbackServiceMock.Setup(x => x.GetAll())
                .Returns(_context.feedbacks);

            _barberServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Barber, bool>>>()))
                .ReturnsAsync((Expression<Func<Barber, bool>> predicate) =>
                    _context.barbers.FirstOrDefault(predicate));

            _specialityServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Speciality, bool>>>()))
                .Returns<Func<Speciality, bool>>(predicate => Task.FromResult(_context.speciality.FirstOrDefault(predicate)));

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
            var filter = new BarberFilterViewModel();

            var result = await _barberController.Index(filter) as ViewResult;
            var model = result?.Model as BarberFilterViewModel;

            Assert.That(!result.Equals(null));
            Assert.That(!model.Equals(null));
            Assert.That(1.Equals(model.Barbers.Count));
        }

        [Test]
        public async Task Index_WithExperienceFilter_ReturnsFilteredBarbers()
        {
            var filter = new BarberFilterViewModel { MinExperienceYears = 5 };

            var result = await _barberController.Index(filter) as ViewResult;
            var model = result?.Model as BarberFilterViewModel;

            Assert.That(!result.Equals(null));
            Assert.That(!model.Equals(null));
            Assert.That(1.Equals(model.Barbers.Count));
        }

        [Test]
        public async Task Add_ValidBarber_AddsSuccessfully()
        {
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

            var result = await _barberController.Add(barberModel) as RedirectToActionResult;

            Assert.That(!result.Equals(null));
            Assert.That("Index".Equals(result.ActionName.ToString()));
            _barberServiceMock.Verify(x => x.Add(It.IsAny<Barber>()), Times.Once);
        }

        [Test]
        public async Task Add_InvalidBarber_ReturnsRedirectToAdd()
        {
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(new User { Id = Guid.NewGuid().ToString() });

            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["error"]).Returns("Невалидни данни!");

            _barberController.TempData = tempData.Object;

            var barberModel = new BarberCreateViewModel
            {
                FirstName = "",
                LastName = "Петров",
                ExperienceYears = 40, 
                ProfilePictureUrl = "https://example.com/barber.jpg",
                SelectedSpecialityId = Guid.Empty
            };

            var result = await _barberController.Add(barberModel) as RedirectToActionResult;

            Assert.That(!result.Equals(null));
            Assert.That("Add".Equals(result.ActionName.ToString()));
            Assert.That("Barber".Equals(result.ControllerName.ToString()));
        }

        [Test]
        public async Task Edit_ValidBarber_UpdatesSuccessfully()
        {
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

            _barberController.TempData = tempData.Object;

            var result = await _barberController.Edit(barberModel) as RedirectToActionResult;

            Assert.That(!result.Equals(null));
            Assert.That("Index".Equals(result.ActionName.ToString()));
            _barberServiceMock.Verify(x => x.Update(It.IsAny<Barber>()), Times.Once);
        }

        [Test]
        public async Task Details_ExistingBarber_ReturnsBarberDetails()
        {
            var barberId = Guid.NewGuid();
            var specialityId = Guid.NewGuid();

            var barber = new Barber
            {
                Id = barberId,
                FirstName = "Иван",
                LastName = "Новев",
                ExperienceYears = 5,
                ProfilePictureUrl = "https://example.com/barber.jpg",
                SpecialityId = specialityId
            };

            var feedbacks = new List<Feedback>
    {
        new Feedback { BarberId = barberId, User = new User { UserName = "User1" }, FeedBackDate = DateTime.Now.AddDays(-1) },
        new Feedback { BarberId = barberId, User = new User { UserName = "User2" }, FeedBackDate = DateTime.Now }
    };

            var speciality = new Speciality
            {
                Id = specialityId,
                Type = "Шеф"
            };

            _barberServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<Barber, bool>>>()))
                              .ReturnsAsync(barber);

            _feedbackServiceMock.Setup(f => f.GetAll())
                                .Returns(feedbacks.AsQueryable());

            _specialityServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<Speciality, bool>>>()))
                                  .ReturnsAsync(speciality);

            var result = await _barberController.Details(barberId) as ViewResult;
            var model = result?.Model as SingleBarberViewModel;

            Assert.That(result != null, "Result should not be null.");
            Assert.That(model != null, "Model should not be null.");

            Assert.That(barberId == model.Id, "Barber ID should match.");
            Assert.That("Иван" == model.FirstName, "FirstName should match.");
            Assert.That("Новев" == model.LastName, "LastName should match.");
            Assert.That("Шеф" == model.Speciality, "Speciality should match.");
            Assert.That(2 == model.Feedbacks.Count, "Feedback count should be 2.");
            Assert.That("User2" == model.Feedbacks.First().User.UserName, "Latest feedback should be first.");
        }

        [Test]
        public async Task Delete_ExistingBarber_DeletesSuccessfully()
        {
            var existingBarber = _context.barbers.First();

            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["success"]).Returns("Упсешно изтрит бръснар.");

            _barberController.TempData = tempData.Object;

            var result = await _barberController.Delete(existingBarber.Id) as RedirectToActionResult;

            Assert.That(!result.Equals(null));
            Assert.That("Index".Equals(result.ActionName.ToString()));
            _barberServiceMock.Verify(x => x.Delete(existingBarber.Id), Times.Once);
        }
        
        [Test]
        public void GetAll_ShouldReturnEmptyList_WhenNoBarbersExist()
        {
            _barberServiceMock.Setup(r => r.GetAll()).Returns(new List<Barber>().AsQueryable());

            var result = _barberServiceMock.Object.GetAll();

            Assert.That(!result.Any());
        }
        [Test]
        public async Task Get_WithNonExistentBarber_ShouldReturnNull()
        {
            var result = await _barberServiceMock.Object.Get(b => b.FirstName == "NonExistent");

            Assert.That(result == null || result.ToString() == "System.NullReferenceException");
        }

        [Test]
        public void GetAll_ShouldReturnAllBarbers()
        {
            var barbers = new List<Barber>
            {
                new Barber { Id = Guid.NewGuid(), FirstName = "Ivan", LastName = "Petrov" },
                new Barber { Id = Guid.NewGuid(), FirstName = "Petar", LastName = "Georgiev" }
            }.AsQueryable();

            _barberServiceMock.Setup(r => r.GetAll()).Returns(barbers);

            var result = _barberServiceMock.Object.GetAll();

            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}