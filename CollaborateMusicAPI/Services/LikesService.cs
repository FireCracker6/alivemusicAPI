using ALIVEMusicAPI.Helpers;
using ALIVEMusicAPI.Repositories;

namespace ALIVEMusicAPI.Services;

public interface ILikesService
{
    Task<ServiceResponse> AddLikeAsync(int trackId, Guid userId);
    Task<ServiceResponse> RemoveLikeAsync(int trackId, Guid userId);
    Task<int> GetLikesCountByTrackIdAsync(int trackId);
    // Task<bool> UserLikesTrackAsync(int trackId, string userId);
    Task<ServiceResponse> GetLikesByUserIdAsync(Guid userId);
}

public class LikesService : ILikesService
{
    private readonly ILikesRepository _likesRepository;
    private readonly ITrackRepository _trackRepository;

    public LikesService(ILikesRepository likesRepository, ITrackRepository trackRepository)
    {
        _likesRepository = likesRepository;
        _trackRepository = trackRepository;
    }

    public async Task<ServiceResponse> AddLikeAsync(int trackId, Guid userId)
    {
        var track = await _trackRepository.GetTrack(trackId);
        if (track == null)
        {
            return new ServiceResponse
            {
             
                Message = "Track not found"
            };
        }
        await _likesRepository.AddLike(userId, trackId);
        return new ServiceResponse
        {
           
            Message = "Like added"
        };
    }

    public async Task<ServiceResponse> RemoveLikeAsync(int trackId, Guid userId)
    {
        var track = await _trackRepository.GetTrack(trackId);
        if (track == null)
        {
            return new ServiceResponse
            {
                Message = "Track not found"
            };
        }
        var removedLike = await _likesRepository.RemoveLike(userId, trackId);
        if (removedLike == null)
        {
            return new ServiceResponse
            {
                Message = "Like not found"
            };
        }
        return new ServiceResponse
        {
            Data = removedLike,
            Message = "Like removed"
        };
    }


    public async Task<ServiceResponse> GetLikesByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return new ServiceResponse
            {
                Message = "Invalid user id"
            };
        }

        var likes = await _likesRepository.GetLikedTracksByUserAsync(userId);
        if (likes.Count == 0)
        {
            return new ServiceResponse
            {
                Message = "User has no likes"
            };
        }

        return new ServiceResponse
        {
            Data = likes
        };
    }


    public async Task<int> GetLikesCountByTrackIdAsync(int trackId)
    {
        return await _likesRepository.GetLikesCountByTrackId(trackId);
    }




    //public async Task<bool> UserLikesTrackAsync(int trackId, string userId)
    //{
    //    var like = await _likesRepository.GetLikeByUserIdAndTrackIdAsync(userId, trackId);
    //    return like != null;
    //}
}
