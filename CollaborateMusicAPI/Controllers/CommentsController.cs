using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Models.Entities;
using ALIVEMusicAPI.Repositories;
using ALIVEMusicAPI.Services;
using CollaborateMusicAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALIVEMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly ICommentsService _commentsService;
    private readonly ITrackRepository _trackRepository;
    private readonly IArtistRepository _artistRepository;
    private readonly IUsersRepository _userRepository;


    public CommentsController(ICommentsRepository commentsRepository, ITrackRepository trackRepository, IArtistRepository artistRepository, IUsersRepository userRepository, ICommentsService commentsService)
    {
        _commentsRepository = commentsRepository;
        _trackRepository = trackRepository;
        _artistRepository = artistRepository;
        _userRepository = userRepository;
        _commentsService = commentsService;
    }

    [HttpPost("addcomment")]
    public async Task<IActionResult> AddComment([FromBody] CommentsDTO commentDTO)
    {
        var track = await _trackRepository.GetTrack((int)commentDTO.TrackID);
        if (track == null)
        {
            return NotFound("Track not found");
        }

        var user = await _userRepository.GetUserByIdAsync(commentDTO.UserID);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var comment = await _commentsService.AddComment(commentDTO.UserID, (int)commentDTO.TrackID, commentDTO.ArtistID, commentDTO.Content);
        return Ok(comment);
    }


    [HttpPost("addreply")]
    public async Task<IActionResult> AddReply([FromBody] CommentsDTO commentDTO)
    {
        var track = await _trackRepository.GetTrack((int)commentDTO.TrackID);
        if (track == null)
        {
            return NotFound("Track not found");
        }

        var user = await _userRepository.GetUserByIdAsync(commentDTO.UserID);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var comment = await _commentsService.AddReply(commentDTO.UserID, (int)commentDTO.TrackID, (int)commentDTO.ArtistID, (int)commentDTO.ParentCommentID, commentDTO.Content);
        return Ok(comment);
    }


    [HttpPut("updatecomment")]
    public async Task<IActionResult> UpdateComment([FromBody] CommentsDTO commentDTO)
    {
        var comment = await _commentsRepository.GetComment((int)commentDTO.CommentID);
        if (comment == null)
        {
            return NotFound("Comment not found");
        }

        await _commentsService.UpdateComment((int)commentDTO.CommentID, commentDTO.Content);
        return Ok(comment);
    }

    [HttpDelete("deletecomment")]
    public async Task<IActionResult> DeleteComment([FromBody] CommentsDTO commentDTO)
    {
        var comment = await _commentsRepository.GetComment((int)commentDTO.CommentID);
        if (comment == null)
        {
            return NotFound("Comment not found");
        }

        await _commentsRepository.DeleteComment((int)commentDTO.CommentID);
        return Ok();
    }


    [HttpGet("comment/{commentId}/comments")]
    public async Task<IActionResult> GetCommentsByCommentId(int commentId)
    {
        if (commentId == 0)
        {
            return NotFound("Comment not found");
        }   
        var comments = await _commentsRepository.GetComment(commentId);
        return Ok(comments);
    }
    [HttpGet("track/{trackId}/comments")]
    public async Task<IActionResult> GetCommentsByTrackId(int trackId)
    {
        if (trackId == 0)
        {
            return NotFound("Track not found");
        }
        var comments = await _commentsService.GetCommentsByTrackId(trackId);
        return Ok(comments);
    }



}
