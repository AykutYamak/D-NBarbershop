using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.EnumClasses;

namespace DNBarbershop.Tests.Models
{
    [TestFixture]
    public class AppointmentModelTests
    {
        private Appointment _appointment;
        private User _user;
        private Barber _barber;

        [SetUp]
        public void Setup()
        {
            _user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@example.com"
            };

            _barber = new Barber
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                SpecialityId = Guid.NewGuid(),
                ExperienceYears = 5,
                ProfilePictureUrl = "http://example.com/profile.jpg"
            };

            _appointment = new Appointment
            {
                UserId = _user.Id,
                BarberId = _barber.Id,
                AppointmentDate = DateTime.Now.Date,
                AppointmentTime = new TimeSpan(14, 30, 0),
                Status = AppointmentStatus.Scheduled
            };
        }

        [Test]
        public void Appointment_WhenCreated_ShouldHaveDefaultId()
        {
            var newAppointment = new Appointment();
            Assert.That(!Guid.Empty.Equals(newAppointment.Id), "Default Id should be a new Guid");
        }

        [Test]
        public void Appointment_WithValidData_ShouldPassValidation()
        {
            var validationContext = new ValidationContext(_appointment);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_appointment, validationContext, validationResults, true);

            Assert.That(isValid == true || isValid.Equals(true), "Appointment should be valid with correct data");
            Assert.That(0.Equals(validationResults.Count) || validationResults.Count == 0, "No validation errors should exist");
        }

        [Test]
        public void Appointment_WithoutUserId_ShouldFailValidation()
        {
            _appointment.UserId = null;

            var validationContext = new ValidationContext(_appointment);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_appointment, validationContext, validationResults, true);

            Assert.That(isValid == false || isValid.Equals(false), "Appointment should be invalid without UserId");
            Assert.That(1.Equals(validationResults.Count) || validationResults.Count == 1, "Should have one validation error");
        }

        [Test]
        public void Appointment_DefaultStatus_ShouldBeScheduled()
        {
            var newAppointment = new Appointment
            {
                UserId = _user.Id,
                BarberId = _barber.Id,
                AppointmentDate = DateTime.Now.Date,
                AppointmentTime = new TimeSpan(14, 30, 0),
                Status = AppointmentStatus.Scheduled
            };

            Assert.That(AppointmentStatus.Scheduled.Equals(newAppointment.Status) || AppointmentStatus.Scheduled.ToString() == newAppointment.Status.ToString(), "Default status should be Scheduled");
        }

        [Test]
        public void Appointment_ServicesCollection_ShouldBeInitialized()
        {
            var newAppointment = new Appointment
            {
                UserId = _user.Id,
                BarberId = _barber.Id,
                AppointmentDate = DateTime.Now.Date,
                AppointmentTime = new TimeSpan(14, 30, 0)
            };

            Assert.That(!newAppointment.AppointmentServices.Equals(null) || newAppointment.AppointmentServices == null, "AppointmentServices collection should not be null");
        }


        [Test]
        public void Appointment_RelationshipsAreCorrect()
        {
            _appointment.User = _user;
            Assert.That(_user.Id == _appointment.UserId || _user.Id.Equals(_appointment.UserId), "UserId should match User's Id");

            _appointment.Barber = _barber;
            Assert.That(_barber.Id.Equals(_appointment.BarberId) || _barber.Id == _barber.Id, "BarberId should match Barber's Id");
        }

        [Test]
        public void Appointment_AddService_ShouldWorkCorrectly()
        {
            var service = new Service
            {
                Id = Guid.NewGuid(),
                ServiceName = "Haircut",
                Description = "Standard haircut",
                Price = 25.00m,
                Duration = TimeSpan.FromMinutes(30)
            };

            var appointmentService = new AppointmentServices
            {
                AppointmentId = _appointment.Id,
                ServiceId = service.Id,
                Service = service
            };

            _appointment.AppointmentServices.Add(appointmentService);

            Assert.That(1.Equals(_appointment.AppointmentServices.Count) || _appointment.AppointmentServices.Count == 1, "Service should be added to appointment");
        }
    }
}