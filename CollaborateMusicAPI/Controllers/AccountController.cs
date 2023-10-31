using System.Security.Claims;
using CollaborateMusicAPI.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollaborateMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApplicationDBContext _context;

    public AccountController(ApplicationDBContext context)
    {
        _context = context;
    }

    [HttpGet("getuserinfo")]
    public IActionResult GetUserInfo()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        int userId;
        if (!int.TryParse(userIdString, out userId))
        {
            return BadRequest("Invalid user ID format");
        }

        var user = _context.Users.Find(userId);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Email,
        });

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
            fullName = "John Doe"
        });
    }
}
