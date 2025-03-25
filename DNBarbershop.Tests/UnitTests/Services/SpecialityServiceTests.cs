using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using DNBarbershop.Core.Services;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Tests.UnitTests.Services
{
    [TestFixture]
    public class SpecialityServiceTests
    {
        private Mock<IRepository<Speciality>> _mockRepository;
        private SpecialityService _specialityService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository<Speciality>>();
            _specialityService = new SpecialityService(_mockRepository.Object);
        }

        [TestCase("Haircut")]
        [TestCase("Coloring")]
        [TestCase("Beard Trim")]
        public async Task Add_ValidSpeciality_CallsRepositoryAdd(string specialityName)
        {
            var speciality = new Speciality { Id = Guid.NewGuid(), Type = specialityName };
            _mockRepository.Setup(r => r.Add(speciality)).Returns(Task.CompletedTask);

            await _specialityService.Add(speciality);

            _mockRepository.Verify(r => r.Add(speciality), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task DeleteAll_WithDifferentRecordCounts_BehavesCorrectly(int recordCount)
        {
            if (recordCount > 0)
            {
                _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(recordCount);
                _mockRepository.Setup(r => r.DeleteAll()).Returns(Task.CompletedTask);

                await _specialityService.DeleteAll();

                _mockRepository.Verify(r => r.DeleteAll(), Times.Once);
            }
            else
            {
                _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(0);

                var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _specialityService.DeleteAll());
                Assert.That(ex.Message, Is.EqualTo("Nothing to delete here."));
            }
        }

        [TestCase("Styling", true)]
        [TestCase("Coloring", true)]
        [TestCase("Non-Existent", false)]
        public async Task Get_WithVariousFilters_ReturnsExpectedResult(string specialityName, bool shouldExist)
        {
            Expression<Func<Speciality, bool>> filter = s => s.Type == specialityName;

            Speciality expectedSpeciality = shouldExist
                ? new Speciality { Id = Guid.NewGuid(), Type = specialityName }
                : null;

            _mockRepository.Setup(r => r.Get(filter)).ReturnsAsync(expectedSpeciality);

            var result = await _specialityService.Get(filter);

            if (shouldExist)
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Type, Is.EqualTo(specialityName));
            }
            else
            {
                Assert.That(result, Is.Null);
            }
            _mockRepository.Verify(r => r.Get(filter), Times.Once);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void GetAll_WithDifferentNumberOfSpecialities_ReturnsCorrectCount(int specialityCount)
        {
            var specialities = Enumerable.Range(0, specialityCount)
                .Select(i => new Speciality
                {
                    Id = Guid.NewGuid(),
                    Type = $"Speciality {i}"
                })
                .AsQueryable();

            _mockRepository.Setup(r => r.GetAll()).Returns(specialities);

            var result = _specialityService.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(specialityCount));
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public async Task RemoveRange_WithVariousEntityCounts_BehavesCorrectly(int entityCount)
        {
            var specialities = Enumerable.Range(0, entityCount)
                .Select(i => new Speciality
                {
                    Id = Guid.NewGuid(),
                    Type = $"Speciality {i}"
                })
                .ToList();

            _mockRepository.Setup(r => r.RemoveRange(specialities)).Returns(Task.CompletedTask);

            await _specialityService.RemoveRange(specialities);

            _mockRepository.Verify(r => r.RemoveRange(specialities), Times.Once);
        }

        [Test]
        public void RemoveRange_WithEmptyList_ThrowsArgumentException()
        {
            var specialities = new List<Speciality>();

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _specialityService.RemoveRange(specialities));
            Assert.That(ex.Message, Is.EqualTo("Validation didn't pass."));
        }

        [TestCase("Haircut")]
        [TestCase("Coloring")]
        [TestCase("Beard Styling")]
        public async Task Update_WithDifferentSpecialities_CallsRepositoryUpdate(string specialityName)
        {
            var speciality = new Speciality { Id = Guid.NewGuid(), Type = specialityName };
            _mockRepository.Setup(r => r.Update(speciality)).Returns(Task.CompletedTask);

            await _specialityService.Update(speciality);

            _mockRepository.Verify(r => r.Update(speciality), Times.Once);
        }
    }
}