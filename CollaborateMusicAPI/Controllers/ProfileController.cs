using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Services;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollaborateMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IProfileService _profileService;
    private readonly IAzureBlobService _azureBlobService;

    public ProfileController(ApplicationDBContext context, IProfileService profileService, IAzureBlobService azureBlobService)
    {
        _context = context;
        _profileService = profileService;
        _azureBlobService = azureBlobService;
    }


    [HttpPost("createprofile")]
    public async Task<IActionResult> CreateProfile(UserProfileDTO userProfileDTO)
    {
      try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProfile = _context.UserProfiles.FirstOrDefault(p => p.UserID == userProfileDTO.UserID);
            if (existingProfile != null)
            {
                return Conflict("Profile already exists");
            }

            var response = await _profileService.CreateProfileAsync(userProfileDTO);
    

            if (response.StatusCode == Enums.StatusCode.Created)
            {
                return CreatedAtAction(nameof(GetProfile), new { userId = userProfileDTO.UserID }, response.Content);
            }

            return StatusCode((int)response.StatusCode, response.Message);


        } catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
       
    }
    [HttpPut("updateprofile")]
    public async Task<IActionResult> UpdateProfile(UserProfileDTO userProfileDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProfile = _context.UserProfiles.FirstOrDefault(p => p.UserID == userProfileDTO.UserID);
            if (existingProfile == null)
            {
                return NotFound("Profile not found");
            }

            var response = await _profileService.UpdateProfileAsync(userProfileDTO);

            if (response.StatusCode == Enums.StatusCode.NotFound)
            {
                return NotFound(response.Message);
            }

            return Ok(response.Content);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("profile-picture/{userId}")]
    public async Task<IActionResult> GetProfilePicture(string userId)
    
    
    
    {
        var serviceResponse = await _profileService.GetProfileAsync(userId);
        if (serviceResponse.Content == null)
        {
            return NotFound("Profile not found");
        }

        var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(serviceResponse.Content.ProfilePicturePath);

        return File(imageBytes, "image/jpeg");
    }


    [HttpPut("updateprofilepicture")]
    public async Task<IActionResult> UpdateProfilePicture([FromForm]  Guid userId, IFormFile file)
    {
        var response = await _profileService.UpdateProfilePictureAsync( userId, file);
        if (response.StatusCode == Enums.StatusCode.NotFound)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Content);
    }

 
   

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfile(string userId)
    {
        var response = await _profileService.GetProfileAsync(userId);
        if (response.StatusCode == Enums.StatusCode.NotFound)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Content);
    }

    // Refresh the SAS URL for the profile picture
    [HttpGet("refresh-token/{userId}")]
    public IActionResult RefreshToken(string userId)
    {
        var newSasUrl = _azureBlobService.GetBlobSasUrl("profile-pictures", $"{userId}.jpg");
        return Ok(newSasUrl);
    }


}
