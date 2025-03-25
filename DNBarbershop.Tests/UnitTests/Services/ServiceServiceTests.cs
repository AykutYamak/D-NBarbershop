using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.DataAccess.Repository;
using Moq;

namespace DNBarbershop.Tests.UnitTests.Services
{
    [TestFixture]
    public class ServiceServiceTests
    {
        private Mock<IRepository<Service>> _mockRepository;
        private ServiceService _serviceService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository<Service>>();
            _serviceService = new ServiceService(_mockRepository.Object);
        }

        [TestCase("Haircut", 15.00)]
        [TestCase("Beard Trim", 10.50)]
        [TestCase("Full Service", 25.75)]
        public async Task Add_DifferentServices_ShouldCallRepositoryAdd(string serviceName, decimal price)
        {
            var service = new Service
            {
                Id = Guid.NewGuid(),
                ServiceName = serviceName,
                Price = price,
                Duration = TimeSpan.FromMinutes(30)
            };

            await _serviceService.Add(service);

            _mockRepository.Verify(r => r.Add(service), Times.Once);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task RemoveRange_MultipleServices_ShouldCallRepositoryRemoveRange(int count)
        {
            var services = Enumerable.Range(0, count)
                .Select(i => new Service
                {
                    Id = Guid.NewGuid(),
                    ServiceName = $"Service {i}",
                    Price = 10 + i
                })
                .ToList();

            await _serviceService.RemoveRange(services);

            _mockRepository.Verify(r => r.RemoveRange(services), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task DeleteAll_WithVariousServiceCounts_ShouldBehaveProperly(int count)
        {
            _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(count);

            if (count > 0)
            {
                await _serviceService.DeleteAll();
                _mockRepository.Verify(r => r.DeleteAll(), Times.Once);
            }
            else
            {
                Assert.ThrowsAsync<ArgumentException>(async () => await _serviceService.DeleteAll());
            }
        }

        [TestCase("Haircut", 15.00, 30)]
        [TestCase("Beard Trim", 10.50, 20)]
        [TestCase("Full Service", 25.75, 45)]
        public async Task Update_DifferentServices_ShouldCallRepositoryUpdate(
            string serviceName, decimal price, int durationMinutes)
        {
            var service = new Service
            {
                Id = Guid.NewGuid(),
                ServiceName = serviceName,
                Price = price,
                Duration = TimeSpan.FromMinutes(durationMinutes)
            };

            await _serviceService.Update(service);

            _mockRepository.Verify(r => r.Update(service), Times.Once);
        }

        [Test]
        public void GetAll_ShouldReturnQueryableServices()
        {
            var services = new List<Service>
            {
                new Service { Id = Guid.NewGuid(), ServiceName = "Service 1", Price = 10 },
                new Service { Id = Guid.NewGuid(), ServiceName = "Service 2", Price = 20 }
            }.AsQueryable();

            _mockRepository.Setup(r => r.GetAll()).Returns(services);

            var result = _serviceService.GetAll();

            Assert.That(2.Equals(result.Count()));
        }

        [TestCase("Haircut")]
        [TestCase("Beard Trim")]
        [TestCase("Full Service")]
        public async Task Get_WithServiceName_ShouldReturnMatchingService(string serviceName)
        {
            var expectedService = new Service
            {
                Id = Guid.NewGuid(),
                ServiceName = serviceName
            };

            _mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Service, bool>>>()))
                .ReturnsAsync(expectedService);

            var result = await _serviceService.Get(s => s.ServiceName == serviceName);

            Assert.That(expectedService.Equals(result));
            Assert.That(serviceName.Equals(result.ServiceName));
        }

        [Test]
        public async Task Delete_ValidId_ShouldCallRepositoryDelete()
        {
            var serviceId = Guid.NewGuid();

            await _serviceService.Delete(serviceId);

            _mockRepository.Verify(r => r.Delete(serviceId), Times.Once);
        }
    }
}
