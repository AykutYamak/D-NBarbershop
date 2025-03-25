//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using NUnit.Framework;
//using DNBarbershop.Controllers;
//using DNBarbershop.Models.Entities;
//using DNBarbershop.Models.ViewModels.Appointments;
//using DNBarbershop.Core.IServices;

//namespace DNBarbershop.Tests.Controllers
//{
//    [TestFixture]
//    public class AppointmentControllerTests
//    {
//        private Mock<UserManager<User>> _mockUserManager;
//        private Mock<IAppointmentService> _mockAppointmentService;
//        private Mock<IBarberService> _mockBarberService;
//        private Mock<IServiceService> _mockServiceService;
//        private Mock<IUserService> _mockUserService;
//        private Mock<IAppointmentServiceService> _mockAppointmentServiceService;
//        private Mock<AppointmentController> _appointmentController;
//        private User _currentUser;

//        [SetUp]
//        public void Setup()
//        {
//            // Create a mock user store
//            var userStoreMock = new Mock<IUserStore<User>>();

//            // Mock UserManager with a mock user store
//            _mockUserManager = new Mock<UserManager<User>>(
//                userStoreMock.Object,
//                null,
//                null,
//                null,
//                null,
//                null,
//                null,
//                null,
//                null);

//            // Initialize other mock services
//            _mockAppointmentService = new Mock<IAppointmentService>();
//            _mockBarberService = new Mock<IBarberService>();
//            _mockServiceService = new Mock<IServiceService>();
//            _mockUserService = new Mock<IUserService>();
//            _mockAppointmentServiceService = new Mock<IAppointmentServiceService>();

//            // Create a test user
//            _currentUser = new User
//            {
//                Id = Guid.NewGuid().ToString(),
//                FirstName = "Test",
//                LastName = "User",
//                Appointments = new List<Appointment>()
//            };

//            // Setup user manager to return current user
//            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
//                .ReturnsAsync(_currentUser);

//            // Create controller with mocked dependencies
//            _appointmentController = new Mock<AppointmentController>(
//                _mockUserManager.Object,
//                _mockAppointmentService.Object,
//                _mockBarberService.Object,
//                _mockServiceService.Object,
//                _mockUserService.Object,
//                _mockAppointmentServiceService.Object);
//        }

//        [Test]
//        public async Task MakeAppointment_ValidAppointment_ReturnsRedirectToDetailsAction()
//        {
//            // Arrange
//            var model = new AppointmentCreateViewModel
//            {
//                UserId = _currentUser.Id,
//                BarberId = Guid.NewGuid(),
//                AppointmentDate = DateTime.Now.AddDays(1),
//                AppointmentTime = TimeSpan.FromHours(10),
//                SelectedServiceIds = new List<Guid> { Guid.NewGuid() }
//            };

//            var service = new Service
//            {
//                Id = model.SelectedServiceIds.First(),
//                Duration = TimeSpan.FromMinutes(30)
//            };

//            // Setup mocks
//            _mockServiceService.Setup(s => s.GetAll()).Returns(new List<Service> { service }.AsQueryable());
//            _mockBarberService.Setup(s => s.GetAll()).Returns(new List<Barber> { new Barber { Id = model.BarberId } }.AsQueryable());
//            _mockAppointmentService.Setup(s => s.GetAll()).Returns(new List<Appointment>().AsQueryable());
//            _mockAppointmentService.Setup(s => s.Add(It.IsAny<Appointment>())).Returns(Task.CompletedTask);

//            // Act
//            var result = await _appointmentController.Object.MakeAppointment(model) as RedirectToActionResult;

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Not.Null);
//                Assert.That(result.ActionName, Is.EqualTo("Details"));
//                Assert.That(result.ControllerName, Is.EqualTo("User"));
//            });
//        }

//        [Test]
//        public async Task MakeAppointment_NoServices_ReturnsSameView()
//        {
//            // Arrange
//            var model = new AppointmentCreateViewModel
//            {
//                UserId = _currentUser.Id,
//                BarberId = Guid.NewGuid(),
//                AppointmentDate = DateTime.Now.AddDays(1),
//                AppointmentTime = TimeSpan.FromHours(10),
//                SelectedServiceIds = null
//            };

//            // Act
//            var result = await _appointmentController.Object.MakeAppointment(model) as ViewResult;

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Not.Null);
//                Assert.That(result.ViewName, Is.EqualTo("MakeAppointment"));
//            });
//        }

//        [Test]
//        public async Task MakeAppointment_PastDateTime_ReturnsSameView()
//        {
//            // Arrange
//            var model = new AppointmentCreateViewModel
//            {
//                UserId = _currentUser.Id,
//                BarberId = Guid.NewGuid(),
//                AppointmentDate = DateTime.Now.AddDays(-1),
//                AppointmentTime = TimeSpan.FromHours(10),
//                SelectedServiceIds = new List<Guid> { Guid.NewGuid() }
//            };

//            // Act
//            var result = await _appointmentController.Object.MakeAppointment(model) as ViewResult;

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Not.Null);
//                Assert.That(result.ViewName, Is.EqualTo("MakeAppointment"));
//            });
//        }

//        [Test]
//        public async Task Edit_ValidAppointment_ReturnsRedirectToIndexAction()
//        {
//            // Arrange
//            var existingAppointment = new Appointment
//            {
//                Id = Guid.NewGuid(),
//                UserId = _currentUser.Id,
//                BarberId = Guid.NewGuid(),
//                AppointmentDate = DateTime.Now.AddDays(1),
//                AppointmentTime = TimeSpan.FromHours(10),
//                AppointmentServices = new List<AppointmentServices>()
//            };

//            var model = new AppointmentEditViewModel
//            {
//                Id = existingAppointment.Id,
//                UserId = _currentUser.Id,
//                BarberId = existingAppointment.BarberId,
//                AppointmentDate = DateTime.Now.AddDays(2),
//                AppointmentTime = TimeSpan.FromHours(11),
//                SelectedServiceIds = new List<Guid> { Guid.NewGuid() }
//            };

//            var service = new Service
//            {
//                Id = model.SelectedServiceIds.First(),
//                Duration = TimeSpan.FromMinutes(30)
//            };

//            // Setup service mocks
//            _mockAppointmentService.Setup(s => s.GetWithRels(It.IsAny<System.Linq.Expressions.Expression<Func<Appointment, bool>>> ()))
//                .ReturnsAsync(existingAppointment);
//            _mockServiceService.Setup(s => s.GetAll()).Returns(new List<Service> { service }.AsQueryable());
//            _mockServiceService.Setup(s => s.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Service, bool>>>())).ReturnsAsync(service);
//            _mockAppointmentService.Setup(s => s.GetAll()).Returns(new List<Appointment>().AsQueryable());
//            _mockAppointmentService.Setup(s => s.Update(It.IsAny<Appointment>())).Returns(Task.CompletedTask);

//            // Act
//            var result = await _appointmentController.Object.Edit(model) as RedirectToActionResult;

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Not.Null);
//                Assert.That(result.ActionName, Is.EqualTo("Index"));
//            });
//        }

//        [Test]
//        public async Task Edit_NonExistentAppointment_ReturnsRedirectToIndexAction()
//        {
//            // Arrange
//            var model = new AppointmentEditViewModel
//            {
//                Id = Guid.NewGuid(),
//                UserId = _currentUser.Id
//            };

//            _mockAppointmentService.Setup(s => s.GetWithRels(It.IsAny<System.Linq.Expressions.Expression<Func<Appointment, bool>>>()))
//                .ReturnsAsync((Appointment)null);

//            // Act
//            var result = await _appointmentController.Object.Edit(model) as RedirectToActionResult;

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Not.Null);
//                Assert.That(result.ActionName, Is.EqualTo("Index"));
//            });
//        }

//        [Test]
//        public async Task Delete_ValidAppointment_ReturnsRedirectToIndexAction()
//        {
//            // Arrange
//            var appointmentId = Guid.NewGuid();

//            _mockAppointmentServiceService.Setup(s => s.DeleteByAppointmentId(appointmentId)).Returns(Task.CompletedTask);
//            _mockAppointmentService.Setup(s => s.Delete(appointmentId)).Returns(Task.CompletedTask);

//            // Act
//            var result = await _appointmentController.Object.Delete(appointmentId) as RedirectToActionResult;

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Not.Null);
//                Assert.That(result.ActionName, Is.EqualTo("Index"));

//                _mockAppointmentServiceService.Verify(s => s.DeleteByAppointmentId(appointmentId), Times.Once);
//                _mockAppointmentService.Verify(s => s.Delete(appointmentId), Times.Once);
//            });
//        }

//        [Test]
//        public async Task UserAppointmentDelete_ValidAppointment_ReturnsRedirectToDetailsAction()
//        {
//            // Arrange
//            var appointmentId = Guid.NewGuid();

//            _mockAppointmentServiceService.Setup(s => s.DeleteByAppointmentId(appointmentId)).Returns(Task.CompletedTask);
//            _mockAppointmentService.Setup(s => s.Delete(appointmentId)).Returns(Task.CompletedTask);

//            // Act
//            var result = await _appointmentController.Object.UserAppointmentDelete(appointmentId) as RedirectToActionResult;

//            // Assert
//            Assert.Multiple(() =>
//            {
//                Assert.That(result, Is.Not.Null);
//                Assert.That(result.ActionName, Is.EqualTo("Details"));
//                Assert.That(result.ControllerName, Is.EqualTo("User"));

//                _mockAppointmentServiceService.Verify(s => s.DeleteByAppointmentId(appointmentId), Times.Once);
//                _mockAppointmentService.Verify(s => s.Delete(appointmentId), Times.Once);
//            });
//        }
//    }
//}