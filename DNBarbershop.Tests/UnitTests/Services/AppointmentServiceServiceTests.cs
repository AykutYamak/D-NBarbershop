using DNBarbershop.Core.Services;
using DNBarbershop.DataAccess.AppointmentServiceRepository;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
using Moq;

namespace DNBarbershop.Tests.UnitTests.Services
{
    [TestFixture]
    public class AppointmentServiceServiceTests
    {
        private Mock<IRepository<AppointmentServices>> _mockRepository;
        private Mock<IAppointmentServiceRepository<AppointmentServices>> _mockAppointmentServiceRepository;
        private AppointmentServiceService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository<AppointmentServices>>();
            _mockAppointmentServiceRepository = new Mock<IAppointmentServiceRepository<AppointmentServices>>();
            _service = new AppointmentServiceService(
                _mockRepository.Object,
                _mockAppointmentServiceRepository.Object
            );
        }

        [Test]
        public async Task Add_ValidEntity_ShouldCallRepositoryAddMethod()
        {
            var appointmentService = new AppointmentServices
            {
                Id = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid()
            };

            await _service.Add(appointmentService);

            _mockRepository.Verify(r => r.Add(appointmentService), Times.Once);
        }

        [Test]
        public async Task Add_NullEntity_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.Add(null)
            );
        }

        [Test]
        public async Task Delete_ValidIds_ShouldCallRepositoryDeleteMethod()
        {
            var appointmentId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();

            await _service.Delete(appointmentId, serviceId);

            _mockAppointmentServiceRepository.Verify(
                r => r.Delete(appointmentId, serviceId),
                Times.Once
            );
        }

        [Test]
        public async Task DeleteByAppointmentId_ValidAppointmentId_ShouldCallRepositoryDeleteMethod()
        {
            var appointmentId = Guid.NewGuid();

            await _service.DeleteByAppointmentId(appointmentId);

            _mockAppointmentServiceRepository.Verify(
                r => r.DeleteByAppointmentId(appointmentId),
                Times.Once
            );
        }

        [Test]
        public async Task Update_ValidEntity_ShouldCallRepositoryUpdateMethod()
        {
            var appointmentService = new AppointmentServices
            {
                Id = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid()
            };

            await _service.Update(appointmentService);

            _mockRepository.Verify(r => r.Update(appointmentService), Times.Once);
        }

        [Test]
        public async Task Update_NullEntity_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.Update(null)
            );
        }

    }
}