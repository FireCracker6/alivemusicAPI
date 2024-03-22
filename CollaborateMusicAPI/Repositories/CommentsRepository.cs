using System.Threading.Tasks;
using ALIVEMusicAPI.Models;
using ALIVEMusicAPI.Models.Entities;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ALIVEMusicAPI.Repositories;

public interface ICommentsRepository : IRepository<Comment, ApplicationDBContext>
{
    Task<Comment> AddComment(Guid userId, int trackId, int artistId, string content);
    Task<Comment> AddReply(Guid userId, int trackId, int artistId, int parentCommentId, string content);
    Task<Comment> UpdateComment(int commentId, string newContent);
    Task DeleteComment(int commentId);
    Task<Comment> GetComment(int commentId);
    Task<List<Comment>> GetCommentsByTrackId(int trackId);
    //Task SaveComment(Comment comment);
    //Task<List<Comment>> GetCommentsByTrackIdAsync(int trackId);
    //Task<List<Comment>> GetCommentsByUserIdAsync(Guid userId);
    //Task<int> GetCommentsCountByTrackId(int trackId);
    //Task<Comment> GetCommentByIdAsync(int commentId);
    //Task<List<Comment>> GetCommentsByArtistIdAsync(int artistId);
    //Task<List<Comment>> GetCommentsByAlbumIdAsync(int albumId);
    //Task<List<Comment>> GetCommentsByPlaylistIdAsync(int playlistId);
    //Task<List<Comment>> GetCommentsByUserIdAndTrackIdAsync(Guid userId, int trackId);
    //Task<List<Comment>> GetCommentsByUserIdAndArtistIdAsync(Guid userId, int artistId);
    //Task<List<Comment>> GetCommentsByUserIdAndAlbumIdAsync(Guid userId, int albumId);
    //Task<List<Comment>> GetCommentsByUserIdAndPlaylistIdAsync(Guid userId, int playlistId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAsync(Guid userId, Guid userId2);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndTrackIdAsync(Guid userId, Guid userId2, int trackId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndArtistIdAsync(Guid userId, Guid userId2, int artistId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndAlbumIdAsync(Guid userId, Guid userId2, int albumId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndPlaylistIdAsync(Guid userId, Guid userId2, int playlistId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndUserIdAsync(Guid userId, Guid userId2, Guid userId3);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndUserIdAndTrackIdAsync(Guid userId, Guid userId2, Guid userId3, int trackId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndUserIdAndArtistIdAsync(Guid userId, Guid userId2, Guid userId3, int artistId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndUserIdAndAlbumIdAsync(Guid userId, Guid userId2, Guid userId3, int albumId);
    //Task<List<Comment>> GetCommentsByUserIdAndUserIdAndUserIdAndPlaylistIdAsync(Guid userId, Guid userId2, Guid userId3, int playlistId);
    //Task<List<Comment>> GetComments
}

public class CommentsRepository : Repository<Comment, ApplicationDBContext>, ICommentsRepository
{
    public CommentsRepository(ApplicationDBContext context) : base(context)
    {
    }
    public async Task<Comment> AddComment(Guid userId, int trackId, int artistId, string content)
    {
        var comment = new Comment
        {
            UserID = userId,
            TrackID = trackId,
            ArtistID = artistId,
            Content = content,
            Timestamp = DateTime.UtcNow  // Set the timestamp to the current time
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        // Fetch the comment again, this time including the related User and UserProfile entities
        comment = await _context.Comments
            .Include(c => c.User)
                .ThenInclude(u => u.UserProfile)
            .FirstOrDefaultAsync(c => c.CommentID == comment.CommentID);

        var closure = new CommentClosure
        {
            AncestorId = comment.CommentID,
            DescendantId = comment.CommentID,
            Depth = 0
        };
        _context.CommentClosures.Add(closure);
        await _context.SaveChangesAsync();

        return comment!;
    }

    public async Task<Comment> AddReply(Guid userId, int trackId, int artistId, int parentCommentId, string content)
    {
        var comment = new Comment
        {
            UserID = userId,
            TrackID = trackId,
            ArtistID = artistId,
            ParentCommentID = parentCommentId,
            Content = content,
            Timestamp = DateTime.UtcNow  // Set the timestamp to the current time
        };

        // Add the new comment to the database
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        var parentClosures = await _context.CommentClosures
            .Where(c => c.DescendantId == parentCommentId)
            .ToListAsync();
        foreach(var parentClosure in parentClosures)
        {
            var closure = new CommentClosure
            {
                AncestorId = parentClosure.AncestorId,
                DescendantId = comment.CommentID,
                Depth = parentClosure.Depth + 1
            };
            _context.CommentClosures.Add(closure);
        }
        await _context.SaveChangesAsync();

        // Fetch the comment from the database
        comment = await _context.Comments
           .Include(c => c.User)
               .ThenInclude(u => u.UserProfile)
           .FirstOrDefaultAsync(c => c.CommentID == comment.CommentID);

        return comment;
    }


    public async Task<Comment> UpdateComment(int commentId, string newContent)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
        {
            throw new Exception("Comment not found");
        }

        comment.Content = newContent;
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task DeleteComment(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
        {
            throw new Exception("Comment not found");
        }

        var closures = await _context.CommentClosures
      .Where(cc => cc.AncestorId == commentId || cc.DescendantId == commentId)
      .ToListAsync();
        _context.CommentClosures.RemoveRange(closures);

        _context.Comments.Remove(comment);

    }

    public async Task<Comment> GetComment(int commentId) 
    {
        var comment = await _context.Comments
            .Include(c => c.Artist)
            .Include(c => c.User)
                .ThenInclude(u => u.UserProfile)
            .FirstOrDefaultAsync(c => c.CommentID == commentId);

        if (comment == null)
        {
            throw new Exception("Comment not found");
        }

        return comment;
    }

    public async Task<List<Comment>> GetCommentsByTrackId(int trackId)
    {
        var comments = await _context.Comments
            .Include(c => c.Artist)
            .Include(c => c.User)
                .ThenInclude(u => u.UserProfile)
            .Where(c => c.TrackID == trackId)  // Fetch all comments, not just top-level ones
            .ToListAsync();

        foreach (var comment in comments)
        {
            // Get all descendant IDs of the comment
            var descendantIds = await _context.CommentClosures
                .Where(cc => cc.AncestorId == comment.CommentID && cc.Depth > 0)
                .Select(cc => cc.DescendantId)
                .ToListAsync();

            // Fetch the descendant comments
            var descendants = await _context.Comments
                .Include(c => c.User)
                    .ThenInclude(u => u.UserProfile)
                .Where(c => descendantIds.Contains(c.CommentID))
                .ToListAsync();

            // Add the descendants to the comment's ChildComments collection
            foreach (var descendant in descendants)
            {
                comment.ChildComments.Add(descendant);
            }
        }

        // Return only the top-level comments
        return comments.Where(c => c.ParentCommentID == null).ToList();
    }








}

