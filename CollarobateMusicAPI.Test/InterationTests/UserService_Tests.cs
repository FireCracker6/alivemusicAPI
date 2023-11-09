using System;
using System.Linq.Expressions;
using CollaborateMusicAPI.Authentication;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using CollaborateMusicAPI.Services;
using CollarobateMusicAPI.Test.UnitTests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace CollarobateMusicAPI.Test.InterationTests;

public class UserService_Tests
{

    private readonly ApplicationDBContext _context;
    private readonly IUserService _userService;
    private readonly IUsersRepository _usersRepository;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;


    public UserService_Tests(IUsersRepository usersRepository = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDBContext(options);
        _usersRepository = new UsersRepository(_context);

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

       
        _mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                        .ReturnsAsync((string email) => _context.Users.FirstOrDefault(u => u.Email == email));
        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        var tokenServiceMock = new Mock<ITokenService>();
     

       
        var jwtSettings = Options.Create(new JwtSettings { Key = "4b623c772ff94971e1b1bb0723b2a0cb"  });
        var generateTokenService = new GenerateTokenService(jwtSettings);


        

            _userService = new UserService(
            _usersRepository,
            generateTokenService, 
            tokenServiceMock.Object, 
            _mockUserManager.Object); 
            this._usersRepository = usersRepository;
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
        Assert.IsType<ServiceResponse<UserWithTokenResponse>>(result);
        Assert.Equal(CollaborateMusicAPI.Enums.StatusCode.Created, result.StatusCode);
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

        ApplicationUser createdUser = new ApplicationUser
        {
            UserName = schema.Email,
            Email = schema.Email
        };

        // First creation
        _mockUserManager.Setup(um => um.FindByEmailAsync(schema.Email)).ReturnsAsync((ApplicationUser)null);
        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), schema.Password)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _userService.CreateAsync(request);

        // Setup FindByEmailAsync to return the created user for subsequent calls
        _mockUserManager.Setup(um => um.FindByEmailAsync(schema.Email)).ReturnsAsync(createdUser);

        // Act again for the second time
        var result = await _userService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CollaborateMusicAPI.Enums.StatusCode.Conflict, result.StatusCode);
        Assert.Equal(409, (int)result.StatusCode);
        Assert.Null(result.Content);
    }
}