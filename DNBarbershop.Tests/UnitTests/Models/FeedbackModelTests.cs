using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNBarbershop.Models.Entities;
using DNBarbershop.Common;

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
            UserName = "testuser"
        };

        _testBarber = new Barber
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
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
        // Arrange
        var validationContext = new ValidationContext(_validFeedback);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

        // Assert
        Assert.IsTrue(isValid, "Feedback with valid data should pass validation");
        Assert.AreEqual(0, validationResults.Count, "No validation errors should be present");
    }

    [Test]
    public void Feedback_WithEmptyUserId_ShouldFailValidation()
    {
        // Arrange
        _validFeedback.UserId = string.Empty;
        var validationContext = new ValidationContext(_validFeedback);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

        // Assert
        Assert.IsFalse(isValid, "Feedback with empty UserId should fail validation");
        Assert.AreEqual(1, validationResults.Count, "Should have one validation error");
        Assert.AreEqual(ErrorMessages.RequiredErrorMessage, validationResults[0].ErrorMessage);
    }

    [Test]
    public void Feedback_WithRatingOutOfRange_ShouldFailValidation()
    {
        // Arrange
        int[] invalidRatings = new[] { -1, 6 };

        foreach (var rating in invalidRatings)
        {
            _validFeedback.Rating = rating;
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

            // Assert
            Assert.IsFalse(isValid, $"Feedback with rating {rating} should fail validation");
            Assert.AreEqual(1, validationResults.Count, "Should have one validation error");
            Assert.AreEqual("Rating should be in the range [0;5]", validationResults[0].ErrorMessage);
        }
    }

    [Test]
    public void Feedback_WithValidRatingRange_ShouldPass()
    {
        // Arrange
        int[] validRatings = new[] { 0, 1, 2, 3, 4, 5 };

        foreach (var rating in validRatings)
        {
            _validFeedback.Rating = rating;
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

            // Assert
            Assert.IsTrue(isValid, $"Feedback with rating {rating} should pass validation");
        }
    }

    [Test]
    public void Feedback_WithCommentLongerThan1000Characters_ShouldFailValidation()
    {
        // Arrange
        _validFeedback.Comment = new string('A', 1001);
        var validationContext = new ValidationContext(_validFeedback);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

        // Assert
        Assert.IsFalse(isValid, "Feedback with comment longer than 1000 characters should fail validation");
        Assert.AreEqual(1, validationResults.Count, "Should have one validation error");
        Assert.AreEqual(ErrorMessages.MaxLengthExceededErrorMessage(1000), validationResults[0].ErrorMessage);
    }

    [Test]
    public void Feedback_WithDefaultGuid_ShouldHaveNewGuid()
    {
        // Arrange
        var feedback = new Feedback();

        // Assert
        Assert.AreNotEqual(Guid.Empty, feedback.Id, "Feedback Id should be a new Guid");
    }

    [Test]
    public void Feedback_DateFormatting_ShouldMatchSpecifiedFormat()
    {
        // Arrange
        DateTime testDate = new DateTime(2023, 5, 15);
        _validFeedback.FeedBackDate = testDate;

        // Act
        string formattedDate = testDate.ToString("yyyy-MM-dd");

        // Assert
        Assert.AreEqual("2023-05-15", formattedDate, "Date should be formatted as yyyy-MM-dd");
    }

    [Test]
    public void Feedback_WithNullComment_ShouldBeValid()
    {
        // Arrange
        _validFeedback.Comment = null;
        var validationContext = new ValidationContext(_validFeedback);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

        // Assert
        Assert.IsTrue(isValid, "Feedback with null Comment should be valid");
    }

    [Test]
    public void Feedback_WithFutureDate_ShouldBeValid()
    {
        // Arrange
        _validFeedback.FeedBackDate = DateTime.Now.Date.AddDays(1);
        var validationContext = new ValidationContext(_validFeedback);
        var validationResults = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);

        // Assert
        Assert.IsTrue(isValid, "Feedback with future date should be valid");
    }

    [Test]
    public void Feedback_CommentLengthVariations_ShouldBeValid()
    {
        // Arrange
        string[] commentVariations = new[]
        {
            null,
            "",
            "Short comment",
            new string('A', 1000)
        };

        // Act & Assert
        foreach (var comment in commentVariations)
        {
            _validFeedback.Comment = comment;
            var validationContext = new ValidationContext(_validFeedback);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validFeedback, validationContext, validationResults, true);
            Assert.IsTrue(isValid, $"Comment '{comment ?? "null"}' should be valid");
        }
    }
}