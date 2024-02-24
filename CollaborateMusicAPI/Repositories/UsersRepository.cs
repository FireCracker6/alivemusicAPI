using System.Diagnostics;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollaborateMusicAPI.Repositories;

public interface IUsersRepository : IRepository<ApplicationUser, ApplicationDBContext>
{
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task SaveRefreshToken(RefreshToken refreshToken);
    Task<ApplicationUser> GetUserByIdAsync(Guid id);
    Task UpdateUser(ApplicationUser user);
}

public class UsersRepository : Repository<ApplicationUser, ApplicationDBContext>, IUsersRepository
{
    public UsersRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Trim() == email.Trim());
        Debug.WriteLine($"Searched for email: {email}. Found: {user?.Email ?? "No user found"}");
        return user;
    }

    public async Task SaveRefreshToken(RefreshToken refreshToken) // Async method
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<ApplicationUser> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }

    public async Task UpdateUser(ApplicationUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

}
