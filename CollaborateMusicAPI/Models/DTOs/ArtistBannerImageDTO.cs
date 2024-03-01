namespace ALIVEMusicAPI.Models.DTOs;

public class ArtistBannerImageDTO
{
    public Guid UserProfileID { get; set; }
    public IFormFile? BannerPic {  get; set; } 
}
