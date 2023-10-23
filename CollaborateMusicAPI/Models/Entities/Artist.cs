namespace CollaborateMusicAPI.Models.Entities;

public class Artist
{
    public int ArtistID { get; set; } 
    public string ArtistName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string? ArtistPicturePath { get; set; }
    public int UserProfileID { get; set; }
    public virtual UserProfile UserProfile { get; set; } = null!;
}
