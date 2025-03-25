//using NUnit.Framework;
//using System;
//using DNBarbershop.Models.Entities;
//using DNBarbershop.Models.EnumClasses;
//using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;
//using DNBarbershop.Core.IService;
//using Moq;

//[TestFixture]
//public class AppointmentModelTests
//{
//    private Appointment _validAppointment;
//    private User _testUser;
//    private Barber _testBarber;
//    private Mock<ISpecialityService> _specialityService;

//    [SetUp]
//    public void Setup()
//    {
//        // Create a valid test user and barber
//        _testUser = new User
//        {
//            Id = Guid.NewGuid().ToString(),
//            FirstName = "Test",
//            LastName = "Testov"
//        };

//        _testBarber = new Barber
//        {
//            Id = Guid.NewGuid(),
//            FirstName = "Ivan",
//            LastName = "Ivanov",
//            SpecialityId = _specialityService.Object.Get(x => x.Type == "Шеф").Result.Id,
//            ExperienceYears = 3,
//            ProfilePictureUrl = "asd"
//        };


//        // Create a valid appointment
//        _validAppointment = new Appointment
//        {
//            UserId = _testUser.Id,
//            User = _testUser,
//            BarberId = _testBarber.Id,
//            Barber = _testBarber,
//            AppointmentDate = DateTime.Now.Date.AddDays(1),
//            AppointmentTime = new TimeSpan(14, 0, 0), // 2:00 PM
//            Status = AppointmentStatus.Scheduled
//        };
//    }

//    [Test]
//    public void Appointment_WithValidData_ShouldPassValidation()
//    {
//        // Arrange
//        var validationContext = new ValidationContext(_validAppointment);
//        var validationResults = new List<ValidationResult>();

//        // Act
//        bool isValid = Validator.TryValidateObject(_validAppointment, validationContext, validationResults, true);

//        // Assert
//        Assert.That(isValid.Equals("Appointment with valid data should pass validation"));
//        Assert.That(0.Equals(validationResults.Count),"No validation errors should be present");
//    }

//    [Test]
//    public void Appointment_WithEmptyUserId_ShouldFailValidation()
//    {
//        // Arrange
//        _validAppointment.UserId = string.Empty;
//        var validationContext = new ValidationContext(_validAppointment);
//        var validationResults = new List<ValidationResult>();

//        // Act
//        bool isValid = Validator.TryValidateObject(_validAppointment, validationContext, validationResults, true);

//        // Assert
//        Assert.That(isValid.Equals(false),"Appointment with empty UserId should fail validation");
//        Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
//        Assert.That(validationResults[0].ErrorMessage.Contains("This field is required").Equals(true));
//    }

//    [Test]
//    public void Appointment_WithNullUserId_ShouldFailValidation()
//    {
//        // Arrange
//        _validAppointment.UserId = null;
//        var validationContext = new ValidationContext(_validAppointment);
//        var validationResults = new List<ValidationResult>();

//        // Act
//        bool isValid = Validator.TryValidateObject(_validAppointment, validationContext, validationResults, true);

//        // Assert
//        Assert.That(isValid.Equals("Appointment with null UserId should fail validation"));
//        Assert.That(1.Equals(validationResults.Count), "Should have one validation error");
//        Assert.That(validationResults[0].ErrorMessage.Contains("This field is required").Equals(true));
//    }

//    [Test]
//    public void Appointment_WithPastDate_ShouldBeInvalid()
//    {
//        // Arrange
//        _validAppointment.AppointmentDate = DateTime.Now.Date.AddDays(-1);

//        // Act & Assert
//        Assert.Throws<ArgumentException>(() =>
//        {
//            if (_validAppointment.AppointmentDate < DateTime.Now.Date)
//            {
//                throw new ArgumentException("Appointment date cannot be in the past");
//            }
//        }, "Appointment with past date should throw an exception");
//    }

//    [Test]
//    public void Appointment_WithDefaultGuid_ShouldHaveNewGuid()
//    {
//        // Arrange
//        var appointment = new Appointment();

//        // Assert
//        Assert.That(!Guid.Empty.Equals(appointment.Id), "Appointment Id should be a new Guid");
//    }

//    [Test]
//    public void Appointment_WithMultipleServices_ShouldSupportServiceCollection()
//    {
//        // Arrange
//        var service1 = new AppointmentServices();
//        var service2 = new AppointmentServices();

//        // Act
//        _validAppointment.AppointmentServices.Add(service1);
//        _validAppointment.AppointmentServices.Add(service2);

//        // Assert
//        Assert.That(2.Equals(_validAppointment.AppointmentServices.Count), "Should support multiple appointment services");
//    }

//    [Test]
//    public void Appointment_WithInvalidStatus_ShouldBeInvalid()
//    {
//        // Arrange
//        var invalidStatusAppointment = new Appointment
//        {
//            UserId = _testUser.Id,
//            BarberId = _testBarber.Id,
//            AppointmentDate = DateTime.Now.Date.AddDays(1),
//            AppointmentTime = new TimeSpan(14, 0, 0),
//            Status = (AppointmentStatus)999 // Invalid enum value
//        };

//        var validationContext = new ValidationContext(invalidStatusAppointment);
//        var validationResults = new List<ValidationResult>();

//        // Act
//        bool isValid = Validator.TryValidateObject(invalidStatusAppointment, validationContext, validationResults, true);

//        // Assert
//        Assert.That(isValid.Equals(false),"Appointment with invalid status should fail validation");
//    }
//}
