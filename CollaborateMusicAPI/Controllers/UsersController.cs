using System.Diagnostics;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.DTOs;
using CollaborateMusicAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollaborateMusicAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
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
                return BadRequest("Email is already in use.");
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

    [HttpGet]
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
