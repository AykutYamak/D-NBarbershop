using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;
using System.ComponentModel.DataAnnotations.Schema;
using DNBarbershop.Common;

namespace DNBarbershop.Tests.Models
{
    [TestFixture]
    public class AppointmentServicesModelTests
    {
        private Appointment _validAppointment;
        private Service _validService;
        private AppointmentServices _validAppointmentServices;
        private AppointmentServices _invalidAppointmentServices;

        [SetUp]
        public void Setup()
        {
            // Create a valid User
            var user = new User
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            // Create a valid Barber
            var barber = new Barber
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                SpecialityId = Guid.NewGuid(),
                ExperienceYears = 5,
                ProfilePictureUrl = "http://example.com/profile.jpg"
            };

            // Create a valid Appointment
            _validAppointment = new Appointment
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BarberId = barber.Id,
                AppointmentDate = DateTime.Now.Date,
                AppointmentTime = new TimeSpan(14, 30, 0),
                Status = AppointmentStatus.Scheduled
            };

            // Create a valid Service
            _validService = new Service
            {
                Id = Guid.NewGuid(),
                ServiceName = "Haircut",
                Description = "Standard haircut service",
                Price = 25.00m,
                Duration = TimeSpan.FromMinutes(30)
            };

            // Create a valid AppointmentServices
            _validAppointmentServices = new AppointmentServices
            {
                AppointmentId = _validAppointment.Id,
                ServiceId = _validService.Id
            };
        }

        [Test]
        public void AppointmentServices_WhenCreated_ShouldHaveDefaultId()
        {
            var newAppointmentServices = new AppointmentServices();
            Assert.That(!Guid.Empty.Equals(newAppointmentServices.Id), "Default Id should be a new Guid");
        }

        [Test]
        public void AppointmentServices_WithValidData_ShouldPassValidation()
        {
            var validationContext = new ValidationContext(_validAppointmentServices);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_validAppointmentServices, validationContext, validationResults, true);

            Assert.That(isValid=true, "AppointmentServices should be valid with correct data");
            Assert.That(0.Equals(validationResults.Count), "No validation errors should exist");
        }

        [Test]
        public void AppointmentServices_WithoutAppointmentId_ShouldFailValidation()
        {
            // Arrange
            var invalidAppointmentServices = new AppointmentServices
            {
                ServiceId = _validService.Id // No AppointmentId set
            };

            var validationContext = new ValidationContext(invalidAppointmentServices);
            var validationResults = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(invalidAppointmentServices, validationContext, validationResults, true);

            // Assert
            Assert.Multiple(() =>
            {
                // Assert that validation failed
                Assert.That(isValid == false, "AppointmentServices should be invalid without AppointmentId.");

                // Assert that there's exactly one validation error
                Assert.That(validationResults.Count, Is.EqualTo(1), "Should have exactly one validation error.");

                // Assert that the error message matches the required error message
                Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage.ToString()),
                    "Error message should match RequiredErrorMessage.");

                // Assert that the error is specifically for the AppointmentId field
                Assert.That(validationResults[0].MemberNames.Contains("AppointmentId"),
                    "Validation error should be for AppointmentId.");
            });
        }


        [Test]
        public void AppointmentServices_WithoutServiceId_ShouldFailValidation()
        {
            _invalidAppointmentServices = new AppointmentServices
            {
                AppointmentId = _validAppointment.Id
            };

            var validationContext = new ValidationContext(_invalidAppointmentServices, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                _invalidAppointmentServices,
                validationContext,
                validationResults,
                true
            );
            Assert.That(isValid, Is.False, "AppointmentServices should be invalid without ServiceId");
            Assert.That(validationResults.Count, Is.EqualTo(1), "Should have one validation error");
        }

        [Test]
        public void AppointmentServices_RelationshipsAreCorrect()
        {
            // Verify Appointment relationship
            _validAppointmentServices.Appointment = _validAppointment;
            Assert.That(_validAppointment.Equals(_validAppointmentServices.Appointment), "Appointment relationship should be set correctly");
            Assert.That(_validAppointment.Id.Equals(_validAppointmentServices.AppointmentId), "AppointmentId should match Appointment's Id");

            // Verify Service relationship
            _validAppointmentServices.Service = _validService;
            Assert.That(_validService.Equals(_validAppointmentServices.Service), "Service relationship should be set correctly");
            Assert.That(_validService.Id.Equals(_validAppointmentServices.ServiceId), "ServiceId should match Service's Id");
        }

        [Test]
        public void AppointmentServices_DatabaseGeneratedIdAttribute_ShouldBeIdentity()
        {
            var idProperty = typeof(AppointmentServices).GetProperty("Id");
            var databaseGeneratedAttribute = idProperty.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false)[0] as DatabaseGeneratedAttribute;

            Assert.That(DatabaseGeneratedOption.Identity.Equals(databaseGeneratedAttribute.DatabaseGeneratedOption),
                "Id should be generated by the database");
        }

        [Test]
        public void AppointmentServices_NavigationProperties_ShouldBeInitializable()
        {
            var appointmentServices = new AppointmentServices
            {
                Appointment = _validAppointment,
                Service = _validService
            };

            Assert.That(_validAppointment.Equals(appointmentServices.Appointment), "Appointment navigation property should be settable");
            Assert.That(_validService.Equals(appointmentServices.Service), "Service navigation property should be settable");
        }
    }
}
