using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SharedViewModal.EnumModel;
using SharedViewModal.RequestViewModal;
using SharedViewModal.ViewModels;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Services;

namespace Squido.Test.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetUserByEmailAndPasswordAsync_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Email = email,
                Password = hashedPassword,
                FirstName = "Test",
                LastName = "User",
                IsDeleted = false,
                Role = new Role { Id = 1, RoleName = "Customer" }
            };
            
            var userViewModel = new UserViewModel 
            { 
                Id = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = new RoleViewModel { Id = 1, RoleName = "Customer" }
            };

            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetSingleWithIncludeAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);
            _mockMapper.Setup(m => m.Map<UserViewModel>(It.IsAny<User>())).Returns(userViewModel);

            // Act
            var result = await _userService.GetUserByEmailAndPasswordAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id.ToString(), result.Id);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAndPasswordAsync_WithInvalidPassword_ShouldReturnEmptyViewModel()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var wrongPassword = "wrongpassword";
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
            
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Email = email,
                Password = hashedPassword,
                IsDeleted = false,
                Role = new Role { Id = 1, RoleName = "Customer" }
            };

            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetSingleWithIncludeAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);

            // Act
            var result = await _userService.GetUserByEmailAndPasswordAsync(email, wrongPassword);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(default, result.Id);
            Assert.Null(result.Email);
        }

        [Fact]
        public async Task CreateUserAsync_WithNewEmail_ShouldCreateUser()
        {
            // Arrange
            var registerRequest = new RegisterRequestVm
            {
                Email = "new@example.com",
                Username = "newuser",
                FirstName = "New",
                LastName = "User",
                Password = "password123",
                RoleId = 1,
                Gender = GenderEnum.Male
            };

            var user = new User
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                RoleId = registerRequest.RoleId,
                Gender = (WebApplication1.Models.enums.GenderEnum)registerRequest.Gender
            };

            var users = new List<User>();
            
            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetAll()).Returns(users.AsQueryable());
            mockUserRepo.Setup(repo => repo.Insert(It.IsAny<User>())).Callback<User>(u => users.Add(u));
            
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.Save());
            
            _mockMapper.Setup(m => m.Map<User>(It.IsAny<RegisterRequestVm>())).Returns(user);

            // Act
            var result = await _userService.CreateUserAsync(registerRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("User Created", result.Message);
            Assert.Equal(registerRequest, result.Data);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
            mockUserRepo.Verify(repo => repo.Insert(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingEmail_ShouldReturnError()
        {
            // Arrange
            var existingEmail = "existing@example.com";
            var registerRequest = new RegisterRequestVm
            {
                Email = existingEmail,
                Username = "existinguser",
                FirstName = "Existing",
                LastName = "User",
                Password = "password123",
                RoleId = 1
            };

            var existingUser = new User
            {
                Email = existingEmail,
                IsDeleted = false
            };

            var users = new List<User> { existingUser };
            
            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetAll()).Returns(users.AsQueryable());
            
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);

            // Act
            var result = await _userService.CreateUserAsync(registerRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User with the same email already exists", result.Message);
            Assert.Equal("USER_EXISTS", result.ErrorCode);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Never);
            mockUserRepo.Verify(repo => repo.Insert(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = new Role { Id = 1, RoleName = "Customer" }
            };

            var userViewModel = new UserViewModel
            {
                Id = userId.ToString(),
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = new RoleViewModel { Id = 1, RoleName = "Customer" }
            };

            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetSingleWithIncludeAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(user);

            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);
            _mockMapper.Setup(m => m.Map<UserViewModel>(It.IsAny<User>())).Returns(userViewModel);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("User Found", result.Message);
            Assert.Equal(userId.ToString(), result.Data.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetSingleWithIncludeAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync((User)null);

            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.Message);
            Assert.Equal("USER_NOT_FOUND", result.ErrorCode);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ShouldUpdateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userViewModel = new UserViewModel
            {
                Id = userId.ToString(),
                Email = "update@example.com",
                FirstName = "Updated",
                LastName = "User",
                Phone = "1234567890"
            };

            var existingUser = new User
            {
                Id = userId,
                Email = "original@example.com",
                FirstName = "Original",
                LastName = "User",
                RoleId = 1,
                Gender = (WebApplication1.Models.enums.GenderEnum)GenderEnum.Male,
                IsDeleted = false,
                Role = new Role { Id = 1, RoleName = "Customer" }
            };

            var users = new List<User> { existingUser };
            
            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetAll()).Returns(users.AsQueryable());
            mockUserRepo.Setup(repo => repo.GetSingleWithIncludeAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(existingUser);
            mockUserRepo.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.Save());
            
            _mockMapper.Setup(m => m.Map(It.IsAny<UserViewModel>(), It.IsAny<User>(), It.IsAny<Action<IMappingOperationOptions<UserViewModel, User>>>()))
                .Callback<UserViewModel, User, Action<IMappingOperationOptions<UserViewModel, User>>>((src, dest, opt) => {
                    dest.Email = src.Email;
                    dest.FirstName = src.FirstName;
                    dest.LastName = src.LastName;
                    dest.Phone = src.Phone;
                });

            // Act
            var result = await _userService.UpdateUserAsync(userViewModel, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("User Updated", result.Message);
            Assert.Equal(userViewModel, result.Data);
            mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WithInvalidId_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userViewModel = new UserViewModel
            {
                Id = userId.ToString(),
                Email = "nonexistent@example.com"
            };

            var users = new List<User>();
            
            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetAll()).Returns(users.AsQueryable());
            
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);

            // Act
            var result = await _userService.UpdateUserAsync(userViewModel, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found", result.Message);
            Assert.Equal("USER_NOT_FOUND", result.ErrorCode);
            mockUserRepo.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Never);
        }

        [Fact]
        public async Task GetAllUser_WithoutKeyword_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "user1@example.com", FirstName = "User", LastName = "One", IsDeleted = false, Role = new Role() },
                new User { Id = Guid.NewGuid(), Email = "user2@example.com", FirstName = "User", LastName = "Two", IsDeleted = false, Role = new Role() },
                new User { Id = Guid.NewGuid(), Email = "user3@example.com", FirstName = "User", LastName = "Three", IsDeleted = false, Role = new Role() }
            };

            var userViewModels = users.Select(u => new UserViewModel 
            { 
                Id = u.Id.ToString(), 
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).ToList();

            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetAllWithIncludeAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(users);
            
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);
            _mockMapper.Setup(m => m.Map<List<UserViewModel>>(It.IsAny<List<User>>())).Returns(userViewModels);

            // Act
            var result = await _userService.GetAllUser(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Data.Count);
        }

        [Fact]
        public async Task GetAllUser_WithKeyword_ShouldReturnFilteredUsers()
        {
            // Arrange
            var keyword = "two";
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "user1@example.com", FirstName = "User", LastName = "One", IsDeleted = false, Role = new Role() },
                new User { Id = Guid.NewGuid(), Email = "user2@example.com", FirstName = "User", LastName = "Two", IsDeleted = false, Role = new Role() },
                new User { Id = Guid.NewGuid(), Email = "user3@example.com", FirstName = "User", LastName = "Three", IsDeleted = false, Role = new Role() }
            };

            var filteredUserViewModels = new List<UserViewModel>
            {
                new UserViewModel { Id = users[1].Id.ToString(), Email = users[1].Email, FirstName = users[1].FirstName, LastName = users[1].LastName }
            };

            var mockUserRepo = new Mock<IGenericRepository<User>>();
            mockUserRepo.Setup(repo => repo.GetAllWithIncludeAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Expression<Func<User, object>>>()))
                .ReturnsAsync(users);
            
            _mockUnitOfWork.Setup(uow => uow.UserRepository).Returns(mockUserRepo.Object);
            _mockMapper.Setup(m => m.Map<List<UserViewModel>>(It.IsAny<List<User>>())).Returns(filteredUserViewModels);

            // Act
            var result = await _userService.GetAllUser(keyword);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Data);
            Assert.Contains(result.Data, u => u.LastName == "Two");
        }
}