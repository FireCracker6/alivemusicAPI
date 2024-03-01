using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Repositories;
using CollaborateMusicAPI.Enums;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.Entities;

namespace ALIVEMusicAPI.Services;

public interface IProfileService
{
    Task<ServiceResponse<UserProfileDTO>> CreateProfileAsync(UserProfileDTO userProfileDTO);
    Task<ServiceResponse<UserProfileDTO>> GetProfileAsync(string userId);
    Task<ServiceResponse<ArtistDTO>> SaveArtistProfileDetailsAsync(ArtistDTO artistDTO);
    Task<ServiceResponse<string>> UploadArtistBannerLogoAsync(Guid userProfileID, IFormFile bannerLogoPic);
    Task<ServiceResponse<ArtistDTO>> GetArtistProfileAsync(string userId);

}

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IArtistRepository _artistRepository;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(IProfileRepository profileRepository, IArtistRepository artistRepository, ILogger<ProfileService> logger, IAzureBlobService azureBlobService)
    {
        _profileRepository = profileRepository;
        _artistRepository = artistRepository;
        _logger = logger;
        _azureBlobService = azureBlobService;
    }

    public async Task<ServiceResponse<UserProfileDTO>> CreateProfileAsync(UserProfileDTO userProfileDTO)
    {
        var response = new ServiceResponse<UserProfileDTO>();
        try
        {
            // Convert the profile picture to a Stream
            using var profilePicStream = new MemoryStream();
            await userProfileDTO.ProfilePic.CopyToAsync(profilePicStream);

            // Reset the position of the MemoryStream to the start
            profilePicStream.Position = 0;

            // Upload the profile picture to Azure Blob Storage
            var profilePicturePath = await _azureBlobService.UploadFileAsync("profile-pictures", $"{userProfileDTO.UserID}.jpg", profilePicStream);

            var profile = new UserProfile
            {
                FullName = userProfileDTO.FullName,
                Bio = userProfileDTO.Bio,
                Location = userProfileDTO.Location,
                WebsiteURL = userProfileDTO.WebsiteURL,
                UserID = userProfileDTO.UserID,
                ProfilePicturePath = profilePicturePath
            };
            await _profileRepository.SaveProfile(profile);
            response.Content = userProfileDTO;
            response.StatusCode = StatusCode.Created;
            response.Message = "Profile created successfully";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating profile");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error creating profile";
        }
        return response;
    }

    public async Task<ServiceResponse<UserProfileDTO>> UpdateProfileAsync(UserProfileDTO userProfileDTO)
    {
        var response = new ServiceResponse<UserProfileDTO>();
        try
        {
            var profile = await _profileRepository.GetProfileByUserIdAsync(userProfileDTO.UserID);
            if (profile == null)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Message = "Profile not found";
                return response;
            }

            if (userProfileDTO.ProfilePic != null)
            {
                using var profilePicStream = new MemoryStream();
                await userProfileDTO.ProfilePic.CopyToAsync(profilePicStream);
                profilePicStream.Position = 0;
                var profilePicturePath = await _azureBlobService.UploadFileAsync("profile-pictures", $"{userProfileDTO.UserID}.jpg", profilePicStream);
                profile.ProfilePicturePath = profilePicturePath;
            }

            profile.FullName = userProfileDTO.FullName;
            profile.Bio = userProfileDTO.Bio;
            profile.Location = userProfileDTO.Location;
            profile.WebsiteURL = userProfileDTO.WebsiteURL;

            await _profileRepository.UpdateProfile(profile);
            response.Content = userProfileDTO;
            response.StatusCode = StatusCode.Ok;
            response.Message = "Profile updated successfully";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating profile");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error updating profile";
        }
        return response;    

    }
    // Create an optional Artist profile
    public async Task<ServiceResponse<ArtistDTO>> CreateArtistProfileAsync(ArtistDTO artistDTO)
    {
        var response = new ServiceResponse<ArtistDTO>();
        try
        {
            var profile = await _profileRepository.GetProfileByUserIdAsync(artistDTO.UserProfileID);
            if (profile == null)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Message = "Profile not found";
                return response;
            }

            var artist = new Artist
            {
                ArtistName = artistDTO.ArtistName,
                Description = artistDTO.Description,
                Genre = artistDTO.Genre,
                UserProfileID = artistDTO.UserProfileID
            };

            if (artistDTO.ArtistPic != null)
            {
                using var artistPicStream = new MemoryStream();
                await artistDTO.ArtistPic.CopyToAsync(artistPicStream);
                artistPicStream.Position = 0;
                var artistPicturePath = await _azureBlobService.UploadFileAsync("artist-pictures", $"{artistDTO.UserProfileID}.jpg", artistPicStream);
                artist.ArtistPicturePath = artistPicturePath;
            }

            await _artistRepository.SaveArtist(artist);
            response.Content = artistDTO;
            response.StatusCode = StatusCode.Created;
            response.Message = "Artist profile created successfully";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating artist profile");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error creating artist profile";
        }
        return response;
    }




    public async Task<ServiceResponse<UserProfileDTO>> GetProfileAsync(string userId)
    {
        var response = new ServiceResponse<UserProfileDTO>();

        try
        {
            var profile = await _profileRepository.GetProfileByUserIdAsync(Guid.Parse(userId));
            if (profile == null)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Message = "Profile not found";
                return response;
            }

            var profileDto = new UserProfileDTO
            {
                FullName = profile.FullName,
                Email = profile.User.Email,
                UserID = profile.UserID,
                Bio = profile.Bio,
                Location = profile.Location,
                WebsiteURL = profile.WebsiteURL,
                ProfilePicturePath = _azureBlobService.GetBlobSasUrl("profile-pictures", $"{userId}.jpg")
            };

            response.Content = profileDto;
            response.StatusCode = StatusCode.Ok;
            response.Message = "Profile retrieved successfully";


        } catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving profile");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error retrieving profile";
        }

        return response;
    }

    public async Task<ServiceResponse<ArtistDTO>> GetArtistProfileAsync(string userId)
    {
        var response = new ServiceResponse<ArtistDTO>();

        try
        {
            var artist = await _artistRepository.GetArtistByUserIdAsync(Guid.Parse(userId));
            if (artist == null)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Message = "Artist profile not found";
                return response;
            }

            var artistDto = new ArtistDTO
            {
                ArtistID = artist.ArtistID,
                ArtistName = artist.ArtistName,
                Description = artist.Description,
                Genre = artist.Genre,
                UserProfileID = artist.UserProfileID,
                ArtistPicturePath = _azureBlobService.GetBlobSasUrl("artist-pictures", $"{userId}.jpg")
            };

            response.Content = artistDto;
            response.StatusCode = StatusCode.Ok;
            response.Message = "Artist profile retrieved successfully";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving artist profile");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error retrieving artist profile";
        }

        return response;
    }




    public async Task<ServiceResponse<ArtistDTO>> SaveArtistProfileDetailsAsync(ArtistDTO artistDTO)
    {
        var response = new ServiceResponse<ArtistDTO>();
        try
        {
            var profile = await _profileRepository.GetProfileByUserIdAsync(artistDTO.UserProfileID);
            if (profile == null)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Message = "Profile not found";
                return response;
            }

            var artist = new Artist
            {
                ArtistName = artistDTO.ArtistName,
                Description = artistDTO.Description,
                Genre = artistDTO.Genre,
                UserProfileID = artistDTO.UserProfileID
            };

            await _artistRepository.SaveArtist(artist);
            response.Content = artistDTO;
            response.StatusCode = StatusCode.Created;
            response.Message = "Artist profile created successfully";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating artist profile");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error creating artist profile";
        }
        return response;
    }
    public async Task<ServiceResponse<string>> UploadArtistBannerLogoAsync(Guid userProfileID, IFormFile bannerLogoPic)
    {
        var response = new ServiceResponse<string>();
        try
        {
            using var bannerLogoPicStream = new MemoryStream();
            await bannerLogoPic.CopyToAsync(bannerLogoPicStream);
            bannerLogoPicStream.Position = 0;
            var bannerLogoPicPath = await _azureBlobService.UploadFileAsync("artist-banner-logos", $"{userProfileID}.jpg", bannerLogoPicStream);

            response.Content = bannerLogoPicPath;
            response.StatusCode = StatusCode.Created;
            response.Message = "Banner logo picture uploaded successfully";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error uploading banner logo picture");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error uploading banner logo picture";
        }
        return response;
    }

}
