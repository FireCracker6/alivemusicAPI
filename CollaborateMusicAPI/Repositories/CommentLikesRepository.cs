using ALIVEMusicAPI.Models.Entities;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ALIVEMusicAPI.Repositories;

public interface ICommentLikesRepository : IRepository<CommentLikes, ApplicationDBContext>
{
    Task<CommentLikes> AddLike(Guid userId, int commentId);
    Task<CommentLikes> RemoveLike(Guid userId, int commentId);
    Task<int> GetLikesCountByCommentId(int commentId);
    Task<List<CommentLikes>> GetLikedCommentsByUserAsync(Guid userId);
    Task<Dictionary<int, int>> GetLikesCountByTrackId(int trackId);
}

public class CommentLikesRepository : Repository<CommentLikes, ApplicationDBContext>, ICommentLikesRepository
{
    public CommentLikesRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<CommentLikes> AddLike(Guid userId, int commentId)
    {
        var like = new CommentLikes
        {
            UserID = userId,
            CommentID = commentId,
            Timestamp = DateTime.Now
        };
       await _context.CommentLikes.AddAsync(like);
       await  _context.SaveChangesAsync();
       return like;
    }

    public async Task<List<CommentLikes>> GetLikedCommentsByUserAsync(Guid userId)
    {
        return await _context.CommentLikes
            .Where(l => l.UserID == userId)
            .ToListAsync();
    }

    public async Task<int> GetLikesCountByCommentId(int commentId)
    {
        return await _context.CommentLikes
            .CountAsync(l => l.CommentID == commentId);
    }

    public async Task<Dictionary<int, int>> GetLikesCountByTrackId(int trackId)
    {
        // Fetch all CommentLikes for the given TrackID
        var commentLikes = await _context.CommentLikes
            .Where(cl => cl.Comment.TrackID == trackId)
            .ToListAsync();

        // If there are no CommentLikes, return an empty dictionary
        if (!commentLikes.Any())
        {
            return new Dictionary<int, int>();
        }

        // Group the CommentLikes by CommentID and count them
        var likesCount = commentLikes
            .GroupBy(cl => cl.CommentID)
            .ToDictionary(g => g.Key, g => g.Count());

        return likesCount;
    }



    public async Task<CommentLikes> RemoveLike(Guid userId, int commentId)
    {
        var like = await _context.CommentLikes
            .FirstOrDefaultAsync(l => l.UserID == userId && l.CommentID == commentId);
        if (like != null)
        {
            _context.CommentLikes.Remove(like);
           await  _context.SaveChangesAsync();
        }
        return like!;
    }

    public Task SaveLikes(CommentLikes likes)
    {
        throw new NotImplementedException();
    }
}

