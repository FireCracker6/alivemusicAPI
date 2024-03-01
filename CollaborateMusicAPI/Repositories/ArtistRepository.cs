using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ALIVEMusicAPI.Repositories;

public interface IArtistRepository : IRepository<Artist, ApplicationDBContext>
{
    Task<Artist?> GetArtistByUserIdAsync(Guid userId);
    Task UpdateArtist(Artist artist);
    Task SaveArtist(Artist artist);
}


public class ArtistRepository : Repository<Artist, ApplicationDBContext>, IArtistRepository
{
    public ArtistRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<Artist?> GetArtistByUserIdAsync(Guid userId)
    {

        var profile = await _context.Artists
        .Include(p => p.User)
        .FirstOrDefaultAsync(p => p.UserProfileID == userId);
        return profile;
    }


    public async Task UpdateArtist(Artist artist)
    {
        _context.Artists.Update(artist);
        await _context.SaveChangesAsync();
    }

    public async Task SaveArtist(Artist artist)
    {
        await _context.Artists.AddAsync(artist);
        await _context.SaveChangesAsync();
    }
}
