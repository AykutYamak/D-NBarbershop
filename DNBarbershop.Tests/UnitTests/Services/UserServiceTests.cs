using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using DNBarbershop.Core.Services;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;

namespace DNBarbershop.Tests.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IRepository<User>> _mockUserRepository;
        private UserService _userService;
        private User _testUser;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IRepository<User>>();
            _userService = new UserService(_mockUserRepository.Object);

            _testUser = new User
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };
        }

        [Test]
        public async Task Add_ValidUser_ShouldCallRepositoryAdd()
        {
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            await _userService.Add(_testUser);

            _mockUserRepository.Verify(repo => repo.Add(_testUser), Times.Once);
        }

        [Test]
        public async Task Delete_ValidId_ShouldCallRepositoryDelete()
        {
            Guid userId = Guid.NewGuid();
            _mockUserRepository.Setup(repo => repo.Delete(userId))
                .Returns(Task.CompletedTask);

            await _userService.Delete(userId);

            _mockUserRepository.Verify(repo => repo.Delete(userId), Times.Once);
        }

        [Test]
        public async Task DeleteStringId_ValidId_ShouldCallRepositoryDeleteStringId()
        {
            Guid userId = Guid.NewGuid();
            _mockUserRepository.Setup(repo => repo.DeleteStringId(userId))
                .Returns(Task.CompletedTask);

            await _userService.DeleteStringId(userId);

            _mockUserRepository.Verify(repo => repo.DeleteStringId(userId), Times.Once);
        }

        [Test]
        public async Task DeleteAll_WithExistingUsers_ShouldCallRepositoryDeleteAll()
        {
            _mockUserRepository.Setup(repo => repo.GetCount())
                .ReturnsAsync(5);
            _mockUserRepository.Setup(repo => repo.DeleteAll())
                .Returns(Task.CompletedTask);

            await _userService.DeleteAll();

            _mockUserRepository.Verify(repo => repo.DeleteAll(), Times.Once);
        }

        [Test]
        public async Task DeleteAll_NoUsers_ShouldThrowArgumentException()
        {
            _mockUserRepository.Setup(repo => repo.GetCount())
                .ReturnsAsync(0);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.DeleteAll()
            );
            Assert.That(ex.Message, Is.EqualTo("Nothing to delete here."));
        }

        [Test]
        public async Task Get_WithFilter_ShouldReturnUser()
        {
            Expression<Func<User, bool>> filter = u => u.Email == "test@example.com";
            _mockUserRepository.Setup(repo => repo.Get(filter))
                .ReturnsAsync(_testUser);

            var result = await _userService.Get(filter);

            Assert.That(_testUser.Equals(result));
            _mockUserRepository.Verify(repo => repo.Get(filter), Times.Once);
        }

        [Test]
        public void GetAll_ShouldReturnQueryableUsers()
        {
            var users = new List<User> { _testUser }.AsQueryable();
            _mockUserRepository.Setup(repo => repo.GetAll())
                .Returns(users);

            var result = _userService.GetAll();

            Assert.That(users.Equals(result));
            _mockUserRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Test]
        public async Task RemoveRange_ValidUsers_ShouldCallRepositoryRemoveRange()
        {
            var users = new List<User> { _testUser, new User() };
            _mockUserRepository.Setup(repo => repo.RemoveRange(users))
                .Returns(Task.CompletedTask);

            await _userService.RemoveRange(users);

            _mockUserRepository.Verify(repo => repo.RemoveRange(users), Times.Once);
        }


        [Test]
        public async Task Update_ValidUser_ShouldCallRepositoryUpdate()
        {
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            await _userService.Update(_testUser);

            _mockUserRepository.Verify(repo => repo.Update(_testUser), Times.Once);
        }
    }
}