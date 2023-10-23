using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;

namespace CollaborateMusicAPI.Repositories;

public class UsersRepository : Repository<Users>
{
    private readonly ApplicationDBContext _context;

    public UsersRepository(ApplicationDBContext context)
    {
        _context = context;
    }
    public Users GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public override async Task<Users> CreateAsync(Users user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
