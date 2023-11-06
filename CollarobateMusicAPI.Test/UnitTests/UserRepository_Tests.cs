using System.Linq.Expressions;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using Moq;

namespace CollarobateMusicAPI.Test.UnitTests;

public class UserRepository_Tests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;

    public UserRepository_Tests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
    }

    [Fact]  
    public async Task CreatAsync_Should_ReturnUserEntity_When_CreatedSuccessfully()
    {
        // Arrange  
        var entity = new ApplicationUser() { Email = "testuser@example.com", PasswordHash = "BytMig123!", OAuthId = null, OAuthProvider = null, CreatedDate = DateTime.UtcNow };

        _usersRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(entity);    

        // Act
        var result = await _usersRepositoryMock.Object.CreateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ApplicationUser>(result);
        Assert.Equal(entity.Email, result.Email);
    }
    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_When_EntityAlreadyExists()
    {
        // Arrange  
        var entity = new ApplicationUser() { Email = "testuser@example.com", PasswordHash = "BytMig123!", OAuthId = null, OAuthProvider = null, CreatedDate = DateTime.UtcNow };

        _usersRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).ReturnsAsync(true);
   

        // Act
        var result = await _usersRepositoryMock.Object.ExistsAsync(x => x.Email == "testuser@example.com");

        // Assert
       Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnFalse_When_EntityDoesNotExists()
    {
        // Arrange  
        var entity = new ApplicationUser() { Email = "testuser@example.com", PasswordHash = "BytMig123!", OAuthId = null, OAuthProvider = null, CreatedDate = DateTime.UtcNow };

        _usersRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).ReturnsAsync(false);


        // Act
        var result = await _usersRepositoryMock.Object.ExistsAsync(x => x.Email == "testuser@example.com");

        // Assert
        Assert.False(result);
    }


}
