using Moq;
using CollaborateMusicAPI.Repositories;
using CollaborateMusicAPI.Services;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Enums;
using System.Linq.Expressions;
using CollaborateMusicAPI.Contexts;
using Microsoft.AspNetCore.Identity;
using CollaborateMusicAPI.Authentication;
using Microsoft.Extensions.Options;

namespace CollarobateMusicAPI.Test.UnitTests;

public class UserService_Tests
{
    private readonly Mock<IUsersRepository> _mockUserRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

    private readonly UserService _userService;
    private readonly GenerateTokenService _generateTokenService;


    public UserService_Tests()
    {
        // Initialize the mocks
        _mockUserRepository = new Mock<IUsersRepository>();
        _mockUserRepository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).ReturnsAsync(true);

        _mockTokenService = new Mock<ITokenService>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

        // Setup mocked JWT settings
        var jwtSettings = new JwtSettings { Key = "4b623c772ff94971e1b1bb0723b2a0cb" };
        var jwtOptions = Options.Create(jwtSettings);

        // Create an instance of GenerateTokenService with mocked settings
        _generateTokenService = new GenerateTokenService(jwtOptions);


        // Create an instance of UserService with the mocked dependencies
        _userService = new UserService(
            _mockUserRepository.Object, 
            _generateTokenService,      
            _mockTokenService.Object,   
            _mockUserManager.Object     
        );
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


        _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync((ApplicationUser)null!); 

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);




        var request = new ServiceRequest<UserRegistrationDto>
        {
            Content = newUserDto
        };

        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCode.Created, result.StatusCode);
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

        // Set up the UserManager mock to return a user, simulating that the user already exists
        _mockUserManager.Setup(um => um.FindByEmailAsync(newUserDto.Email))
                  .ReturnsAsync(new ApplicationUser() { Email = newUserDto.Email });

      

        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCode.Conflict, result.StatusCode);
    }

}