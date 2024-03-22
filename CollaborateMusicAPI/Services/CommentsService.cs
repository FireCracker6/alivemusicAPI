using ALIVEMusicAPI.Helpers;
using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Models.Entities;
using ALIVEMusicAPI.Repositories;
using CollaborateMusicAPI.Models;

namespace ALIVEMusicAPI.Services;

public interface ICommentsService
{
    Task<ServiceResponse<Comment>> AddComment(Guid userId, int trackId, int artistId, string content);
    Task<ServiceResponse<Comment>> AddReply(Guid userId, int trackId, int artistId, int parentCommentId, string content);
    Task<ServiceResponse<Comment>> UpdateComment(int commentId, string newContent);
    Task<ServiceResponse<Comment>> DeleteComment(int commentId);
    Task<List<CommentsDTO>> GetCommentsByTrackId(int trackId);
}

public class CommentsService : ICommentsService
{
    private readonly ICommentsRepository _commentsRepository;

    public CommentsService(ICommentsRepository commentsRepository)
    {
        _commentsRepository = commentsRepository;
    }

    public async Task<ServiceResponse<Comment>> AddComment(Guid userId, int trackId, int artistId, string content)
    {
        ServiceResponse<Comment> response = new ServiceResponse<Comment>();
        try
        {
            Comment comment = await _commentsRepository.AddComment(userId, trackId, artistId, content);
            response.Data = new CommentsDTO
            {
                CommentID = comment.CommentID,
                UserID = comment.UserID,
                TrackID = comment.TrackID,
                ArtistID = comment.ArtistID,
                Content = comment.Content,
                Timestamp = comment.Timestamp,
                UserName = comment.User?.UserProfile?.FullName,
                UserPicturePath = comment.User?.UserProfile?.ProfilePicturePath,
                ArtistName = comment.Artist?.ArtistName

            };
            response.Message = "Comment added successfully";
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Message = $"An error occurred while adding the comment: {ex.Message}";
            response.Success = false;
        }

        return response;
    }

    public async Task<ServiceResponse<Comment>> AddReply(Guid userId, int trackId, int artistId, int parentCommentId, string content)
    {
        ServiceResponse<Comment> response = new ServiceResponse<Comment>();
        try
        {
            Comment comment = await _commentsRepository.AddReply(userId, trackId, artistId, parentCommentId, content);

            if (comment == null)
            {
                response.Message = "Failed to add reply";
                response.Success = false;
                return response;
            }

            response.Data = new CommentsDTO
          {
                CommentID = comment.CommentID,
                ParentCommentID = comment.ParentCommentID,
                UserID = comment.UserID,
                TrackID = comment.TrackID,
                ArtistID = comment.ArtistID,
                Content = comment.Content,
                Timestamp = comment.Timestamp,
                UserName = comment.User?.UserProfile?.FullName,
                UserPicturePath = comment.User?.UserProfile?.ProfilePicturePath,
                ArtistName = comment.Artist?.ArtistName
            };
            response.Message = "Reply added successfully";
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Message = $"An error occurred while adding the reply: {ex.Message}";
            response.Success = false;
        }

        return response;
    }
    
    public async Task<ServiceResponse<Comment>> UpdateComment(int commentId, string newContent)
    {
        ServiceResponse<Comment> response = new ServiceResponse<Comment>();
        try
        {
            Comment comment = await _commentsRepository.UpdateComment(commentId, newContent);
            response.Data = comment;
            response.Message = "Comment updated successfully";
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Message = $"An error occurred while updating the comment: {ex.Message}";
            response.Success = false;
        }

        return response;
    } 
    
    public async Task<ServiceResponse<Comment>> DeleteComment(int commentId)
    {
        ServiceResponse<Comment> response = new ServiceResponse<Comment>();
        try
        {
            await _commentsRepository.DeleteComment(commentId);
            response.Message = "Comment deleted successfully";
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Message = $"An error occurred while deleting the comment: {ex.Message}";
            response.Success = false;
        }

        return response;
    }


    private CommentsDTO MapCommentToDto(Comment comment)
    {
        return new CommentsDTO
        {
            CommentID = comment.CommentID,
            Content = comment.Content,
            Timestamp = comment.Timestamp,
            UserID = comment.UserID,
            UserName = comment.User?.UserProfile?.FullName,
            UserPicturePath = comment.User?.UserProfile?.ProfilePicturePath,
            TrackID = comment.TrackID,
            ArtistID = comment.ArtistID,
            ArtistName = comment.Artist?.ArtistName,
            ParentCommentID = comment.ParentCommentID,

            Replies = comment.ChildComments.Select(MapCommentToDto).ToList()  // Recursive call
        };
    }



    public async Task<List<CommentsDTO>> GetCommentsByTrackId(int trackId)
    {
        var comments = await _commentsRepository.GetCommentsByTrackId(trackId);

        // Build a dictionary of comments by their ID
        var commentDict = comments.ToDictionary(c => c.CommentID, MapCommentToDto);

        // Build the hierarchy of comments
        foreach (var comment in comments)
        {
            foreach (var childComment in comment.ChildComments)
            {
                if (commentDict.TryGetValue(childComment.CommentID, out var childDto))
                {
                    commentDict[comment.CommentID].Replies.Add(childDto);
                }
            }
        }

        // Return only the top-level comments
        return commentDict.Values.Where(c => c.ParentCommentID == null).ToList();
    }






}


