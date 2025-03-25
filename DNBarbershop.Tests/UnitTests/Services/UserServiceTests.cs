using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using DNBarbershop.Core.Services;
using DNBarbershop.Core.IServices;
using DNBarbershop.DataAccess.Repository;
using DNBarbershop.Models.Entities;
namespace DNBarbershop.Tests.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IRepository<User>> _mockUserRepository;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IRepository<User>>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        public static IEnumerable<TestCaseData> ValidUserTestCases
        {
            get
            {
                yield return new TestCaseData(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "johndoe",
                    Email = "john@example.com"
                }).SetName("Add_ValidUser_WithFullDetails");

                yield return new TestCaseData(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Jane",
                    LastName = "Smith",
                    UserName = "janesmith"
                }).SetName("Add_ValidUser_WithMinimalDetails");
            }
        }

        [Test, TestCaseSource(nameof(ValidUserTestCases))]
        public async Task Add_ValidUser_ShouldCallRepositoryAddMethod(User user)
        {
            await _userService.Add(user);

            _mockUserRepository.Verify(r => r.Add(user), Times.Once);
        }

        public static IEnumerable<TestCaseData> InvalidUserTestCases
        {
            get
            {
                yield return new TestCaseData(null).SetName("Add_NullUser");
                yield return new TestCaseData(new User()).SetName("Add_EmptyUser");
            }
        }

        [Test, TestCaseSource(nameof(InvalidUserTestCases))]
        public void Add_InvalidUser_ShouldThrowException(User user)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.Add(user)
            );
        }
        public static IEnumerable<TestCaseData> DeleteUserTestCases
        {
            get
            {
                yield return new TestCaseData(Guid.NewGuid()).SetName("Delete_ValidGuid");
                yield return new TestCaseData(Guid.Empty).SetName("Delete_EmptyGuid");
            }
        }

        [Test, TestCaseSource(nameof(DeleteUserTestCases))]
        public async Task Delete_User_ShouldCallRepositoryDeleteMethod(Guid userId)
        {
            await _userService.Delete(userId);

            _mockUserRepository.Verify(r => r.Delete(userId), Times.Once);
        }

        public static IEnumerable<TestCaseData> DeleteAllTestCases
        {
            get
            {
                yield return new TestCaseData(5).SetName("DeleteAll_WhenUsersExist");
                yield return new TestCaseData(0).SetName("DeleteAll_WhenNoUsersExist");
            }
        }

        [Test, TestCaseSource(nameof(DeleteAllTestCases))]
        public async Task DeleteAll_TestScenarios(int userCount)
        {
            _mockUserRepository.Setup(r => r.GetCount()).ReturnsAsync(userCount);

            if (userCount > 0)
            {
                await _userService.DeleteAll();

                _mockUserRepository.Verify(r => r.DeleteAll(), Times.Once);
            }
            else
            {
                Assert.ThrowsAsync<ArgumentException>(async () =>
                    await _userService.DeleteAll()
                );
            }
        }
        public static IEnumerable<TestCaseData> GetUserTestCases
        {
            get
            {
                yield return new TestCaseData((Expression<Func<User, bool>>)(u => u.FirstName == "John"),
                              new User { FirstName = "John", LastName = "Doe" })
                              .SetName("GetUserByFirstName");

                yield return new TestCaseData((Expression<Func<User, bool>>)(u => u.Email == "john@example.com"),
                                              new User { Email = "john@example.com", FirstName = "John" })
                                              .SetName("GetUserByEmail");
            }
        }

        [Test, TestCaseSource(nameof(GetUserTestCases))]
        public async Task Get_WithFilter_ShouldReturnUser(
            Expression<Func<User, bool>> filter,
            User expectedUser)
        {
            _mockUserRepository.Setup(r => r.Get(filter)).ReturnsAsync(expectedUser);

            var result = await _userService.Get(filter);

            Assert.That(expectedUser.Equals(result));
            _mockUserRepository.Verify(r => r.Get(filter), Times.Once);
        }
        public static IEnumerable<TestCaseData> RemoveRangeTestCases
        {
            get
            {
                yield return new TestCaseData(new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), FirstName = "John" },
                new User { Id = Guid.NewGuid().ToString(), FirstName = "Jane" }
            }).SetName("RemoveRange_MultipleUsers");

                yield return new TestCaseData(new List<User>()).SetName("RemoveRange_EmptyList");
            }
        }

        [Test, TestCaseSource(nameof(RemoveRangeTestCases))]
        public async Task RemoveRange_TestScenarios(List<User> users)
        {
            if (users.Any())
            {
                await _userService.RemoveRange(users);

                _mockUserRepository.Verify(r => r.RemoveRange(users), Times.Once);
            }
            else
            {
                Assert.ThrowsAsync<ArgumentException>(async () =>
                    await _userService.RemoveRange(users)
                );
            }
        }
        public static IEnumerable<TestCaseData> UpdateUserTestCases
        {
            get
            {
                yield return new TestCaseData(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "johndoe",
                    Email = "john@example.com"
                }).SetName("Update_ValidUser_FullDetails");

                yield return new TestCaseData(new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "Jane",
                    LastName = "Smith"
                }).SetName("Update_ValidUser_MinimalDetails");
            }
        }

        [Test, TestCaseSource(nameof(UpdateUserTestCases))]
        public async Task Update_User_ShouldCallRepositoryUpdateMethod(User user)
        {
            await _userService.Update(user);

            _mockUserRepository.Verify(r => r.Update(user), Times.Once);
        }

        [Test]
        public void Update_NullUser_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.Update(null)
            );
        }
    }
}
