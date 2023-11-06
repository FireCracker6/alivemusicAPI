using Moq;
using CollaborateMusicAPI.Repositories;
using CollaborateMusicAPI.Services;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Enums;
using System.Linq.Expressions;
using CollaborateMusicAPI.Contexts;

namespace CollarobateMusicAPI.Test.UnitTests;

public class UserService_Tests
{
    private readonly Mock<IUsersRepository> _mockUserRepository;
    private readonly UserService _userService;  // Use a real instance, not a mock

    public UserService_Tests()  // Remove the parameter
    {
        _mockUserRepository = new Mock<IUsersRepository>();
      // _userService = new UserService(_mockUserRepository.Object);  // Inject the mock repository into the real service
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnServiceResponseWithStatusCode201_WhenUserCreatedSuccessfully()
    {
        // Arrange
        var newUserDto = new UserRegistrationDto
        {
            Email = "testingtest@example.com",
            Password = "BytMig123!",
            ConfirmPassword = "BytMig123!", 
            OAuthId = "default",
            OAuthProvider = "default",
            CreateDate = DateTime.UtcNow
        };

      
        _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new ApplicationUser());

       

        var request = new ServiceRequest<UserRegistrationDto>
        {
            Content = newUserDto
        };

        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CollaborateMusicAPI.Enums.StatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnServiceResponseWithStatusCode409_When_UserAlreadyExists()
    {
        // Arrange
        var newUserDto = new UserRegistrationDto
        {
            Email = "testingtestuser@example.com",
            Password = "BytMig123!",
            ConfirmPassword = "BytMig123!",
            OAuthId = null,
            OAuthProvider = null,
            CreateDate = DateTime.UtcNow
        };

        var request = new ServiceRequest<UserRegistrationDto>
        {
            Content = newUserDto
        };

       
        _mockUserRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).ReturnsAsync(true);




        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCode.Conflict, result.StatusCode);
    }



}
