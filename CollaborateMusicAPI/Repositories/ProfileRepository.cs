using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ALIVEMusicAPI.Repositories;

public interface IProfileRepository : IRepository<UserProfile, ApplicationDBContext>
{
    Task<UserProfile?> GetProfileByUserIdAsync(Guid userId);
    Task UpdateProfile(UserProfile profile);
    Task SaveProfile(UserProfile profile);
}

public class ProfileRepository : Repository<UserProfile, ApplicationDBContext>, IProfileRepository
{
    public ProfileRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetProfileByUserIdAsync(Guid userId)
    {
        var profile = await _context.UserProfiles
    .Include(p => p.User)
    .Include(p => p.User.Artist)
    .FirstOrDefaultAsync(p => p.UserID == userId);
        return profile;
    }

    public async Task UpdateProfile(UserProfile profile)
    {
        _context.UserProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }

    public async Task SaveProfile(UserProfile profile)
    {
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
    }
}

