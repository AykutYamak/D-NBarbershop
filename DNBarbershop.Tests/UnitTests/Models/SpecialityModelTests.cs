using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNBarbershop.Models.Entities;
using DNBarbershop.Common;

namespace DNBarbershop.Tests.Models
{
    [TestFixture]
    public class SpecialityModelTests
    {
        private Speciality _validSpeciality;


        [SetUp]
        public void Setup()
        {
            _validSpeciality = new Speciality
            {
                Type = "Шеф"
            };
        }

        [Test]
        public void Speciality_WithValidData_ShouldPassValidation()
        {
            var validationContext = new ValidationContext(_validSpeciality);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validSpeciality, validationContext, validationResults, true);

            Assert.That(isValid.Equals(true), "Speciality with valid data should pass validation");
            Assert.That(0.Equals(validationResults.Count), "No validation errors should be present");
        }

        [Test]
        public void Speciality_WithEmptyType_ShouldFailValidation()
        {
            _validSpeciality.Type = string.Empty;
            var validationContext = new ValidationContext(_validSpeciality);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validSpeciality, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Speciality with empty Type should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Speciality_WithNullType_ShouldFailValidation()
        {
            _validSpeciality.Type = null;
            var validationContext = new ValidationContext(_validSpeciality);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validSpeciality, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Speciality with null Type should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Speciality_WithTypeLongerThan20Characters_ShouldFailValidation()
        {
            _validSpeciality.Type = "ThisIsAVeryLongSpecialityTypeNameThatExceedsLimit";
            var validationContext = new ValidationContext(_validSpeciality);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validSpeciality, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Speciality with Type longer than 20 characters should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That("MaxLengthExceededErrorMessage".Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Speciality_WithDefaultGuid_ShouldHaveNewGuid()
        {
            var speciality = new Speciality();

            Assert.That(!Guid.Empty.Equals(speciality.Id), "Speciality Id should be a new Guid");
        }

        [Test]
        public void Speciality_WithBarbers_ShouldSupportBarberCollection()
        {
            var barber1 = new Barber { FirstName = "Barber ", LastName = "Barberov", SpecialityId = _validSpeciality.Id, ExperienceYears = 3, ProfilePictureUrl = "asd" };
            var barber2 = new Barber { FirstName = "Barber2", LastName = "Barberov2", SpecialityId = _validSpeciality.Id, ExperienceYears = 4, ProfilePictureUrl = "osdkfgsd" };

            _validSpeciality.Barbers.Add(barber1);
            _validSpeciality.Barbers.Add(barber2);

            Assert.That(2.Equals(_validSpeciality.Barbers.Count), "Should support multiple barbers");
        }

        [Test]
        public void Speciality_WithValidType_ShouldPreserveType()
        {
            string expectedType = "Fade";
            _validSpeciality.Type = expectedType;

            Assert.That(expectedType.Equals(_validSpeciality.Type), "Speciality Type should be preserved");
        }

        [Test]
        public void Speciality_BarbersCollection_ShouldBeInitializedAsEmpty()
        {
            var newSpeciality = new Speciality();

            Assert.That(!newSpeciality.Barbers.Equals(null), "Barbers collection should not be null");
            Assert.That(0.Equals(newSpeciality.Barbers.Count), "Barbers collection should be initially empty");
        }
    }
}