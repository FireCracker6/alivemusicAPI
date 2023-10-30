using System.Diagnostics;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollaborateMusicAPI.Repositories;

public interface IUsersRepository : IRepository<Users, ApplicationDBContext>
{
    Task<Users?> GetUserByEmailAsync(string email);
}


public class UsersRepository : Repository<Users, ApplicationDBContext>, IUsersRepository
{
    

    public UsersRepository(ApplicationDBContext context) : base (context)
    {
      
    }
    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        Debug.WriteLine($"Searched for email: {email}. Found: {user?.Email ?? "No user found"}");
        return user;
    }
   

    
}
