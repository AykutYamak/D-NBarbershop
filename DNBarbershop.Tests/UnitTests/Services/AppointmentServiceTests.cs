using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.DataAccess.AppointmentRepository;
using Moq;
using DNBarbershop.Models.EnumClasses;

namespace DNBarbershop.Tests.UnitTests.Services
{
    [TestFixture]
    public class AppointmentServiceTests
    {
        private Mock<IRepository<Appointment>> _mockRepository;
        private Mock<IAppointmentRepository<Appointment>> _mockAppointmentRepo;
        private AppointmentService _appointmentService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository<Appointment>>();
            _mockAppointmentRepo = new Mock<IAppointmentRepository<Appointment>>();
            _appointmentService = new AppointmentService(_mockRepository.Object, _mockAppointmentRepo.Object);
        }

        [TestCase(AppointmentStatus.Scheduled)]
        [TestCase(AppointmentStatus.Cancelled)]
        [TestCase(AppointmentStatus.Completed)]
        public async Task Add_AppointmentWithDifferentStatuses_ShouldCallRepositoryAdd(AppointmentStatus status)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                AppointmentDate = DateTime.Now,
                Status = status
            };

            await _appointmentService.Add(appointment);

            _mockRepository.Verify(r => r.Add(appointment), Times.Once);
        }
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task RemoveRange_MultipleEntityCounts_ShouldCallRepositoryRemoveRange(int count)
        {
            var appointments = Enumerable.Range(0, count)
                .Select(_ => new Appointment { Id = Guid.NewGuid() })
                .ToList();

            await _appointmentService.RemoveRange(appointments);

            _mockRepository.Verify(r => r.RemoveRange(appointments), Times.Once);
        }

        [TestCase(AppointmentStatus.Scheduled, "2025-04-23")]
        [TestCase(AppointmentStatus.Cancelled, "2025-12-31")]
        [TestCase(AppointmentStatus.Completed, "2026-01-15")]
        public async Task Update_AppointmentWithDifferentStatuses_ShouldCallRepositoryUpdate(
            AppointmentStatus status, string dateString)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                AppointmentDate = DateTime.Parse(dateString),
                Status = status
            };

            await _appointmentService.Update(appointment);

            _mockRepository.Verify(r => r.Update(appointment), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task DeleteAll_WithVariousAppointmentCounts_ShouldBehaveProperly(int count)
        {

            _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(count);

            if (count > 0)
            {
                await _appointmentService.DeleteAll();
                _mockRepository.Verify(r => r.DeleteAll(), Times.Once);
            }
            else
            {
                Assert.ThrowsAsync<ArgumentException>(async () => await _appointmentService.DeleteAll());
            }
        }
    }
}
