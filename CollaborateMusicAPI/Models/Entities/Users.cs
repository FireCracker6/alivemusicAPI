using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CollaborateMusicAPI.Models.Entities;

public class Users
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
   
    public string Email { get; set; }



    [Required(ErrorMessage = "Password is required")]
       public string PasswordHash { get; set; }

    public string? OAuthProvider { get; set; }
    public string? OAuthId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public virtual UserProfile UserProfile { get; set; } = null!;

    public static implicit operator Users(ServiceResponse<Users> v)
    {
        throw new NotImplementedException();
    }
}
