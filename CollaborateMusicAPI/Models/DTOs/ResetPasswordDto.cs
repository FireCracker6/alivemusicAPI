namespace CollaborateMusicAPI.Models.DTOs;

public class ResetPasswordDto
{
    public string Token { get; set; } = null!;
    public string? NewPassword { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string? ClientURI { get; set; } = null!;
}
