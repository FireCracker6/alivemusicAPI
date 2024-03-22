using System.Runtime.CompilerServices;
using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Models.Entities;
using ALIVEMusicAPI.Repositories;
using ALIVEMusicAPI.Services;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALIVEMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LikesController : ControllerBase
{

    private readonly ILikesRepository _likeRepository;
    private readonly ILikesService _likeService;
    private readonly ITrackRepository _trackRepository;
    private readonly IArtistRepository _artistRepository;
    private readonly IUsersRepository _userRepository;

    public LikesController(ILikesRepository likeRepository, ITrackRepository trackRepository, IArtistRepository artistRepository, IUsersRepository userRepository, ILikesService likeService)
    {
        _likeRepository = likeRepository;
        _trackRepository = trackRepository;
        _artistRepository = artistRepository;
        _userRepository = userRepository;
        _likeService = likeService;
    }

    [HttpPost]
    public async Task<IActionResult> LikeTrack([FromBody] LikesDTO likeDTO)
    {
        var track = await _trackRepository.GetTrack(likeDTO.TrackID);
        if (track == null)
        {
            return NotFound("Track not found");
        }

        var user = await _userRepository.GetUserByIdAsync(likeDTO.UserID);
        if (user == null)
        {
            return NotFound("User not found");
        }

        //var artist = await _artistRepository.GetArtistByUserIdAsync(likeDTO.UserID);
        //if (artist == null)
        //{
        //    return NotFound("Artist not found");
        //}

        var like = new Likes
        {
            Timestamp = DateTime.Now,
            TrackID = likeDTO.TrackID,
            UserID = likeDTO.UserID
        };

        await _likeService.AddLikeAsync(likeDTO.TrackID, likeDTO.UserID);
        return Ok(like);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> UnlikeTrack( int trackId, Guid userId)
    {
        var like = await _likeService.RemoveLikeAsync(trackId, userId);
        if (like == null)
        {
            return NotFound("Like not found");
        }


        return Ok();
    }

    [HttpGet("{trackId}/likescount")]
    public async Task<IActionResult> GetLikesCount(int trackId)
    {
        var count = await _likeService.GetLikesCountByTrackIdAsync(trackId);
        return Ok(count);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetLikesByUserId(Guid userId)
    {
        var likes = await _likeService.GetLikesByUserIdAsync(userId);
        if (likes == null)
        {
            return NotFound("Likes not found");
        }
        return Ok(likes);
    }

    //[HttpGet("{trackId}")]
    //public async Task<IActionResult> GetLikes(int trackId)
    //{
    //    var likes = await _likeRepository.GetLikesByTrackId(trackId);
    //    if (likes == null)
    //    {
    //        return NotFound("Likes not found");
    //    }
    //    return Ok(likes);
    //}
}
