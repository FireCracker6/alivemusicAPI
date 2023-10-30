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
        private readonly Mock<IExternalAuthService> _mockExternalAuthService; // Mock the external auth service
        private readonly UsersController _usersController;

        public UsersController_Tests()
        {
            _mockUsersService = new Mock<IUserService>();
            _mockExternalAuthService = new Mock<IExternalAuthService>(); // Initialize the mock

            // Now pass both mocks to the UsersController constructor
            _usersController = new UsersController(_mockUsersService.Object, _mockExternalAuthService.Object);
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
                             .ReturnsAsync(new ServiceResponse<Users>
                             {
                                 Content = new Users { Email = schema.Email }
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
