using System.Diagnostics;
using System.Security.Claims;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollaborateMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;
    public AccountController(ApplicationDBContext context, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, IUserService userService)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
        _userService = userService;
    }


    [HttpGet("getuserinfo")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID format");
        }

        var response = await _userService.GetUserInfo(userId);
        if (response.StatusCode == Enums.StatusCode.NotFound)
        {
            return NotFound(response.Message);
        }

        return Ok(response.Data);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest("Email is required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return NotFound("User not found.");

        // Return user data. Add more fields as needed.
        return Ok(new
        {
            user.Id,
            user.Email,
            fullName = user.Email
        });
    }
}
