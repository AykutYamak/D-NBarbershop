using System;
using System.Linq;
using System.Threading.Tasks;
using DNBarbershop.Controllers;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using DNBarbershop.Models.ViewModels.Appointments;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using DNBarbershop.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;

namespace DNBarbershop.Tests.UnitTests.Controllers
{
    [TestFixture]
    public class AppointmentControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;
        private ApplicationDbContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IAppointmentService> _appointmentServiceMock;
        private Mock<IBarberService> _barberServiceMock;
        private Mock<IServiceService> _serviceServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IAppointmentServiceService> _appointmentServiceServiceMock;
        private AppointmentController _appointmentController;
        private User _currentUser;

        [SetUp]
        public void Setup()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DNBarbershopInMemoryDb" + Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(_dbOptions);
            _context.Database.EnsureCreated();
            DbSeeder.SeedDatabase(_context);

            _currentUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "User",
                Appointments = new List<Appointment>()
            };

            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(_currentUser);

            _appointmentServiceMock = new Mock<IAppointmentService>();
            _barberServiceMock = new Mock<IBarberService>();
            _serviceServiceMock = new Mock<IServiceService>();
            _userServiceMock = new Mock<IUserService>();
            _appointmentServiceServiceMock = new Mock<IAppointmentServiceService>();

            // Setup mock services with basic returns
            _appointmentServiceMock.Setup(x => x.GetAll())
                .Returns(_context.appointments.AsQueryable());
            _barberServiceMock.Setup(x => x.GetAll())
                .Returns(_context.barbers.AsQueryable());
            _serviceServiceMock.Setup(x => x.GetAll())
                .Returns(_context.services.AsQueryable());
            _userServiceMock.Setup(x => x.GetAll())
                .Returns(_context.users.AsQueryable());

            _appointmentController = new AppointmentController(
                _userManagerMock.Object,
                _appointmentServiceMock.Object,
                _barberServiceMock.Object,
                _serviceServiceMock.Object,
                _userServiceMock.Object,
                _appointmentServiceServiceMock.Object
            );

            // Setup TempData
            var tempData = new Mock<ITempDataDictionary>();
            _appointmentController.TempData = tempData.Object;

            // Setup HttpContext
            var httpContext = new DefaultHttpContext();
            _appointmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [TearDown]
        public void TearDown()
        {
            _appointmentController.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_WithNoFilter_ReturnsAllAppointments()
        {
            var filter = new AppointmentFilterViewModel();

            var result = await _appointmentController.Index(filter) as ViewResult;
            var model = result?.Model as AppointmentFilterViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Appointments, Is.Not.Null);
        }

        [Test]
        public async Task Add_ValidAppointment_AddsSuccessfully()
        {
            var barber = _context.barbers.First();
            var service = _context.services.First();

            var appointmentModel = new AppointmentCreateViewModel
            {
                UserId = _currentUser.Id,
                BarberId = barber.Id,
                AppointmentDate = DateTime.Now.AddDays(1),
                AppointmentTime = TimeSpan.FromHours(10),
                SelectedServiceIds = new List<Guid> { service.Id },
                Status = AppointmentStatus.Scheduled
            };

            // Setup availability check
            _appointmentServiceMock.Setup(x => x.IsTimeSlotAvailable(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<int>()))
                .ReturnsAsync(true);

            // Setup service duration
            _serviceServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(service);

            var result = await _appointmentController.Add(appointmentModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _appointmentServiceMock.Verify(x => x.Add(It.IsAny<Appointment>()), Times.Once);
            _appointmentServiceServiceMock.Verify(x => x.Add(It.IsAny<AppointmentServices>()), Times.Once);
        }

        [Test]
        public async Task Add_UnavailableTimeSlot_ReturnsError()
        {
            var barber = _context.barbers.First();
            var service = _context.services.First();

            var appointmentModel = new AppointmentCreateViewModel
            {
                UserId = _currentUser.Id,
                BarberId = barber.Id,
                AppointmentDate = DateTime.Now.AddDays(1),
                AppointmentTime = TimeSpan.FromHours(10),
                SelectedServiceIds = new List<Guid> { service.Id },
                Status = AppointmentStatus.Scheduled
            };

            // Setup availability check to return false
            _appointmentServiceMock.Setup(x => x.IsTimeSlotAvailable(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<int>()))
                .ReturnsAsync(false);

            // Setup service duration
            _serviceServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(service);

            var result = await _appointmentController.Add(appointmentModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Add"));
            _appointmentServiceMock.Verify(x => x.Add(It.IsAny<Appointment>()), Times.Never);
        }

        [Test]
        public async Task Edit_ValidAppointment_UpdatesSuccessfully()
        {
            var existingAppointment = _context.appointments.First();
            var barber = _context.barbers.First();
            var service = _context.services.First();

            var appointmentModel = new AppointmentEditViewModel
            {
                Id = existingAppointment.Id,
                UserId = _currentUser.Id,
                BarberId = barber.Id,
                AppointmentDate = DateTime.Now.AddDays(2),
                AppointmentTime = TimeSpan.FromHours(11),
                SelectedServiceIds = new List<Guid> { service.Id },
                Status = AppointmentStatus.Scheduled
            };

            // Setup existing appointment retrieval
            _appointmentServiceMock.Setup(x => x.GetWithRels(It.IsAny<Expression<Func<Appointment, bool>>>()))
                .ReturnsAsync(existingAppointment);

            // Setup availability check
            _appointmentServiceMock.Setup(x => x.IsTimeSlotAvailable(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<int>()))
                .ReturnsAsync(true);

            // Setup service duration
            _serviceServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(service);

            var result = await _appointmentController.Edit(appointmentModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _appointmentServiceMock.Verify(x => x.Update(It.IsAny<Appointment>()), Times.Once);
            _appointmentServiceServiceMock.Verify(x => x.DeleteByAppointmentId(It.IsAny<Guid>()), Times.Once);
            _appointmentServiceServiceMock.Verify(x => x.Add(It.IsAny<AppointmentServices>()), Times.Once);
        }

        [Test]
        public async Task Delete_ExistingAppointment_DeletesSuccessfully()
        {
            var existingAppointment = _context.appointments.First();

            var result = await _appointmentController.Delete(existingAppointment.Id) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _appointmentServiceServiceMock.Verify(x => x.DeleteByAppointmentId(existingAppointment.Id), Times.Once);
            _appointmentServiceMock.Verify(x => x.Delete(existingAppointment.Id), Times.Once);
        }

        [Test]
        public async Task MakeAppointment_ValidAppointment_AddsSuccessfully()
        {
            var barber = _context.barbers.First();
            var service = _context.services.First();

            var appointmentModel = new AppointmentCreateViewModel
            {
                UserId = _currentUser.Id,
                BarberId = barber.Id,
                AppointmentDate = DateTime.Now.AddDays(1),
                AppointmentTime = TimeSpan.FromHours(10),
                SelectedServiceIds = new List<Guid> { service.Id },
                Status = AppointmentStatus.Scheduled
            };

            // Setup availability check
            _appointmentServiceMock.Setup(x => x.IsTimeSlotAvailable(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<int>()))
                .ReturnsAsync(true);

            // Setup service duration
            _serviceServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(service);

            var result = await _appointmentController.MakeAppointment(appointmentModel) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Details"));
            Assert.That(result.ControllerName, Is.EqualTo("User"));
            _appointmentServiceMock.Verify(x => x.Add(It.IsAny<Appointment>()), Times.Once);
            _appointmentServiceServiceMock.Verify(x => x.Add(It.IsAny<AppointmentServices>()), Times.Once);
        }

        [Test]
        public async Task GetAvailableTimeSlots_ReturnsValidSlots()
        {
            var barberId = Guid.NewGuid();
            var appointmentDate = DateTime.Now.AddDays(1);
            var totalDuration = 60;

            var result = await _appointmentController.GetAvailableTimeSlots(barberId, appointmentDate, totalDuration) as JsonResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);
        }
    }
}