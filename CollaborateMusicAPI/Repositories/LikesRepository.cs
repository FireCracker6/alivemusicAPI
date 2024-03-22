using ALIVEMusicAPI.Models.Entities;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ALIVEMusicAPI.Repositories;

public interface ILikesRepository: IRepository<Likes, ApplicationDBContext>
{
    Task<Likes> AddLike(Guid userId, int trackId);
    Task<Likes> RemoveLike(Guid userId, int trackId);
    Task SaveLikes(Likes likes);
    Task<int> GetLikesCountByTrackId(int trackId);
    Task<List<Likes>> GetLikedTracksByUserAsync(Guid userId);
}

public class LikesRepository : Repository<Likes, ApplicationDBContext>, ILikesRepository
{
    public LikesRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<Likes> AddLikes(Likes likes)
    {
        await _context.Likes.AddAsync(likes);
        await _context.SaveChangesAsync();
        return likes;
    }

    public async Task SaveLikes(Likes likes)
    {
        await _context.Likes.AddAsync(likes);
        await _context.SaveChangesAsync();
    }

    public async Task<Likes> AddLike(Guid userId, int trackId)
    {
        var like = new Likes
        {
            UserID = userId,
            TrackID = trackId,
            Timestamp = DateTime.Now
        };
        await _context.Likes.AddAsync(like);
        await _context.SaveChangesAsync();
        return like;
    }

    public async Task<Likes> RemoveLike(Guid userId, int trackId)
    {
        var like = await _context.Likes
        .FirstOrDefaultAsync(l => l.UserID == userId && l.TrackID == trackId);
        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
        return like!;
    }

    public async Task<List<Likes>> GetLikedTracksByUserAsync(Guid userId)
    {
        return await _context.Likes
            .Where(l => l.UserID == userId)
            .ToListAsync();
    }


    public async Task<int> GetLikesCountByTrackId(int trackId)
    {
        return await _context.Likes.CountAsync(l => l.TrackID == trackId);
    }




}




