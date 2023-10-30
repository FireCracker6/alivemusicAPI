using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static CollaborateMusicAPI.Services.ExternalAuthService;

namespace CollaborateMusicAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IGoogleTokenService _googleTokenService;

    public UsersController(IUserService userService, IGoogleTokenService googleTokenService)
    {
        _userService = userService;
        _googleTokenService = googleTokenService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ModelState.Remove("OAuthId");
            ModelState.Remove("OAuthProvider");

            // Await the asynchronous method
            var existingUserResponse = await _userService.GetUserByEmailAsync(registrationDto.Email);
            if (existingUserResponse.Content != null)
            {
                // Changed from BadRequest to Conflict
                return Conflict("Email is already in use.");
            }

            var response = await _userService.CreateAsync(new ServiceRequest<UserRegistrationDto>
            {
                Content = registrationDto
            });

            return StatusCode((int)response.StatusCode, response);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return Problem();
        }
    }

    //[HttpPost("signup-google")]
    //public async Task<IActionResult> AutenticateWithGoogle(TokenRequest request)
    //{
    //    Console.WriteLine($"Received request: {JsonConvert.SerializeObject(request)}");
    //    var code = request.Code;
    //    if (string.IsNullOrWhiteSpace(code))
    //    {
    //        return BadRequest("Authorization code is required.");
    //    }

    //    // Use the service to fetch, verify the token, and get the Google user.
    //    var googleUserResponse = await _googleTokenService.GetGoogleUserFromCodeAsync(request);


    //    if (googleUserResponse.StatusCode != Enums.StatusCode.Ok || googleUserResponse.Content == null)
    //    {
    //     return BadRequest(new { error = "Invalid Google Token." });

    //    }

    //    var googleUser = googleUserResponse.Content;

    //    var existingUserResponse = await _userService.GetUserByEmailAsync(googleUser.Email);
    //    if (existingUserResponse.StatusCode == Enums.StatusCode.Ok && existingUserResponse.Content != null)
    //    {
    //        return Ok(existingUserResponse.Content);
    //    }

    //    // If user does not exist, create a new user.
    //    var newUserResponse = await _googleTokenService.CreateGoogleUserAsync(googleUser);
    //    if (newUserResponse.StatusCode == Enums.StatusCode.Created)
    //    {
    //        return CreatedAtAction(nameof(AutenticateWithGoogle), newUserResponse.Content);
    //    }

    //    return BadRequest("Could not create user!"); // Or some other appropriate response.
    //}
    [HttpPost("signup-google")]
    public IActionResult SignUpGoogle([FromBody] TokenRequest request)
    {
        if (request != null && !string.IsNullOrEmpty(request.Code))
        {
            return Ok("Received the code!");
        }
        return BadRequest("Code not received.");
    }







    [HttpGet("allusers")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return Problem();
        }
    }

}
