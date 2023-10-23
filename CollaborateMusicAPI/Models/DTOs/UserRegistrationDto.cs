using System.ComponentModel.DataAnnotations;

namespace CollaborateMusicAPI.Models.DTOs;

public class UserRegistrationDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    public string? OAuthId { get; set; }
    public string? OAuthProvider { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

}
