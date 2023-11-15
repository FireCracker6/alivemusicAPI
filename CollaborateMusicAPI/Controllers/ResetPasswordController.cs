using System.Diagnostics;
using CollaborateMusicAPI.Authorization;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Services;
using CollaborateMusicAPI.Services.PasswordReset;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollaborateMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResetPasswordController : ControllerBase
{
   private readonly IPasswordResetService _passwordResetService;
   private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordController(IPasswordResetService passwordResetService, IConfiguration configuration, IUserService userService, UserManager<ApplicationUser> userManager)
    {
        _passwordResetService = passwordResetService;
        _configuration = configuration;
        _userService = userService;
        _userManager = userManager;
    }

    [HttpPost("request-password-reset")]    
    public async Task<IActionResult> RequestPasswordReset(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userResponse = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (userResponse.StatusCode == Enums.StatusCode.NotFound)
            {
                // Optionally log the attempt or take other measures
            }
            else if (userResponse.StatusCode == Enums.StatusCode.Ok)
            {
                var resetPasswordDto = new ResetPasswordDto
                {
                    Email = forgotPasswordDto.Email,
                    ClientURI = GetClientURI()
                };

                var response = await _passwordResetService.RequestPasswordResetAsync(resetPasswordDto);
                return StatusCode((int)response.StatusCode, response);
            }
            else
            {
                // Handle other potential errors
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return Problem();
        }

        // Return a generic response for security
        return Ok("If an account with that email exists, a password reset link has been sent.");
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userResponse = await _userService.GetUserByEmailAsync(model.Email);
        if (userResponse.StatusCode != Enums.StatusCode.Ok)
            return NotFound("User not found.");

        var user = userResponse.Content;
        var resetPassResult = await _userManager.ResetPasswordAsync(user!, model.Token, model.NewPassword);

        if (!resetPassResult.Succeeded)
        {
            // Handle errors
            return BadRequest(resetPassResult.Errors);
        }

        return Ok("Password has been reset successfully.");
    }


private string GetClientURI()
    {
        return _configuration["ClientSettings:ClientURI"]!;
    }
}
