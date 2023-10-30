using CollaborateMusicAPI.Controllers;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.Frameworks;

namespace CollarobateMusicAPI.Test.UnitTests;

public class UsersController_Tests
{
    private readonly Mock<IUserService> _mockUsersService;
    private readonly Mock<IExternalAuthService> _mockExternalAuthService; // Mock the external auth service
    private readonly UsersController _usersController;

//    public UsersController_Tests()
//    {
//        _mockUsersService = new Mock<IUserService>();
//        _mockExternalAuthService = new Mock<IExternalAuthService>(); // Initialize the mock

//        // Now pass both mocks to the UsersController constructor
//        _usersController = new UsersController(_mockUsersService.Object, _mockExternalAuthService.Object);
//    }


//[Fact]
//    public async Task CreateAsync_Should_ReturnBadRequest_WhenModelStateIsInvalid()
//    {
//        // Arrange
//        var schema = new UserRegistrationDto();
//        _usersController.ModelState.AddModelError("Email", "Email is required");

//        // Act
//        var result = await _usersController.Register(schema);

//        // Assert
//        Assert.IsType<BadRequestObjectResult>(result);


//    }


//    [Fact]
//    public async Task CreateAsync_Should_ReturnConflict_When_UserEmail_AlreadyExists()
//    {
//        // Arrange
//        var schema = new UserRegistrationDto();
//        var existingUserResponse = new ServiceResponse<Users>
//        {
//            Content = new Users()
//        };
//        _mockUsersService.Setup(x => x.GetUserByEmailAsync(schema.Email)).ReturnsAsync(existingUserResponse);

//        // Act
//        var result = await _usersController.Register(schema) as ObjectResult;

//        // Assert
//        Assert.NotNull(result);
//        Assert.IsType<ConflictObjectResult>(result); 
//        var conflictResult = result as ConflictObjectResult;
//        Assert.Equal(409, conflictResult?.StatusCode);
//    }
   






}
