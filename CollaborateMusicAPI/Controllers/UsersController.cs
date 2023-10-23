using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using CollaborateMusicAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollaborateMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
    {
        ModelState.Remove("OAuthId");
        ModelState.Remove("OAuthProvider");

        var existingUser = _userService.GetUserByEmail(registrationDto.Email);
        if (existingUser != null)
        {
            return BadRequest("Email is already in use.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Users user = new Users
        {
            Email = registrationDto.Email,
            PasswordHash = registrationDto.Password,
            OAuthProvider = registrationDto.OAuthProvider ?? "default value",
            OAuthId = registrationDto.OAuthId ?? "default value",

        };
        var result = await _userService.CreateAccount(user);
        if (result != null)
        {
            return Ok(result);
        }
        return BadRequest("Failed to create account.");
    }
}
