using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Controllers;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Frameworks;

namespace CollarobateMusicAPI.Test.UnitTests;

public class UsersController_Tests
{
    private readonly Mock<IUserService> _mockUsersService;
    private readonly Mock<IExternalAuthService> _mockExternalAuthService; // Mock the external auth service
    private readonly UsersController _usersController;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IGoogleTokenService> _mockGoogleTokenService;

    public UsersController_Tests()
    {
        _mockUsersService = new Mock<IUserService>();
        _mockExternalAuthService = new Mock<IExternalAuthService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockGoogleTokenService = new Mock<IGoogleTokenService>();

        // Mock the ApplicationDBContext using an in-memory database or a mock, depending on your testing strategy
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // You need to include the Microsoft.EntityFrameworkCore.InMemory package
            .Options;
        var context = new ApplicationDBContext(options);

        _usersController = new UsersController(
            _mockUsersService.Object,
            _mockGoogleTokenService.Object,
            context,
            _mockTokenService.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var schema = new UserRegistrationDto();
        _usersController.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _usersController.Register(schema);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);


    }


    [Fact]
    public async Task CreateAsync_Should_ReturnConflict_When_UserEmail_AlreadyExists()
    {
        // Arrange
        var schema = new UserRegistrationDto();
        var existingUserResponse = new ServiceResponse<ApplicationUser>
        {
            Content = new ApplicationUser()
        };
        _mockUsersService.Setup(x => x.GetUserByEmailAsync(schema.Email)).ReturnsAsync(existingUserResponse);

        // Act
        var result = await _usersController.Register(schema) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ConflictObjectResult>(result);
        var conflictResult = result as ConflictObjectResult;
        Assert.Equal(409, conflictResult?.StatusCode);
    }







}
