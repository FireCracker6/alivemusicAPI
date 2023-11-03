using System.ComponentModel.DataAnnotations;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CollarobateMusicAPI.Test.InterationTests;

public class UserRepository_Tests
{
   private readonly ApplicationDBContext _context;
    private readonly IUsersRepository _usersRepository;

    public UserRepository_Tests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDBContext(options);
      _usersRepository = new UsersRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "testuser@example.com",
            PasswordHash = "BytMig123!",
            OAuthId = null,
            OAuthProvider = null,
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var result = await _usersRepository.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationUser>(result);
        Assert.Equal(user.Email, result.Email);
        
       

    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_WhenEntityAlreadyExists()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "testuser@example.com",
            PasswordHash = "BytMig123!",
            OAuthId = null,
            OAuthProvider = null,
            CreatedDate = DateTime.UtcNow
        };
        await _usersRepository.CreateAsync(user);

        // Act
        var result = await _usersRepository.ExistsAsync(x => x.Email == user.Email);

        // Assert
        Assert.True(result);
        


    }
    [Fact]
    public async Task ExistsAsync_Should_ReturnFalse_WhenEntityAlreadyExists()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "testuser@example.com",
            PasswordHash = "BytMig123!",
            OAuthId = null,
            OAuthProvider = null,
            CreatedDate = DateTime.UtcNow
        };
        

        // Act
        var result = await _usersRepository.ExistsAsync(x => x.Email == user.Email);

        // Assert
        Assert.False(result);
        await DisposeAsync();


    }

   
    private async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        _context.Dispose();
    }
}
