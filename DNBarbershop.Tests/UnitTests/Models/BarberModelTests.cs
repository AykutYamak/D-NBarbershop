using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNBarbershop.Models.Entities;
using DNBarbershop.Common;

namespace DNBarbershop.Tests.Models
{
    [TestFixture]
    public class BarberModelTests
    {
        private Barber _validBarber;
        private Speciality _testSpeciality;

        [SetUp]
        public void Setup()
        {
            _testSpeciality = new Speciality
            {
                Id = Guid.NewGuid(),
                Type = "Haircut"
            };

            _validBarber = new Barber
            {
                FirstName = "John",
                LastName = "Doe",
                SpecialityId = _testSpeciality.Id,
                Speciality = _testSpeciality,
                ExperienceYears = 5,
                ProfilePictureUrl = "https://example.com/profile.jpg"
            };
        }

        [Test]
        public void Barber_WithValidData_ShouldPassValidation()
        {
            var validationContext = new ValidationContext(_validBarber);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validBarber, validationContext, validationResults, true);

            Assert.That(isValid.Equals(true), "Barber with valid data should pass validation");
            Assert.That(0.Equals(validationResults.Count), "No validation errors should be present");
        }

        [Test]
        public void Barber_WithEmptyFirstName_ShouldFailValidation()
        {
            _validBarber.FirstName = string.Empty;
            var validationContext = new ValidationContext(_validBarber);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validBarber, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Barber with empty FirstName should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Barber_WithFirstNameLongerThan100Characters_ShouldFailValidation()
        {
            _validBarber.FirstName = new string('A', 101);
            var validationContext = new ValidationContext(_validBarber);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validBarber, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), ErrorMessages.MaxLengthExceededErrorMessage(100));
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That("MaxLengthExceededErrorMessage" == validationResults[0].ErrorMessage.ToString());
        }

        [Test]
        public void Barber_WithEmptyLastName_ShouldFailValidation()
        {
            _validBarber.LastName = string.Empty;
            var validationContext = new ValidationContext(_validBarber);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validBarber, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Barber with empty LastName should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Barber_WithLastNameLongerThan100Characters_ShouldFailValidation()
        {
            _validBarber.LastName = new string('A', 101);
            var validationContext = new ValidationContext(_validBarber);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validBarber, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Barber with LastName longer than 100 characters should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");

            Assert.That(validationResults[0].ErrorMessage.ToString() == "MaxLengthExceededErrorMessage");
        }

        [Test]
        public void Barber_WithNegativeExperienceYears_ShouldThrowException()
        {
            _validBarber.ExperienceYears = -1;
            Assert.Throws<ArgumentException>(() =>
            {
                if (_validBarber.ExperienceYears < 0)
                {
                    throw new ArgumentException("Experience years must be a non-negative number");
                }
            }, "Barber with negative experience years should throw an exception");
        }

        [Test]
        public void Barber_WithEmptyProfilePictureUrl_ShouldFailValidation()
        {
            _validBarber.ProfilePictureUrl = string.Empty;
            var validationContext = new ValidationContext(_validBarber);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validBarber, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Barber with empty ProfilePictureUrl should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Barber_WithDefaultGuid_ShouldHaveNewGuid()
        {
            var barber = new Barber();

            Assert.That(!Guid.Empty.Equals(barber.Id), "Barber Id should be a new Guid");
        }

        [Test]
        public void Barber_WithFeedbacks_ShouldSupportFeedbackCollection()
        {
            var feedback1 = new Feedback();
            var feedback2 = new Feedback();

            _validBarber.Feedbacks.Add(feedback1);
            _validBarber.Feedbacks.Add(feedback2);

            Assert.That(2.Equals(_validBarber.Feedbacks.Count), "Should support multiple feedbacks");
        }

        [Test]
        public void Barber_WithAppointments_ShouldSupportAppointmentCollection()
        {
            var appointment1 = new Appointment();
            var appointment2 = new Appointment();

            _validBarber.Appointments.Add(appointment1);
            _validBarber.Appointments.Add(appointment2);

            Assert.That(2.Equals(_validBarber.Appointments.Count), "Should support multiple appointments");
        }

        [Test]
        public void Barber_FullName_ShouldCombineFirstAndLastName()
        {
            _validBarber.FirstName = "John";
            _validBarber.LastName = "Doe";

            string fullName = $"{_validBarber.FirstName} {_validBarber.LastName}";

            Assert.That("John Doe".Equals(fullName), "Full name should combine first and last name");
        }

        [Test]
        public void Barber_CollectionsInitialization_ShouldBeEmpty()
        {
            var newBarber = new Barber();

            Assert.That(!newBarber.Feedbacks.Equals(null), "Feedbacks collection should not be null");
            Assert.That(!newBarber.Appointments.Equals(null), "Appointments collection should not be null");
            Assert.That(0.Equals(newBarber.Feedbacks.Count), "Feedbacks collection should be initially empty");
            Assert.That(0.Equals(newBarber.Appointments.Count), "Appointments collection should be initially empty");
        }
    }
}