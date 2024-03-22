using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Services;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALIVEMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrackController : ControllerBase
{
    private readonly ITrackService _trackService;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<TrackController> _logger;

    public TrackController(ITrackService trackService, ILogger<TrackController> logger, IAzureBlobService azureBlobService)
    {

        _trackService = trackService;
        _logger = logger;
        _azureBlobService = azureBlobService;
    }

   // [HttpPost("uploadtrack")]
    //public async Task<IActionResult> UploadTrack([FromForm] TrackUploadDTO trackUploadDto)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }

    //    try
    //    {
    //        var track = await _trackService.UploadTrackAsync(trackUploadDto);
    //        return Ok(new { Message = "Track uploaded successfully", TrackID = track.track.TrackID });
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e, "Error uploading track");
    //        return StatusCode(500, "Error uploading track");
    //    }
    //}
    [HttpPost("uploadtrack")]
    public async Task<IActionResult> UploadTrackAsync(TrackUploadDTO trackUploadDTO)
    {
        try
        {
            var (track, jobId) = await _trackService.UploadTrackAsync(trackUploadDTO);
            return Ok(new { trackID = track.TrackID, jobId = jobId });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("replaceTrack")]
    public async Task<IActionResult> ReplaceTrack([FromForm] IFormFile newFile, [FromForm] string blobName)
    {
        using (var stream = newFile.OpenReadStream())
        {
            await _azureBlobService.ReplaceBlobAsync("tracks", blobName, stream);
        }

        return Ok(new { message = "Track replaced successfully" });
    }



    [HttpGet("{trackId}")]
    public async Task<IActionResult> GetTrack(int trackId)
    {
        var track = await _trackService.GetTrack(trackId);
        if (track == null)
        {
            return NotFound();
        }

        return Ok(new { TrackFilePath = track.TrackFilePath });
    }

    [HttpGet("job/{jobId}")]
    public async Task<IActionResult> GetTrackByJobId(Guid jobId)
    {
        var track = await _trackService.GetTrackByJobId(jobId);
        if (track == null)
        {
            return NotFound();
        }

        return Ok(new { TrackFilePath = track.TrackFilePath });
    }

    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetTracksByArtistId(int artistId)
    {
        try
        {
            var tracks = await _trackService.GetAllTracksByArtistId(artistId);
            if (tracks == null)
            {
                return NotFound();
            }

            return Ok(tracks);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetAllTracks()
    {
        try
        {
            var tracks = await _trackService.GetAllTracks();
            if (tracks == null)
            {
                return NotFound();
            }

            return Ok(tracks);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
