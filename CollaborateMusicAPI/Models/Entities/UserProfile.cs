using CollaborateMusicAPI.Contexts;
using System.ComponentModel.DataAnnotations;

namespace CollaborateMusicAPI.Models.Entities;

public class UserProfile
{
    [Key]
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string? Location { get; set; }
    public string? WebsiteURL { get; set; }
   
    public virtual ApplicationUser User { get; set; } = null!;
}