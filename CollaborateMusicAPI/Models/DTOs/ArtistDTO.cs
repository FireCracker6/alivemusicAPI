namespace ALIVEMusicAPI.Models.DTOs;

public class ArtistDTO
{
  //  public int ArtistID { get; set; }
    public string? ArtistName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string? ArtistPicturePath { get; set; }
    public IFormFile? ArtistPic { get; set; }
    public Guid? UserID { get; set; }

    public string? ProfilePicturePath { get; set; }
    public string? Location { get; set; }
    public string? WebsiteURL { get; set; }

}
