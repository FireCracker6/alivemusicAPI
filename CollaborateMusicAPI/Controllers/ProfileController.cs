using CollaborateMusicAPI.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollaborateMusicAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDBContext _context;

    public ProfileController(ApplicationDBContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public IActionResult GetProfile(int userId)
    {
        var profile = _context.UserProfiles.FirstOrDefault(p => p.UserProfileID == userId);
        if (profile == null)
        {
            return NotFound();
        }
        return Ok(profile);
    }
}
