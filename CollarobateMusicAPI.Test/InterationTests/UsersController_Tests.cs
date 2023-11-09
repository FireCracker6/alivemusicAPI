using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Controllers;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using CollaborateMusicAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CollarobateMusicAPI.Test.InterationTests
{
    public class UsersController_Tests
    {
        private readonly Mock<IUserService> _mockUsersService;
        private readonly Mock<IExternalAuthService> _mockExternalAuthService;
        private readonly Mock<IGoogleTokenService> _mockGoogleTokenService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly UsersController _usersController;
        private readonly ApplicationDBContext _mockContext;

        public UsersController_Tests()
        {
            _mockUsersService = new Mock<IUserService>();
            _mockExternalAuthService = new Mock<IExternalAuthService>();
            _mockGoogleTokenService = new Mock<IGoogleTokenService>();
            _mockTokenService = new Mock<ITokenService>();
            // Set up the token service to return some dummy token when called
            _mockTokenService.Setup(t => t.GenerateTokenAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<bool>()))
                   .ReturnsAsync("dummy-token");


            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") 
                .Options;
            _mockContext = new ApplicationDBContext(options);


            _usersController = new UsersController(
                _mockUsersService.Object,
                _mockGoogleTokenService.Object,
                _mockContext,
                _mockTokenService.Object);
        }

        [Fact]
        public async Task Create_Should_ReturnConflict_When_UserAlreadyExists()
        {
            // Arrange
            var schema = new UserRegistrationDto()
            {
                Email = "alive@domain.com",
                Password = "BytMig123!",
                OAuthId = null,
                OAuthProvider = null,
                CreateDate = DateTime.UtcNow
            };

            // Mock GetUserByEmailAsync to simulate a user already exists
            _mockUsersService.Setup(service => service.GetUserByEmailAsync(schema.Email))
                             .ReturnsAsync(new ServiceResponse<ApplicationUser>
                             {
                                 Content = new ApplicationUser { Email = schema.Email }
                             });

            // Act
            var result = await _usersController.Register(schema);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Register_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var invalidSchema = new UserRegistrationDto()
            {
                Email = "sdf", // Invalid email.
                Password = "somePassword",
                ConfirmPassword = "differentPassword", // Not matching the actual password.
                OAuthId = null,
                OAuthProvider = null,
                CreateDate = DateTime.UtcNow
            };

            _usersController.ModelState.AddModelError("Email", "Invalid email address format");
            _usersController.ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password must match");

            // Act
            var result = await _usersController.Register(invalidSchema);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }



    }
}
