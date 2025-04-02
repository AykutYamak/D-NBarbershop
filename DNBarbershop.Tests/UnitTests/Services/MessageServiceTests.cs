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
    public class MessageServiceTests
    {
        private Mock<IRepository<Message>> _mockRepository;
        private MessageService _messageService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository<Message>>();
            _messageService = new MessageService(_mockRepository.Object);
        }

        [TestCase("test@example.com", "Hello, World!", false)]
        [TestCase("admin@system.bg", "Important Notice", true)]
        [TestCase("support@company.com", "Booking Confirmation", false)]
        public async Task Add_ValidMessage_CallsRepositoryAdd(string email, string content, bool isRead)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Email = email,
                Content = content,
                Date = DateTime.Now,
                IsRead = isRead,
                UserId = null
            };
            _mockRepository.Setup(r => r.Add(message)).Returns(Task.CompletedTask);

            await _messageService.Add(message);

            _mockRepository.Verify(r => r.Add(message), Times.Once);
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

                await _messageService.DeleteAll();

                _mockRepository.Verify(r => r.DeleteAll(), Times.Once);
            }
            else
            {
                _mockRepository.Setup(r => r.GetCount()).ReturnsAsync(0);

                var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _messageService.DeleteAll());
                Assert.That(ex.Message, Is.EqualTo("Nothing to delete here."));
            }
        }

        [TestCase("test@example.com", true)]
        [TestCase("admin@system.bg", true)]
        [TestCase("non-existent@email.com", false)]
        public async Task Get_WithVariousFilters_ReturnsExpectedResult(string email, bool shouldExist)
        {
            Expression<Func<Message, bool>> filter = m => m.Email == email;

            Message expectedMessage = shouldExist
                ? new Message
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Content = "Test Message",
                    Date = DateTime.Now,
                    IsRead = false
                }
                : null;

            _mockRepository.Setup(r => r.Get(filter)).ReturnsAsync(expectedMessage);

            var result = await _messageService.Get(filter);

            if (shouldExist)
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Email, Is.EqualTo(email));
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
        public void GetAll_WithDifferentNumberOfMessages_ReturnsCorrectCount(int messageCount)
        {
            var messages = Enumerable.Range(0, messageCount)
                .Select(i => new Message
                {
                    Id = Guid.NewGuid(),
                    Content = $"Message {i}",
                    Email = $"user{i}@example.com",
                    Date = DateTime.Now,
                    IsRead = false
                })
                .AsQueryable();

            _mockRepository.Setup(r => r.GetAll()).Returns(messages);

            var result = _messageService.GetAll();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(messageCount));
        }


        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public async Task RemoveRange_WithVariousEntityCounts_BehavesCorrectly(int entityCount)
        {
            var messages = Enumerable.Range(0, entityCount)
                .Select(i => new Message
                {
                    Id = Guid.NewGuid(),
                    Content = $"Message {i}",
                    Email = $"user{i}@example.com",
                    Date = DateTime.Now,
                    IsRead = false
                })
                .ToList();

            _mockRepository.Setup(r => r.RemoveRange(messages)).Returns(Task.CompletedTask);

            await _messageService.RemoveRange(messages);

            _mockRepository.Verify(r => r.RemoveRange(messages), Times.Once);
        }

        [TestCase("test@example.com", "Hello, World!", false)]
        [TestCase("admin@system.bg", "Updated Message", true)]
        [TestCase("support@company.com", "Booking Details", false)]
        public async Task Update_WithDifferentMessages_CallsRepositoryUpdate(string email, string content, bool isRead)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Email = email,
                Content = content,
                Date = DateTime.Now,
                IsRead = isRead
            };
            _mockRepository.Setup(r => r.Update(message)).Returns(Task.CompletedTask);

            await _messageService.Update(message);

            _mockRepository.Verify(r => r.Update(message), Times.Once);
        }

        [Test]
        public void InvalidEmail_ShouldFail_Validation()
        {
            string[] invalidEmails = new[]
            {
                "invalid-email",
                "missing@domain",
                "no-at-symbol.com",
                "@missingusername.com"
            };

            foreach (var email in invalidEmails)
            {
                var message = new Message
                {
                    Email = email,
                    Content = "Test Content",
                    Date = DateTime.Now
                };

                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(message);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                    message, validationContext, validationResults, true);

                Assert.That(isValid, Is.False, $"Email '{email}' should be invalid");
                Assert.That(validationResults.Count, Is.GreaterThan(0), $"Email '{email}' should have validation errors");
            }
        }

        [Test]
        public void ValidEmail_ShouldPass_Validation()
        {
            string[] validEmails = new[]
            {
                "user@example.com",
                "firstname.lastname@domain.com",
                "user+tag@example.co.uk",
                "user123@domain-name.com"
            };

            foreach (var email in validEmails)
            {
                var message = new Message
                {
                    Email = email,
                    Content = "Test Content",
                    Date = DateTime.Now
                };

                var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(message);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                    message, validationContext, validationResults, true);

                Assert.That(isValid, Is.True, $"Email '{email}' should be valid");
                Assert.That(validationResults.Count, Is.EqualTo(0), $"Email '{email}' should have no validation errors");
            }
        }
    }
}