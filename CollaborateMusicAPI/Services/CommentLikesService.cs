using ALIVEMusicAPI.Helpers;
using ALIVEMusicAPI.Repositories;
using CollaborateMusicAPI.Repositories;

namespace ALIVEMusicAPI.Services;

public interface ICommentLikesService
{
    Task<ServiceResponse> AddLikeAsync(int commentId, Guid userId);
    Task<ServiceResponse> RemoveLikeAsync(int commentId, Guid userId);
    Task<ServiceResponse> GetLikesByUserIdAsync(Guid userId);
    Task<ServiceResponse> GetLikesCountByCommentIdAsync(int commentId);
}

public class CommentLikesService : ICommentLikesService
{
    private readonly ICommentLikesRepository _commentLikesRepository;
    private readonly ICommentsRepository _commentsRepository;
    private readonly IUsersRepository _usersRepository;

    public CommentLikesService(ICommentLikesRepository commentLikesRepository, ICommentsRepository commentsRepository, IUsersRepository usersRepository)
    {
        _commentLikesRepository = commentLikesRepository;
        _commentsRepository = commentsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<ServiceResponse> AddLikeAsync(int commentId, Guid userId)
    {
        var comment = await _commentsRepository.GetComment(commentId);
        if (comment == null)
        {
            return new ServiceResponse
            {
                Message = "Comment not found"
            };
        }
        await _commentLikesRepository.AddLike(userId, commentId);
        return new ServiceResponse
        {
            Message = "Like added"
        };
    }

    public async Task<ServiceResponse> RemoveLikeAsync(int commentId, Guid userId)
    {
        var comment = await _commentsRepository.GetComment(commentId);
        if (comment == null)
        {
            return new ServiceResponse
            {
                Message = "Comment not found"
            };
        }
        var removedLike = await _commentLikesRepository.RemoveLike(userId, commentId);
        if (removedLike == null)
        {
            return new ServiceResponse
            {
                Message = "Like not found"
            };
        }
        return new ServiceResponse
        {
            Message = "Like removed"
        };
    }

    public async Task<ServiceResponse> GetLikesByUserIdAsync(Guid userId)
    {
        
        if (userId == Guid.Empty)
        {
            return new ServiceResponse
            {
                Message = "User not found"
            };
        }


        var likes = await _commentLikesRepository.GetLikedCommentsByUserAsync(userId);

        if (likes == null)
        {
            return new ServiceResponse
            {
                Message = "Likes not found"
            };
        }
        return new ServiceResponse
        {
            Data = likes
        };
    }

    public async Task<ServiceResponse> GetLikesCountByCommentIdAsync(int commentId)
    {
        var likesCount = await _commentLikesRepository.GetLikesCountByCommentId(commentId);
        return new ServiceResponse
        {
            Data = likesCount
        };
    }
}
