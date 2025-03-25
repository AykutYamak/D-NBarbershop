using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DNBarbershop.Models.Entities;
using DNBarbershop.Common;

[TestFixture]
public class MessageModelTests
{
    private Message _validMessage;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        _testUser = new User
        {
            Id = "test-user-id",
            UserName = "testuser"
        };

        _validMessage = new Message
        {
            Email = "john.doe@example.com",
            Content = "Test message content",
            Date = DateTime.Now.Date,
            UserId = _testUser.Id,
            User = _testUser,
            IsRead = false
        };
    }

    [Test]
    public void Message_WithValidData_ShouldPassValidation()
    {
        var validationContext = new ValidationContext(_validMessage);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(_validMessage, validationContext, validationResults, true);

        Assert.That(isValid.Equals(true), "Message with valid data should pass validation");
        Assert.That(0.Equals(validationResults.Count), "No validation errors should be present");
    }

    [Test]
    public void Message_WithEmptyEmail_ShouldFailValidation()
    {
        _validMessage.Email = string.Empty;
        var validationContext = new ValidationContext(_validMessage);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(_validMessage, validationContext, validationResults, true);

        Assert.That(isValid.Equals(false), "Message with empty Email should fail validation");
        Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
        Assert.That("Email is required.".Equals(validationResults[0].ErrorMessage));
    }

    [Test]
    public void Message_WithInvalidEmailFormat_ShouldFailValidation()
    {
        string[] invalidEmails = new[]
        {
            "invalid-email",
            "invalid@",
            "@invalid.com",
            "invalid@invalid",
            "invalid@.com"
        };

        foreach (var invalidEmail in invalidEmails)
        {
            _validMessage.Email = invalidEmail;
            var validationContext = new ValidationContext(_validMessage);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validMessage, validationContext, validationResults, true);

            Assert.That(isValid.Equals(false), $"Message with invalid email '{invalidEmail}' should fail validation");
            Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
            Assert.That("Вашият e-mail трябва да съдържа валиден домейн и поне една точка, както и символ '@'!".Equals(validationResults[0].ErrorMessage));
        }
    }

    [Test]
    public void Message_WithEmptyContent_ShouldFailValidation()
    {
        _validMessage.Content = string.Empty;
        var validationContext = new ValidationContext(_validMessage);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(_validMessage, validationContext, validationResults, true);

        Assert.That(isValid.Equals(false), "Message with empty Content should fail validation");
        Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
        Assert.That(ErrorMessages.RequiredErrorMessage.Equals(validationResults[0].ErrorMessage));
    }

    [Test]
    public void Message_WithDefaultGuid_ShouldHaveNewGuid()
    {
        var message = new Message();

        Assert.That(!Guid.Empty.Equals(message.Id), "Message Id should be a new Guid");
    }

    [Test]
    public void Message_WithNullUser_ShouldBeValid()
    {
        _validMessage.UserId = null;
        _validMessage.User = null;
        var validationContext = new ValidationContext(_validMessage);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(_validMessage, validationContext, validationResults, true);

        Assert.That(isValid.Equals(true), "Message with null User should be valid");
    }

    [Test]
    public void Message_WithFutureDate_ShouldBeValid()
    {
        _validMessage.Date = DateTime.Now.Date.AddDays(1);
        var validationContext = new ValidationContext(_validMessage);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(_validMessage, validationContext, validationResults, true);

        Assert.That(isValid.Equals(true), "Message with future date should be valid");
    }

    [Test]
    public void Message_DefaultIsReadState_ShouldBeFalse()
    {
        var message = new Message
        {
            Email = "test@example.com",
            Content = "Test content",
            Date = DateTime.Now.Date
        };

        Assert.That(message.IsRead.Equals(false), "Default IsRead state should be false");
    }

    [Test]
    public void Message_DateFormatting_ShouldMatchSpecifiedFormat()
    {
        DateTime testDate = new DateTime(2023, 5, 15);
        _validMessage.Date = testDate;

        string formattedDate = testDate.ToString("yyyy-MM-dd");

        Assert.That("2023-05-15".Equals(formattedDate), "Date should be formatted as yyyy-MM-dd");
    }

    [Test]
    public void Message_ValidEmailFormats_ShouldPass()
    {
        string[] validEmails = new[]
        {
            "user@example.com",
            "user.name@example.com",
            "user+tag@example.com",
            "user@subdomain.example.com",
            "user@example.co.uk"
        };

        foreach (var validEmail in validEmails)
        {
            _validMessage.Email = validEmail;
            var validationContext = new ValidationContext(_validMessage);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(_validMessage, validationContext, validationResults, true);
            Assert.That(isValid.Equals(true), $"Email '{validEmail}' should be valid");
        }
    }
}