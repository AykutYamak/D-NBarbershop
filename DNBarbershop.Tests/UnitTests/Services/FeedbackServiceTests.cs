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
    public class FeedbackServiceTests
    {
        private Mock<IRepository<Feedback>> _mockRepository;
        private FeedbackService _feedbackService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository<Feedback>>();
            _feedbackService = new FeedbackService(_mockRepository.Object);
        }

        [TestCase(5, "John", "Doe")]
        [TestCase(4, "Jane", "Smith")]
        [TestCase(3, "Alice", "Johnson")]
        public async Task Add_ValidFeedback_CallsRepositoryAdd(int rating, string barberFirstName, string barberLastName)
        {
            // Arrange
            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                Rating = rating,
                Barber = new Barber
                {
                    FirstName = barberFirstName,
                    LastName = barberLastName
                },
                Comment = "Great service"
            };
            _mockRepository.Setup(r => r.Add(feedback)).Returns(Task.CompletedTask);

            // Act
            await _feedbackService.Add(feedback);

            // Assert
            _mockRepository.Verify(r => r.Add(feedback), Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public async Task DeleteAll_WithDifferentRecordCounts_BehavesCorrectly(int recordCount)
        {
            if (recordCount > 0)
            {
                // Arrange
                _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(recordCount);
                _mockRepository.Setup(r => r.DeleteAll()).Returns(Task.CompletedTask);

                // Act
                await _feedbackService.DeleteAll();

                // Assert
                _mockRepository.Verify(r => r.DeleteAll(), Times.Once);
            }
            else
            {
                // Arrange
                _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(0);

                // Act & Assert
                var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _feedbackService.DeleteAll());
                Assert.That(ex.Message, Is.EqualTo("Nothing to delete here."));
            }
        }

        [TestCase("John", "Doe", true)]
        [TestCase("Jane", "Smith", true)]
        [TestCase("Non", "Existent", false)]
        public async Task Get_WithVariousFilters_ReturnsExpectedResult(string barberFirstName, string barberLastName, bool shouldExist)
        {
            // Arrange
            Expression<Func<Feedback, bool>> filter = f =>
                f.Barber.FirstName == barberFirstName && f.Barber.LastName == barberLastName;

            Feedback expectedFeedback = shouldExist
                ? new Feedback
                {
                    Id = Guid.NewGuid(),
                    Barber = new Barber
                    {
                        FirstName = barberFirstName,
                        LastName = barberLastName
                    },
                    Rating = 4,
                    Comment = "Test Feedback"
                }
                : null;

            _mockRepository.Setup(r => r.Get(filter)).ReturnsAsync(expectedFeedback);

            // Act
            var result = await _feedbackService.Get(filter);

            // Assert
            if (shouldExist)
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Barber.FirstName, Is.EqualTo(barberFirstName));
                Assert.That(result.Barber.LastName, Is.EqualTo(barberLastName));
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
        public void GetAll_WithDifferentNumberOfFeedbacks_ReturnsCorrectCount(int feedbackCount)
        {
            // Arrange
            var feedbacks = Enumerable.Range(0, feedbackCount)
                .Select(i => new Feedback
                {
                    Id = Guid.NewGuid(),
                    Barber = new Barber
                    {
                        FirstName = $"Barber{i}",
                        LastName = "LastName"
                    },
                    Rating = 4,
                    Comment = $"Feedback {i}"
                })
                .AsQueryable();

            _mockRepository.Setup(r => r.GetAll()).Returns(feedbacks);

            // Act
            var result = _feedbackService.GetAll();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(feedbackCount));
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public async Task RemoveRange_WithVariousEntityCounts_BehavesCorrectly(int entityCount)
        {
            // Arrange
            var feedbacks = Enumerable.Range(0, entityCount)
                .Select(i => new Feedback
                {
                    Id = Guid.NewGuid(),
                    Barber = new Barber
                    {
                        FirstName = $"Barber{i}",
                        LastName = "LastName"
                    },
                    Rating = 4,
                    Comment = $"Feedback {i}"
                })
                .ToList();

            _mockRepository.Setup(r => r.RemoveRange(feedbacks)).Returns(Task.CompletedTask);

            // Act
            await _feedbackService.RemoveRange(feedbacks);

            // Assert
            _mockRepository.Verify(r => r.RemoveRange(feedbacks), Times.Once);
        }

        [TestCase(5, "John", "Doe")]
        [TestCase(4, "Jane", "Smith")]
        [TestCase(3, "Alice", "Johnson")]
        public async Task Update_WithDifferentFeedbacks_CallsRepositoryUpdate(int rating, string barberFirstName, string barberLastName)
        {
            // Arrange
            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                Rating = rating,
                Barber = new Barber
                {
                    FirstName = barberFirstName,
                    LastName = barberLastName
                },
                Comment = "Updated feedback"
            };
            _mockRepository.Setup(r => r.Update(feedback)).Returns(Task.CompletedTask);

            // Act
            await _feedbackService.Update(feedback);

            // Assert
            _mockRepository.Verify(r => r.Update(feedback), Times.Once);
        }

      
    }
}
