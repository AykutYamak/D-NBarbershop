using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNBarbershop.Models.Entities;
using DNBarbershop.Common;
using Moq;
namespace DNBarbershop.Tests.Models
{
    [TestFixture]
    public class FeedbackModelTests
    {
        private Feedback _validFeedback;
        private User _testUser;
        private Barber _testBarber;

        [SetUp]
        public void Setup()
        {
            _testUser = new User
            {
                Id = "test-user-id",
                UserName = "testuser",
                FirstName = "Aykut",
                LastName = "Yamak"
            };

            _testBarber = new Barber
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                SpecialityId = Guid.NewGuid(),
                ProfilePictureUrl = "asdasd",
                ExperienceYears = 2
            };

            _validFeedback = new Feedback
            {
                UserId = _testUser.Id,
                User = _testUser,
                BarberId = _testBarber.Id,
                Barber = _testBarber,
                Rating = 4,
                Comment = "Great service!",
                FeedBackDate = DateTime.Now.Date
            };
        }

        [Test]
        public void Feedback_WithValidData_ShouldPassValidation()
        {
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

            Assert.That(isValid.Equals(true), "Feedback with valid data should pass validation");
            Assert.That(0.Equals(validationResults.Count), "No validation errors should be present");
        }

        [Test]
        public void Feedback_WithEmptyUserId_ShouldFailValidation()
        {
            _validFeedback.UserId = string.Empty;
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Feedback with empty UserId should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Feedback_WithRatingOutOfRange_ShouldFailValidation()
        {
            int[] invalidRatings = new[] { -1, 6 };

            foreach (var rating in invalidRatings)
            {
                _validFeedback.Rating = rating;
                var validationContext = new ValidationContext(_validFeedback);
                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

                Assert.That(isValid.Equals(false), $"Feedback with rating {rating} should fail validation");
                Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
                Assert.That("Rating should be in the range [0;5]".Equals(validationResults[0].ErrorMessage));
            }
        }

        [Test]
        public void Feedback_WithValidRatingRange_ShouldPass()
        {
            int[] validRatings = new[] { 0, 1, 2, 3, 4, 5 };

            foreach (var rating in validRatings)
            {
                _validFeedback.Rating = rating;
                var validationContext = new ValidationContext(_validFeedback);
                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

                Assert.That(isValid.Equals(true), $"Feedback with rating {rating} should pass validation");
            }
        }

        [Test]
        public void Feedback_WithCommentLongerThan1000Characters_ShouldFailValidation()
        {
            _validFeedback.Comment = new string('A', 1001);
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), "Feedback with comment longer than 1000 characters should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That("MaxLengthExceededErrorMessage".Equals(validationResults[0].ErrorMessage));
        }

        [Test]
        public void Feedback_WithDefaultGuid_ShouldHaveNewGuid()
        {
            var feedback = new Feedback();

            Assert.That(!Guid.Empty.Equals(feedback.Id), "Feedback Id should be a new Guid");
        }

        [Test]
        public void Feedback_DateFormatting_ShouldMatchSpecifiedFormat()
        {
            DateTime testDate = new DateTime(2023, 5, 15);
            _validFeedback.FeedBackDate = testDate;

            string formattedDate = testDate.ToString("yyyy-MM-dd");

            Assert.That("2023-05-15".Equals(formattedDate), "Date should be formatted as yyyy-MM-dd");
        }

        [Test]
        public void Feedback_WithNullComment_ShouldBeValid()
        {
            _validFeedback.Comment = null;
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

            Assert.That(isValid.Equals(true), "Feedback with null Comment should be valid");
        }

        [Test]
        public void Feedback_WithFutureDate_ShouldBeValid()
        {
            _validFeedback.FeedBackDate = DateTime.Now.Date.AddDays(1);
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

            Assert.That(isValid.Equals(true), "Feedback with future date should be valid");
        }

        [Test]
        public void Feedback_CommentLengthVariations_ShouldBeValid()
        {
            string[] commentVariations = new[]
            {
            null,
            "",
            "Short comment",
            new string('A', 1000)
        };

            foreach (var comment in commentVariations)
            {
                _validFeedback.Comment = comment;
                var validationContext = new ValidationContext(_validFeedback);
                var validationResults = new List<ValidationResult>();

                bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);
                Assert.That(isValid.Equals(true), $"Comment '{comment ?? "null"}' should be valid");
            }
        }
    }
}