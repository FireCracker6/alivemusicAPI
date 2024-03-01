using CollaborateMusicAPI.Contexts;

namespace CollaborateMusicAPI.Models.Entities;

public class Artist
{
    public int ArtistID { get; set; } 
    public string ArtistName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string? ArtistPicturePath { get; set; }
    public Guid UserProfileID { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual UserProfile UserProfile { get; set; } = null!;
}
