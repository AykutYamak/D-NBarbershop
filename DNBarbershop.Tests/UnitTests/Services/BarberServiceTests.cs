using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNBarbershop.Core.Services;
using DNBarbershop.Models.Entities;
using DNBarbershop.DataAccess.Repository;

namespace DNBarbershop.Tests.UnitTests.Services
{
    [TestFixture]
    public class BarberServiceTestCases
    {
        private Mock<IRepository<Barber>> _mockRepository;
        private BarberService _barberService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository<Barber>>();
            _barberService = new BarberService(_mockRepository.Object);
        }

        [TestCase("Ivan", "Georgiev", "Шеф", 6)]
        [TestCase("Petar", "Petrov", "Стажант", 2)]
        [TestCase("Hristo", "Ivanov", "Майстор", 10)]
        public async Task Add_DifferentBarbers_ShouldCallRepositoryAdd(
            string firstName, string lastName, string specialityType, int experienceYears)
        {
            var barber = new Barber
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Speciality = new Speciality { Type = specialityType },
                ExperienceYears = experienceYears
            };

            await _barberService.Add(barber);

            _mockRepository.Verify(r => r.Add(barber), Times.Once);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task RemoveRange_MultipleBarbers_ShouldCallRepositoryRemoveRange(int count)
        {
            var barbers = Enumerable.Range(0, count)
                .Select(i => new Barber
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"Barber {i}",
                    Speciality = new Speciality { Type = "Стажант" }
                })
                .ToList();

            await _barberService.RemoveRange(barbers);

            _mockRepository.Verify(r => r.RemoveRange(barbers), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task DeleteAll_WithVariousBarberCounts_ShouldBehaveProperly(int count)
        {
            _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(count);

            if (count > 0)
            {
                await _barberService.DeleteAll();
                _mockRepository.Verify(r => r.DeleteAll(), Times.Once);
            }
            else
            {
                Assert.ThrowsAsync<ArgumentException>(async () => await _barberService.DeleteAll());
            }
        }

        [TestCase("Ivan", "Georgiev", "Шеф", 6)]
        [TestCase("Petar", "Petrov", "Стажант", 2)]
        [TestCase("Hristo", "Ivanov", "Майстор", 10)]
        public async Task Update_DifferentBarbers_ShouldCallRepositoryUpdate(
            string firstName, string lastName, string specialityType, int experienceYears)
        {
            var barber = new Barber
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Speciality = new Speciality { Type = specialityType },
                ExperienceYears = experienceYears
            };

            await _barberService.Update(barber);

            _mockRepository.Verify(r => r.Update(barber), Times.Once);
        }

        [Test]
        public void GetAll_ShouldReturnQueryableBarbers()
        {
            var barbers = new List<Barber>
            {
                new Barber { Id = Guid.NewGuid(), FirstName = "Ivan", Speciality = new Speciality { Type = "Шеф" } },
                new Barber { Id = Guid.NewGuid(), FirstName = "Petar", Speciality = new Speciality { Type = "Стажант" } }
            }.AsQueryable();

            _mockRepository.Setup(r => r.GetAll()).Returns(barbers);

            var result = _barberService.GetAll();

            Assert.That(2.Equals(result.Count()));
        }

        [TestCase("Ivan")]
        [TestCase("Petar")]
        [TestCase("Hristo")]
        public async Task Get_WithBarberName_ShouldReturnMatchingBarber(string firstName)
        {
            var expectedBarber = new Barber
            {
                Id = Guid.NewGuid(),
                FirstName = firstName
            };

            _mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Barber, bool>>>()))
                .ReturnsAsync(expectedBarber);

            var result = await _barberService.Get(b => b.FirstName == firstName);

            Assert.That(expectedBarber.Equals(result));
            Assert.That(firstName.Equals(result.FirstName));
        }

        [Test]
        public async Task Delete_ValidId_ShouldCallRepositoryDelete()
        {
            var barberId = Guid.NewGuid();

            await _barberService.Delete(barberId);

            _mockRepository.Verify(r => r.Delete(barberId), Times.Once);
        }

        [Test]
        public void RemoveRange_EmptyList_ShouldThrowArgumentException()
        {
            var barbers = new List<Barber>();

            Assert.ThrowsAsync<ArgumentException>(async () => await _barberService.RemoveRange(barbers));
        }
    }
}