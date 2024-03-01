namespace ALIVEMusicAPI.Models.DTOs;

public class ArtistDTO
{
    public int ArtistID { get; set; }
    public string ArtistName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string? ArtistPicturePath { get; set; }
    public IFormFile? ArtistPic { get; set; }
    public Guid UserProfileID { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string? Location { get; set; }
    public string? WebsiteURL { get; set; }
    public Guid UserID { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string Duration { get; set; } = null!; // Weekly, Monthly, Yearly
}
