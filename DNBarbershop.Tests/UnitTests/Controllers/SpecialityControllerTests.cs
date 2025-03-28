using NUnit.Framework;
using Moq;
using DNBarbershop.Core.IServices;
using DNBarbershop.Models.Entities;
using DNBarbershop.Models.ViewModels.Specialities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DNBarbershop.Controllers;
using DNBarbershop.Core.IService;
using DNBarbershop.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DNBarbershop.Tests.UnitTests.Controllers
{
    [TestFixture]
    public class SpecialityControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions;
        private ApplicationDbContext _context;
        private Mock<IBarberService> _barberServiceMock;
        private Mock<ISpecialityService> _specialityServiceMock;
        private SpecialityController _specialityController;

        [TearDown]
        public void TearDown()
        {
            _specialityController.Dispose();
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [SetUp]
        public void Setup()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DNBarbershopInMemoryDb" + Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(_dbOptions);
            _context.Database.EnsureCreated();
            DbSeeder.SeedDatabase(_context);

            _barberServiceMock = new Mock<IBarberService>();
            _specialityServiceMock = new Mock<ISpecialityService>();

            _specialityServiceMock.Setup(x => x.GetAll())
                .Returns(_context.speciality);

            _specialityServiceMock.Setup(x => x.Get(It.IsAny<Expression<Func<Speciality, bool>>>()))
                .Returns<Func<Speciality, bool>>(predicate => Task.FromResult(_context.speciality.FirstOrDefault(predicate)));

            _specialityController = new SpecialityController(_barberServiceMock.Object, _specialityServiceMock.Object);
        }


        [Test]
        public async Task Index_ReturnsViewResult_WithCorrectModel()
        {
            var specialities = new List<Speciality> { new Speciality { Id = Guid.NewGuid(), Type = "Haircut" } };
            _specialityServiceMock.Setup(s => s.GetAll()).Returns(specialities.AsQueryable());

            var result = await _specialityController.Index(null) as ViewResult;
            var model = result.Model as SpecialityViewModel;

            Assert.That(result!=null);
            Assert.That(model, Is.TypeOf<SpecialityViewModel>());
            Assert.That(1.Equals(model.Specialities.Count()));
        }

        [Test]
        public void Add_ReturnsViewResult_WithEmptyModel()
        {
            var result = _specialityController.Add() as ViewResult;

            Assert.That(result!=null);
            Assert.That(result.Model, Is.TypeOf<SpecialityCreateViewModel>());
        }

        [Test]
        public async Task Add_Post_RedirectsToIndex_WhenModelIsValid()
        {
            var model = new SpecialityCreateViewModel { Type = "Haircut" };
            _specialityServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<Speciality, bool>>>())).ReturnsAsync((Speciality)null);
            
            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["success"]).Returns("Успешно добавено ниво на специализиране.");
            _specialityController.TempData = tempData.Object;

            var result = await _specialityController.Add(model) as RedirectToActionResult;

            Assert.That(result != null);
            Assert.That("Index".Equals(result.ActionName.ToString()));
        }

        [Test]
        public async Task Edit_ReturnsViewResult_WhenIdIsValid()
        {
            var speciality = new Speciality { Id = Guid.NewGuid(), Type = "Haircut" };
            _specialityServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<Speciality, bool>>>())).ReturnsAsync(speciality);

            var result = await _specialityController.Edit(speciality.Id) as ViewResult;

            Assert.That(result != null);
            Assert.That(result.Model, Is.TypeOf<SpecialityEditViewModel>());
        }

        [Test]
        public async Task Edit_Post_RedirectsToIndex_WhenModelIsValid()
        {
            var speciality = new Speciality { Id = Guid.NewGuid(), Type = "Haircut" };
            _specialityServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<Speciality, bool>>>())).ReturnsAsync(speciality);

            var model = new SpecialityEditViewModel { Id = speciality.Id, Type = "Beard Trim" };

            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["success"]).Returns("Успешно редактирано ниво на специализиране.");
            _specialityController.TempData = tempData.Object;

            var result = await _specialityController.Edit(model) as RedirectToActionResult;

            Assert.That(result!=null);
            Assert.That("Index".Equals(result.ActionName.ToString()));
        }

        [Test]
        public async Task Delete_Post_RedirectsToIndex_WhenIdIsValid()
        {
            var speciality = new Speciality { Id = Guid.NewGuid(), Type = "Haircut" };
            _specialityServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<Speciality, bool>>>())).ReturnsAsync(speciality);
            
            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["success"]).Returns("Успешно изтрито ниво на специализиране.");
            _specialityController.TempData = tempData.Object;

            var result = await _specialityController.Delete(speciality.Id) as RedirectToActionResult;

            Assert.That(result != null);
            Assert.That("Index".Equals(result.ActionName.ToString()));
        }

        [Test]
        public async Task Delete_Post_ReturnsNotFound_WhenIdIsInvalid()
        {
            _specialityServiceMock.Setup(s => s.Get(It.IsAny<Expression<Func<Speciality, bool>>>())).ReturnsAsync((Speciality)null);

            var tempData = new Mock<ITempDataDictionary>();
            tempData.Setup(t => t["error"]).Returns("Няма намерено такова ниво на специализиране.");
            _specialityController.TempData = tempData.Object;

            var result = await _specialityController.Delete(Guid.NewGuid()) as NotFoundResult;

            Assert.That(result != null);
            Assert.That(404.Equals(result.StatusCode));
        }
    }
}
