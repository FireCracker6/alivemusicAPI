using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;

namespace CollaborateMusicAPI.Authentication;

public class UserWithTokenResponse
{
    public ApplicationUser User { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
