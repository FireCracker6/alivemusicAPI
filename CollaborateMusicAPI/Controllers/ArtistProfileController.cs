using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Services;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ALIVEMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtistProfileController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IProfileService _profileService;
    private readonly IAzureBlobService _azureBlobService;

    public ArtistProfileController(ApplicationDBContext context, IProfileService profileService, IAzureBlobService azureBlobService)
    {
        _context = context;
        _profileService = profileService;
        _azureBlobService = azureBlobService;
    }

    [HttpPost("createartistprofile")]
    public async Task<IActionResult> CreateArtistProfile(ArtistDTO artistProfileDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProfile = _context.Artists.FirstOrDefault(p => p.UserID == artistProfileDTO.UserID);
            if (existingProfile != null)
            {
                return Conflict("Profile already exists");
            }

            var response = await _profileService.SaveArtistProfileDetailsAsync(artistProfileDTO);

            if (response.StatusCode == CollaborateMusicAPI.Enums.StatusCode.Created)
            {
                return CreatedAtAction(nameof(GetArtistProfile), new { userId = artistProfileDTO.UserID }, response.Content);
            }

            return StatusCode((int)response.StatusCode, response.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("getartistprofile/{artistId}")]
    public async Task<IActionResult> GetArtistProfile(int artistId )
    {
        try
        {
            var response = await _profileService.GetArtistProfileAsync(artistId);
            if (response.Content == null)
            {
                return NotFound("Profile not found");
            }
            return Ok(response.Content);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("addartistimage")]
    public async Task<IActionResult> AddArtistImage([FromForm] ArtistBannerImageDTO artistBannerImageDTO)
    {
        try
        {
            if (artistBannerImageDTO.BannerPic == null)
            {
                return BadRequest("No file was uploaded");
            }

            var response = await _profileService.UploadArtistBannerLogoAsync(artistBannerImageDTO.UserProfileID, artistBannerImageDTO.BannerPic);
            if (response.StatusCode == CollaborateMusicAPI.Enums.StatusCode.Created)
            {
                return Ok(response.Content);
            }
            return StatusCode((int)response.StatusCode, response.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

    }

    [HttpGet("getartistbannerlogo/{userId}")]
    public IActionResult GetArtistBannerLogo(string userId)
    {
        try
        {
            var sasUrl = _azureBlobService.GetBlobSasUrl("artist-banner-logos", $"{userId}.jpg");
            return Ok(sasUrl);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

   
    




}
