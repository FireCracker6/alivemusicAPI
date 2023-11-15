using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Services.Email;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Web;
using CollaborateMusicAPI.Authentication;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Repositories;

namespace CollaborateMusicAPI.Services.PasswordReset;

public interface IPasswordResetService
{
    Task<ServiceResponse<ResetPasswordDto>> RequestPasswordResetAsync(ResetPasswordDto resetPasswordDto);
   

}


public class PasswordResetService : IPasswordResetService
{
    private readonly IUsersRepository _usersRepository;
    private readonly GenerateTokenService _generateTokenService;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;

    public PasswordResetService(IUsersRepository usersRepository, GenerateTokenService generateTokenService, ITokenService tokenService, UserManager<ApplicationUser> userManager, IEmailService emailService, IUserService userService)
    {
        _usersRepository = usersRepository;
        _generateTokenService = generateTokenService;
        _tokenService = tokenService;
        _userManager = userManager;
        _emailService = emailService;
        _userService = userService;
    }



    public async Task<ServiceResponse<ResetPasswordDto>> RequestPasswordResetAsync(ResetPasswordDto resetPasswordDto)
    {
        var response = new ServiceResponse<ResetPasswordDto>();

        try
        {
            if (resetPasswordDto == null)
            {
                response.StatusCode = Enums.StatusCode.BadRequest;
                response.Message = "Content cannot be null.";
                return response;
            }

            var userResponse = await _userService.GetUserByEmailAsync(resetPasswordDto.Email);
            var user = userResponse.Content;

            if (user == null)
            {
                response.StatusCode = Enums.StatusCode.NotFound;
                response.Message = "User not found.";
                return response;
            }

            // Check if the user is registered via Google or another third-party service
            if (user.SecurityStamp == null) // Replace 'IsGoogleUser' with the appropriate property/flag in your user model
            {
                response.StatusCode = Enums.StatusCode.BadRequest;
                response.Message = "Cannot reset password for 3rd party account. Please use Google to sign in.";
                return response;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            var callbackUrl = $"{resetPasswordDto.ClientURI}/reset-password?email={resetPasswordDto.Email}&token={encodedToken}";

            await _emailService.SendPasswordResetEmailAsync(resetPasswordDto.Email, callbackUrl);

            response.StatusCode = Enums.StatusCode.Ok;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.StatusCode = Enums.StatusCode.InternalServerError;
            response.Message = ex.Message;
        }

        return response;
    }



}
