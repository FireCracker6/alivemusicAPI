using CollaborateMusicAPI.Models.Entities;

namespace CollaborateMusicAPI.Authentication;

public class UserWithTokenResponse
{
    public Users User { get; set; }
    public string Token { get; set; }
}
