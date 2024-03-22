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
    Task<ServiceResponse<ArtistDTO>> GetArtistProfileAsync(int artistId);
    Task<ServiceResponse<UserProfileDTO>> UpdateProfileAsync(UserProfileDTO userProfileDTO);
    Task<ServiceResponse<string>> UpdateProfilePictureAsync(Guid userId, IFormFile profilePic);
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
                ProfilePicturePath = $"https://{_azureBlobService.AccountName}.blob.core.windows.net/profile-pictures/{userProfileDTO.UserID}.jpg"

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
                profile.ProfilePicturePath = $"https://{_azureBlobService.AccountName}.blob.core.windows.net/profile-pictures/{userProfileDTO.UserID}.jpg";
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

    public async Task<ServiceResponse<string>> UpdateProfilePictureAsync(Guid userId, IFormFile profilePic)
    {
        var response = new ServiceResponse<string>();
        try
        {
            var profile = await _profileRepository.GetProfileByUserIdAsync(userId);
            if (profile == null)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Message = "Profile not found";
                return response;
            }

            if (profilePic != null)
            {
                using var profilePicStream = new MemoryStream();
                await profilePic.CopyToAsync(profilePicStream);
                profilePicStream.Position = 0;
                var profilePicturePath = await _azureBlobService.UploadFileAsync("profile-pictures", $"{userId}.jpg", profilePicStream);
                profile.ProfilePicturePath = $"https://{_azureBlobService.AccountName}.blob.core.windows.net/profile-pictures/{userId}.jpg";
            }

            await _profileRepository.UpdateProfile(profile);
            response.Content = profile.ProfilePicturePath;
            response.StatusCode = StatusCode.Ok;
            response.Message = "Profile picture updated successfully";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating profile picture");
            response.StatusCode = StatusCode.InternalServerError;
            response.Message = "Error updating profile picture";
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

                ProfilePicturePath = profile.ProfilePicturePath
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

    public async Task<ServiceResponse<ArtistDTO>> GetArtistProfileAsync(int artistId)
    {
        var response = new ServiceResponse<ArtistDTO>();

        try
        {
            var artist = await _artistRepository.GetArtistByIdAsync(artistId);
            if (artist == null)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Message = "Artist profile not found";
                return response;
            }

            // Generate a new SAS token for the artist picture
            var artistPicturePath = await _azureBlobService.GetBlobSasUrl("artist-banner-logos", $"{artist.UserID}.jpg");

            var artistDto = new ArtistDTO
            {
                ArtistName = artist.ArtistName,
                Description = artist.Description,
                Genre = artist.Genre,
                UserID = artist.UserID,
                ArtistPicturePath = artistPicturePath
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
            var profile = await _profileRepository.GetProfileByUserIdAsync((Guid)artistDTO.UserID);
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
                UserID = (Guid)artistDTO.UserID,
                ArtistPicturePath = $"https://{_azureBlobService.AccountName}.blob.core.windows.net/artist-banner-logos/{artistDTO.UserID}.jpg"
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
            // Check if the file is an image
            var validImageTypes = new string[]
            {
        "image/gif",
        "image/jpeg",
        "image/pjpeg",
        "image/png"
            };

            if (bannerLogoPic == null || bannerLogoPic.Length == 0 || !validImageTypes.Contains(bannerLogoPic.ContentType))
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Message = "Invalid file. Please upload an image file.";
                return response;
            }

            // Check if the file size is too large (more than 2MB)
            if (bannerLogoPic.Length > 4 * 1024 * 1024)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Message = "File size is too large. Please upload an image file less than 4MB.";
                return response;
            }

            using var bannerLogoPicStream = new MemoryStream();
            await bannerLogoPic.CopyToAsync(bannerLogoPicStream);
            bannerLogoPicStream.Position = 0;
            await _azureBlobService.UploadFileAsync("artist-banner-logos", $"{userProfileID}.jpg", bannerLogoPicStream);

            response.Content = $"https://{_azureBlobService.AccountName}.blob.core.windows.net/artist-banner-logos/{userProfileID}.jpg";
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


    public async Task<UserProfileDTO> GetUserProfile(Guid userId)
    {
        var profile = await _profileRepository.GetProfileByUserIdAsync(userId);
        if (profile == null)
        {
            throw new Exception("Profile not found");
        }

        var userProfileDto = new UserProfileDTO
        {
            UserID = profile.User.Id,
            UserName = profile.User.UserName,
            Email = profile.User.Email,
            FullName = profile.FullName,
            Bio = profile.Bio,
            ProfilePicturePath = profile.ProfilePicturePath,
            Location = profile.Location,
            WebsiteURL = profile.WebsiteURL,
        };

        if (profile.User.Artist != null)
        {
            userProfileDto.ArtistName = profile.User.Artist.ArtistName;
            userProfileDto.ArtistDescription = profile.User.Artist.Description;
            userProfileDto.ArtistGenre = profile.User.Artist.Genre;
            userProfileDto.ArtistPicturePath = profile.User.Artist.ArtistPicturePath;
        }

        return userProfileDto;
    }


    // PRODUCTION!!! ONLY

    //public async Task<ServiceResponse<string>> UploadArtistBannerLogoAsync(Guid userProfileID, IFormFile bannerLogoPic)
    //{
    //    var response = new ServiceResponse<string>();
    //    try
    //    {
    //        // Check if the artist already has a banner logo picture
    //        var existingBannerLogoPicPath = _azureBlobService.GetBlobSasUrl("artist-banner-logos", $"{userProfileID}.jpg");
    //        if (!string.IsNullOrEmpty(existingBannerLogoPicPath))
    //        {
    //            response.StatusCode = StatusCode.Conflict;
    //            response.Message = "Banner logo picture already exists. Do you want to update it?";
    //            return response;
    //        }

    //        using var bannerLogoPicStream = new MemoryStream();
    //        await bannerLogoPic.CopyToAsync(bannerLogoPicStream);
    //        bannerLogoPicStream.Position = 0;
    //        var bannerLogoPicPath = await _azureBlobService.UploadFileAsync("artist-banner-logos", $"{userProfileID}.jpg", bannerLogoPicStream);

    //        response.Content = bannerLogoPicPath;
    //        response.StatusCode = StatusCode.Created;
    //        response.Message = "Banner logo picture uploaded successfully";
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e, "Error uploading banner logo picture");
    //        response.StatusCode = StatusCode.InternalServerError;
    //        response.Message = "Error uploading banner logo picture";
    //    }
    //    return response;
    //}


}
