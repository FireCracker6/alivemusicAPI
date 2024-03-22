using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Repositories;
using ALIVEMusicAPI.Services;
using CollaborateMusicAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALIVEMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentLikesController : ControllerBase
{
    private readonly ICommentLikesRepository _commentLikesRepository;
    private readonly ICommentLikesService _commentLikesService;
    private readonly ICommentsRepository _commentsRepository;
    private readonly IUsersRepository _userRepository;

    public CommentLikesController(ICommentLikesRepository commentLikesRepository, ICommentsRepository commentsRepository, IUsersRepository userRepository, ICommentLikesService commentLikesService)
    {
        _commentLikesRepository = commentLikesRepository;
        _commentsRepository = commentsRepository;
        _userRepository = userRepository;
        _commentLikesService = commentLikesService;
    }

    [HttpPost("addlike")]
    public async Task<IActionResult> AddLike([FromBody] CommentLikesDTO likeDTO)
    
    {
        var comment = await _commentsRepository.GetComment(likeDTO.CommentID);
        if (comment == null)
        {
            return NotFound("Comment not found");
        }

        var user = await _userRepository.GetUserByIdAsync(likeDTO.UserID);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var like = await _commentLikesService.AddLikeAsync( likeDTO.CommentID, likeDTO.UserID);
        return Ok(like);
    }

    [HttpPost("removelike")]
    public async Task<IActionResult> RemoveLike([FromBody] CommentLikesDTO likeDTO)
    {
        var comment = await _commentsRepository.GetComment(likeDTO.CommentID);
        if (comment == null)
        {
            return NotFound("Comment not found");
        }

        var user = await _userRepository.GetUserByIdAsync(likeDTO.UserID);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var like = await _commentLikesService.RemoveLikeAsync(likeDTO.CommentID, likeDTO.UserID);
        return Ok(like);
    }

    [HttpGet("{commentId}/likescount")]
    public async Task<IActionResult> GetLikesCount(int commentId)
    {
        var count = await _commentLikesRepository.GetLikesCountByCommentId(commentId);
        return Ok(count);
    }

    [HttpGet("track/{trackId}/likescount")]
    public async Task<IActionResult> GetLikesCountByTrackId(int trackId)
    {
        var likesCount = await _commentLikesRepository.GetLikesCountByTrackId(trackId);
        return Ok(likesCount);
    }




    [HttpGet("getlikedcomments/{userId}")]
    public async Task<IActionResult> GetLikedComments(Guid userId)
    {
        var likedComments = await _commentLikesRepository.GetLikedCommentsByUserAsync(userId);
        return Ok(likedComments);
    }   
}
