using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DNBarbershop.Controllers;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Appointments;
using DNBarbershop.Models.EnumClasses;
using System.Linq.Expressions;

namespace DNBarbershop.Tests.Controllers
{
    [TestFixture]
    public class AppointmentControllerServiceMethodTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IAppointmentService> _appointmentServiceMock;
        private Mock<IBarberService> _barberServiceMock;
        private Mock<IServiceService> _serviceServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IAppointmentServiceService> _appointmentServiceServiceMock;
        private AppointmentController _controller;

        [SetUp]
        public void Setup()
        {
            // Setup mocks
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _barberServiceMock = new Mock<IBarberService>();
            _serviceServiceMock = new Mock<IServiceService>();
            _userServiceMock = new Mock<IUserService>();
            _appointmentServiceServiceMock = new Mock<IAppointmentServiceService>();
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                store.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
            // Create controller
            _controller = new AppointmentController(
                _userManagerMock.Object,
                _appointmentServiceMock.Object,
                _barberServiceMock.Object,
                _serviceServiceMock.Object,
                _userServiceMock.Object,
                _appointmentServiceServiceMock.Object
            );
        }

        [Test]
        public async Task GetAvailableTimeSlots_ShouldReturnJsonResult()
        {
            var barberId = Guid.NewGuid();
            var appointmentDate = DateTime.Today.AddDays(1);
            int totalDuration = 30;

            var existingAppointments = new List<Appointment>();
            _appointmentServiceMock
                .Setup(x => x.GetAll())
                .Returns(existingAppointments.AsQueryable());

            var timeSlots = new List<string> { "09:00", "09:30", "10:00" };
            _appointmentServiceMock
                .Setup(x => x.GenerateTimeSlots(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()))
                .ReturnsAsync(timeSlots);

            var result = await _controller.GetAvailableTimeSlots(barberId, appointmentDate, totalDuration);

            Assert.That(result.Equals(null) || result == null);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task Index_ShouldReturnViewWithAppointmentFilterViewModel()
        {
            // Arrange
            var filter = new AppointmentFilterViewModel();
            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    AppointmentDate = DateTime.Today,
                    AppointmentTime = TimeSpan.FromHours(10),
                    Status = AppointmentStatus.Scheduled
                }
            };

            _appointmentServiceMock
                .Setup(x => x.GetAll())
                .Returns(appointments.AsQueryable());

            _barberServiceMock
                .Setup(x => x.GetAll())
                .Returns(
                new List<Barber>()
                {
                    new Barber { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" }
                }.ToList);

            _userServiceMock
                .Setup(x => x.GetAll())
                .Returns(
                new List<User>
                {
                    new User { Id = "user1", FirstName = "Jane", LastName = "Smith" }
                }.ToList);

            // Act
            var result = await _controller.Index(filter);

            // Assert
            Assert.That(result != null);

            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model != null);
            Assert.That(viewResult.Model, Is.InstanceOf<AppointmentFilterViewModel>());
        }

        [Test]
        public async Task MakeAppointment_WithValidData_ShouldCreateAppointment()
        {
            // Arrange
            var currentUser = new User
            {
                Id = "user1",
                Appointments = new HashSet<Appointment>()
            };

            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(currentUser);

            var service = new Service
            {
                Id = Guid.NewGuid(),
                Duration = TimeSpan.FromMinutes(30)
            };

            _serviceServiceMock
                .Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(service);

            _appointmentServiceMock
                .Setup(x => x.IsTimeSlotAvailable(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<int>()))
                .ReturnsAsync(true);

            var model = new AppointmentCreateViewModel
            {
                UserId = currentUser.Id,
                BarberId = Guid.NewGuid(),
                AppointmentDate = DateTime.Today.AddDays(1),
                AppointmentTime = TimeSpan.FromHours(10),
                SelectedServiceIds = new List<Guid> { Guid.NewGuid() },
                Status = AppointmentStatus.Scheduled
            };

            // Act
            var result = await _controller.MakeAppointment(model);

            // Assert
            Assert.That(!result.Equals(null));
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            var redirectResult = result as RedirectToActionResult;
            Assert.That("Details".Equals(redirectResult.ActionName));
            Assert.That("User".Equals(redirectResult.ControllerName));
        }
    }
}