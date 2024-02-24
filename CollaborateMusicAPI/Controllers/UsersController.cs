using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Services;
using CollaborateMusicAPI.Services.Email;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static CollaborateMusicAPI.Services.ExternalAuthService;

namespace CollaborateMusicAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IGoogleTokenService _googleTokenService;
    private readonly ApplicationDBContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;


    public UsersController(IUserService userService, IGoogleTokenService googleTokenService, ApplicationDBContext context, ITokenService tokenService, IEmailService emailService)
    {
        _userService = userService;
        _googleTokenService = googleTokenService;
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
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
            if (response.StatusCode == Enums.StatusCode.Created)
            {
                // Await the SendWelcomeEmailAsync method
                await _emailService.SendWelcomeEmailAsync(registrationDto.Email, "Welcome to Alive! Music", registrationDto.Email);


                // Optionally, handle the response from the SendWelcomeEmailAsync method
            }

            return StatusCode((int)response.StatusCode, response);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return Problem();
        }
    }

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

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto loginDto)
    {
        var response = await _userService.LoginAsync(loginDto);

        if (response.Content == null)
        {
            return Unauthorized();  // Return 401 Unauthorized if login fails
        }

        // Assuming response.Content contains the JWT token
        var token = response.Content.JwtToken;

        // Now return the token along with user details
        return Ok(new
        {
            Token = token,
            User = new
            {
                Id = response.Content.Id,
                Email = response.Content.Email,
                FullName = response.Content.Email // Use the actual property for the user's full name
            }
        });
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout(string userId)
    {
      var response = await _userService.LogoutAsync(userId);

        if (response.StatusCode == Enums.StatusCode.Ok)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }   




}
