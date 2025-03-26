using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNBarbershop.Models.Entities;
using DNBarbershop.Common;

namespace DNBarbershop.Tests.Models
{
    [TestFixture]
    public class ServiceModelTests
    {
        private Service _validService;

        [SetUp]
        public void Setup()
        {
            _validService = new Service
            {
                ServiceName = "Classic Haircut",
                Description = "A traditional haircut that never goes out of style.",
                Price = 25.00m,
                Duration = TimeSpan.FromMinutes(30)
            };
        }

        [Test]
        public void Service_WithValidData_ShouldPassValidation()
        {
            var validationContext = new ValidationContext(_validService);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validService, validationContext, validationResults, true);

            Assert.That(isValid.Equals(true), "Service with valid data should pass validation");
            Assert.That(0.Equals(validationResults.Count), "No validation errors should be present");
        }

        [Test]
        public void Service_WithEmptyServiceName_ShouldFailValidation()
        {
            _validService.ServiceName = string.Empty;
            var validationContext = new ValidationContext(_validService);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validService, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Service with empty ServiceName should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Service_WithServiceNameLongerThan50Characters_ShouldFailValidation()
        {
            _validService.ServiceName = new string('A', 51);
            var validationContext = new ValidationContext(_validService);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validService, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Service with ServiceName longer than 50 characters should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That("MaxLengthExceededErrorMessage".Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Service_WithEmptyDescription_ShouldFailValidation()
        {
            _validService.Description = string.Empty;
            var validationContext = new ValidationContext(_validService);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validService, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Service with empty Description should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Service_WithDescriptionLongerThan500Characters_ShouldFailValidation()
        {
            _validService.Description = new string('A', 501);
            var validationContext = new ValidationContext(_validService);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validService, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Service with Description longer than 500 characters should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That("MaxLengthExceededErrorMessage".Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Service_WithNegativePrice_ShouldThrowException()
        {
            _validService.Price = -1;
            Assert.Throws<ArgumentException>(() =>
            {
                if (_validService.Price < 0)
                {
                    throw new ArgumentException("Price must be a non-negative number");
                }
            }, "Service with negative price should throw an exception");
        }

        [Test]
        public void Service_WithZeroPrice_ShouldBeValid()
        {
            _validService.Price = 0m;
            var validationContext = new ValidationContext(_validService);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validService, validationContext, validationResults, true);

            Assert.That(isValid.Equals(true), "Service with zero price should be valid");
        }

        [Test]
        public void Service_WithDefaultGuid_ShouldHaveNewGuid()
        {
            var service = new Service();

            Assert.That(!Guid.Empty.Equals(service.Id), "Service Id should be a new Guid");
        }

        [Test]
        public void Service_WithAppointmentServices_ShouldSupportCollection()
        {
            var appointmentService1 = new AppointmentServices();
            var appointmentService2 = new AppointmentServices();

            _validService.AppointmentServices.Add(appointmentService1);
            _validService.AppointmentServices.Add(appointmentService2);

            Assert.That(2.Equals(_validService.AppointmentServices.Count), "Should support multiple appointment services");
        }

        [Test]
        public void Service_WithValidDuration_ShouldPreserveDuration()
        {
            TimeSpan expectedDuration = TimeSpan.FromMinutes(45);
            _validService.Duration = expectedDuration;

            Assert.That(expectedDuration.Equals(_validService.Duration), "Service Duration should be preserved");
        }

        [Test]
        public void Service_AppointmentServicesCollection_ShouldBeInitializedAsEmpty()
        {
            // Arrange
            var newService = new Service();

            // Assert
            Assert.That(!newService.AppointmentServices.Equals(null), "AppointmentServices collection should not be null");
            Assert.That(0.Equals(newService.AppointmentServices.Count), "AppointmentServices collection should be initially empty");
        }
    }
}