namespace ALIVEMusicAPI.Models.DTOs;

public class UserProfileDTO
{
    public Guid UserID { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string FullName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePicturePath { get; set; }
    public IFormFile? ProfilePic { get; set; }
    public string? Location { get; set; }
    public string? WebsiteURL { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureURL { get; set; }

    public string? ArtistName { get; set; }
    public string? ArtistDescription { get; set; }
    public string? ArtistGenre { get; set; }
    public string? ArtistPicturePath { get; set; }


  
}
