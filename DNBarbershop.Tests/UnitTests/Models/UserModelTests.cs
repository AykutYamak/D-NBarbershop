using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using DNBarbershop.Models.Entities;
using DNBarbershop.Common;

namespace DNBarbershop.Tests.Models
{
    [TestFixture]
    public class UserModelTests
    {
        private User _user;

        [SetUp]
        public void Setup()
        {
            _user = new User();
        }

        [Test]
        public void FirstName_RequiredValidation_ShouldFail_WhenNull()
        {
            // Arrange
            var validationContext = new ValidationContext(_user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(
                null,
                validationContext,
                validationResults,
                typeof(User).GetProperty(nameof(User.FirstName))
            );

            // Assert
            Assert.IsFalse(isValid, "FirstName should be required");
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage));
        }

        [Test]
        public void FirstName_RequiredValidation_ShouldFail_WhenEmpty()
        {
            // Arrange
            _user.FirstName = string.Empty;
            var validationContext = new ValidationContext(_user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(
                _user.FirstName,
                validationContext,
                validationResults,
                typeof(User).GetProperty(nameof(User.FirstName))
            );

            // Assert
            Assert.IsFalse(isValid, "FirstName should not be empty");
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage));
        }

        [Test]
        public void FirstName_StringLengthValidation_ShouldFail_WhenExceedsMaxLength()
        {
            // Arrange
            _user.FirstName = new string('A', 101);
            var validationContext = new ValidationContext(_user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(
                _user.FirstName,
                validationContext,
                validationResults,
                typeof(User).GetProperty(nameof(User.FirstName))
            );

            // Assert
            Assert.IsFalse(isValid, "FirstName should not exceed 100 characters");
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(nameof(ErrorMessages.MaxLengthExceededErrorMessage)));
        }

        [Test]
        public void LastName_RequiredValidation_ShouldFail_WhenNull()
        {
            // Arrange
            var validationContext = new ValidationContext(_user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(
                null,
                validationContext,
                validationResults,
                typeof(User).GetProperty(nameof(User.LastName))
            );

            // Assert
            Assert.IsFalse(isValid, "LastName should be required");
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage));
        }

        [Test]
        public void LastName_RequiredValidation_ShouldFail_WhenEmpty()
        {
            // Arrange
            _user.LastName = string.Empty;
            var validationContext = new ValidationContext(_user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(
                _user.LastName,
                validationContext,
                validationResults,
                typeof(User).GetProperty(nameof(User.LastName))
            );

            // Assert
            Assert.IsFalse(isValid, "LastName should not be empty");
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage));
        }

        [Test]
        public void LastName_StringLengthValidation_ShouldFail_WhenExceedsMaxLength()
        {
            // Arrange
            _user.LastName = new string('B', 101);
            var validationContext = new ValidationContext(_user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(
                _user.LastName,
                validationContext,
                validationResults,
                typeof(User).GetProperty(nameof(User.LastName))
            );

            // Assert
            Assert.IsFalse(isValid, "LastName should not exceed 100 characters");
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(nameof(ErrorMessages.MaxLengthExceededErrorMessage)));
        }

        [Test]
        public void User_CollectionProperties_ShouldInitializeEmpty()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.IsNotNull(user.Appointments);
            Assert.IsNotNull(user.Feedbacks);
            Assert.IsNotNull(user.Messages);
            Assert.That(user.Appointments, Is.Empty);
            Assert.That(user.Feedbacks, Is.Empty);
            Assert.That(user.Messages, Is.Empty);
        }

        [Test]
        public void User_CollectionProperties_ShouldAllowAddingItems()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe"
            };

            var appointment = new Appointment();
            var feedback = new Feedback();
            var message = new Message();

            // Act
            user.Appointments.Add(appointment);
            user.Feedbacks.Add(feedback);
            user.Messages.Add(message);

            // Assert
            Assert.That(user.Appointments, Does.Contain(appointment));
            Assert.That(user.Feedbacks, Does.Contain(feedback));
            Assert.That(user.Messages, Does.Contain(message));
        }
    }

    // Placeholder classes for compilation
    public class Appointment { }
    public class Feedback { }
    public class Message { }
}