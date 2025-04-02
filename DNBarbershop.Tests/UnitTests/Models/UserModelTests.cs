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
        private ValidationContext _validationContext;

        [SetUp]
        public void Setup()
        {
            _user = new User();
            _validationContext = new ValidationContext(_user, serviceProvider: null, items: null);
        }

        [Test]
        public void FirstName_WhenNull_ShouldFailValidation()
        {
            _user.FirstName = null;

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(false) || isValid == false);
            Assert.That(validationResults.Count, Is.GreaterThan(0));
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage));
        }

        [Test]
        public void FirstName_WhenEmptyString_ShouldFailValidation()
        {
            _user.FirstName = string.Empty;
            _user.LastName = "Doe"; 

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(false) || isValid == false);
            Assert.That(validationResults.Count, Is.GreaterThan(0));
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage));
        }

        [Test]
        public void FirstName_WhenTooLong_ShouldFailValidation()
        {
            _user.FirstName = new string('a', 101);
            _user.LastName = "Doe";

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(false) || isValid == false);
            Assert.That(validationResults.Count, Is.GreaterThan(0));
            Assert.That(validationResults[0].ErrorMessage.Equals("MaxLengthExceededErrorMessage"));
        }

        [Test]
        public void FirstName_WhenValidLength_ShouldPassValidation()
        {
            _user.FirstName = "John";
            _user.LastName = "Doe";

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(true) || isValid == true);
            Assert.That(validationResults.Count, Is.EqualTo(0));
        }

        [Test]
        public void LastName_WhenNull_ShouldFailValidation()
        {
            _user.LastName = null;

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(false) || isValid == false);
            Assert.That(validationResults.Count, Is.GreaterThan(0));
            Assert.That(validationResults[0].ErrorMessage.Equals("This field is required!"));
        }

        [Test]
        public void LastName_WhenEmptyString_ShouldFailValidation()
        {
            _user.FirstName = "John";
            _user.LastName = string.Empty;

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(false) || isValid == false);
            Assert.That(validationResults.Count, Is.GreaterThan(0));
            Assert.That(validationResults[0].ErrorMessage, Is.EqualTo(ErrorMessages.RequiredErrorMessage));
        }

        [Test]
        public void LastName_WhenTooLong_ShouldFailValidation()
        {
            _user.FirstName = "John";
            _user.LastName = new string('a', 101);

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(false) || isValid == false);
            Assert.That(validationResults.Count, Is.GreaterThan(0));
            Assert.That(validationResults[0].ErrorMessage.Equals("MaxLengthExceededErrorMessage"));
        }

        [Test]
        public void LastName_WhenValidLength_ShouldPassValidation()
        {
            _user.FirstName = "John";
            _user.LastName = "Doe";

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(_user, _validationContext, validationResults, validateAllProperties: true);

            Assert.That(isValid.Equals(true) || isValid == true);
            Assert.That(validationResults.Count, Is.EqualTo(0));
        }

        [Test]
        public void Collections_ShouldBeInitializedAsEmptyHashSet()
        {
            _user.FirstName = "John";
            _user.LastName = "Doe";

            Assert.That(_user.Appointments, Is.Not.Null);
            Assert.That(_user.Appointments, Is.InstanceOf<HashSet<Appointment>>());
            Assert.That(_user.Appointments.Count, Is.EqualTo(0));

            Assert.That(_user.Feedbacks, Is.Not.Null);
            Assert.That(_user.Feedbacks, Is.InstanceOf<HashSet<Feedback>>());
            Assert.That(_user.Feedbacks.Count, Is.EqualTo(0));

            Assert.That(_user.Messages, Is.Not.Null);
            Assert.That(_user.Messages, Is.InstanceOf<HashSet<Message>>());
            Assert.That(_user.Messages.Count, Is.EqualTo(0));
        }
    }
}