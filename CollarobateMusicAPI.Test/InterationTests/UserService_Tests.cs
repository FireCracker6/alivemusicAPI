using System.Linq.Expressions;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using CollaborateMusicAPI.Services;
using CollarobateMusicAPI.Test.UnitTests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CollarobateMusicAPI.Test.InterationTests;

public class UserService_Tests
{

    private readonly ApplicationDBContext _context;
    private readonly IUserService _userService;
    private readonly IUsersRepository _usersRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;


    public UserService_Tests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDBContext(options);
        _usersRepository = new UsersRepository(_context);

        // Mocking the UserManager<TUser>
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // Mocking the ITokenService
        var tokenServiceMock = new Mock<ITokenService>();

        // Then, you create the GenerateTokenService mock with its dependencies
        var generateTokenServiceMock = new Mock<GenerateTokenService>(tokenServiceMock.Object);

        // Now, you can use generateTokenServiceMock.Object when you initialize UserService
        _userService = new UserService(
            _usersRepository,
            generateTokenServiceMock.Object,
            _mockTokenService.Object, // This is either a mock or the real ITokenService
            _mockUserManager.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnServiceResponseWithStatusResponse201_WhenCreatedSuccessfylly()
    {
        // Arrange
        var schema = new UserRegistrationDto
        {
            Email = "testuser@example.com",
            Password = "BytMig123!",
            OAuthId = null,
            OAuthProvider = null,
            CreateDate = DateTime.UtcNow
        };
        var request = new ServiceRequest<UserRegistrationDto> { Content = schema };



        // Act
        var result = await _userService.CreateAsync(request);



        // Assert
        Assert.NotNull(result.Content);
        Assert.IsType<ServiceResponse<ApplicationUser>>(result);
        Assert.Equal(201, (int)result.StatusCode);
        Assert.Equal(schema.Email, result.Content.User.Email);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnServiceResponseWithStatusResponse409_WhenUserAlreadyExists()
    {
        // Arrange
        var schema = new UserRegistrationDto
        {
            Email = "testuser@example.com",
            Password = "BytMig123!",
            OAuthId = null,
            OAuthProvider = null,
            CreateDate = DateTime.UtcNow
        };
        var request = new ServiceRequest<UserRegistrationDto> { Content = schema };
        await _userService.CreateAsync(request);


        // Act
        var result = await _userService.CreateAsync(request);



        // Assert
        Assert.NotNull(result);
        Assert.IsType<ServiceResponse<ApplicationUser>>(result);
        Assert.Equal(409, (int)result.StatusCode);
        Assert.Null(result.Content);
    }
}
