using System;
using System.Linq;
using System.Threading.Tasks;
using DNBarbershop.Controllers;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Services;
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
    public class ServiceControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;
        private ApplicationDbContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IServiceService> _serviceServiceMock;
        private ServiceController _serviceController;

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
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);

            // Mock Service Service
            _serviceServiceMock = new Mock<IServiceService>();

            // Setup mocked services to return seeded data
            _serviceServiceMock.Setup(x => x.GetAll())
                .Returns(_context.services);

            // Create controller
            _serviceController = new ServiceController(
                _serviceServiceMock.Object,
                _userManagerMock.Object
            );

            // Setup HttpContext and TempData
            var httpContext = new DefaultHttpContext();
            _serviceController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _serviceController.TempData = new Mock<ITempDataDictionary>().Object;
        }

        [TearDown]
        public void TearDown()
        {
            _serviceController.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_WithNoFilter_ReturnsAllServices()
        {
            // Arrange
            var serviceModel = new ServiceViewModel();

            // Act
            var result = await _serviceController.Index(serviceModel) as ViewResult;
            var model = result?.Model as ServiceViewModel;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Services.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task Add_WithUnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _serviceController.Add() as UnauthorizedResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Add_ValidService_AddsSuccessfully()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString(), Email = "admin@example.com" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var serviceModel = new ServiceCreateViewModel
            {
                ServiceName = "New Service",
                Description = "Service Description",
                Price = 50
            };

            // Setup to simulate no existing services with same name
            _serviceServiceMock.Setup(x => x.GetAll())
                .Returns(new List<Service>().AsQueryable());

            // Act
            var result = await _serviceController.Add(serviceModel, 0, 30) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _serviceServiceMock.Verify(x => x.Add(It.IsAny<Service>()), Times.Once);
        }

        [Test]
        public async Task Add_InvalidServiceData_ReturnsRedirectWithError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString(), Email = "admin@example.com" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var serviceModel = new ServiceCreateViewModel
            {
                ServiceName = "",  // Invalid name
                Description = "",  // Invalid description
                Price = 0          // Invalid price
            };

            // Act
            var result = await _serviceController.Add(serviceModel, 0, 0) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Add"));
            Assert.That(result.ControllerName, Is.EqualTo("Service"));
            _serviceServiceMock.Verify(x => x.Add(It.IsAny<Service>()), Times.Never);
        }

        [Test]
        public async Task Add_DuplicateService_ReturnsRedirectWithError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid().ToString(), Email = "admin@example.com" };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var existingServices = new List<Service>
            {
                new Service
                {
                    ServiceName = "Existing Service",
                    Description = "Existing Description"
                }
            }.AsQueryable();

            _serviceServiceMock.Setup(x => x.GetAll())
                .Returns(existingServices);

            var serviceModel = new ServiceCreateViewModel
            {
                ServiceName = "Existing Service",
                Description = "New Description",
                Price = 50
            };

            // Act
            var result = await _serviceController.Add(serviceModel, 0, 30) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Add"));
            Assert.That(result.ControllerName, Is.EqualTo("Service"));
            _serviceServiceMock.Verify(x => x.Add(It.IsAny<Service>()), Times.Never);
        }

        [Test]
        public async Task Edit_ExistingService_UpdatesSuccessfully()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var existingService = new Service
            {
                Id = serviceId,
                ServiceName = "Existing Service",
                Description = "Old Description",
                Price = 40
            };

            _serviceServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(existingService);

            var serviceModel = new ServiceEditViewModel
            {
                Id = serviceId,
                ServiceName = "Updated Service",
                Description = "New Description",
                Price = 50
            };

            // Act
            var result = await _serviceController.Edit(serviceModel, 0, 30) as RedirectToActionResult;

            // Assert
            Assert.That(result!=null);
            Assert.That(result.ActionName.ToString(), Is.EqualTo("Index"));
            _serviceServiceMock.Verify(x => x.Update(It.IsAny<Service>()), Times.Once);
        }

        [Test]
        public async Task Edit_InvalidServiceData_ReturnsRedirectWithError()
        {
            // Arrange
            var serviceModel = new ServiceEditViewModel
            {
                Id = Guid.NewGuid(),
                ServiceName = "",  // Invalid name
                Description = "",  // Invalid description
                Price = 0          // Invalid price
            };

            // Act
            var result = await _serviceController.Edit(serviceModel, 0, 0) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Edit"));
            Assert.That(result.ControllerName, Is.EqualTo("Service"));
            _serviceServiceMock.Verify(x => x.Update(It.IsAny<Service>()), Times.Never);
        }

        [Test]
        public async Task Delete_ExistingService_DeletesSuccessfully()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            var existingService = new Service { Id = serviceId };

            _serviceServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(existingService);

            // Act
            var result = await _serviceController.Delete(serviceId) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _serviceServiceMock.Verify(x => x.Delete(serviceId), Times.Once);
        }

        [Test]
        public async Task Delete_NonExistentService_ReturnsRedirectWithError()
        {
            // Arrange
            var serviceId = Guid.NewGuid();
            _serviceServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync((Service)null);

            // Act
            var result = await _serviceController.Delete(serviceId) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            _serviceServiceMock.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task ServiceDetails_WithPriceFilter_ReturnsFilteredServices()
        {
            // Arrange
            var services = new List<Service>
            {
                new Service { Price = 30 },
                new Service { Price = 50 },
                new Service { Price = 70 }
            }.AsQueryable();

            _serviceServiceMock.Setup(x => x.GetAll())
                .Returns(services);

            var filterModel = new ServiceFilterViewModel
            {
                MaxPrice = 40
            };

            // Act
            var result = await _serviceController.ServiceDetails(filterModel) as ViewResult;
            var model = result?.Model as ServiceFilterViewModel;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Services.Count, Is.EqualTo(1));
            Assert.That(model.Services.First().Price, Is.LessThanOrEqualTo(40));
        }
    }
}