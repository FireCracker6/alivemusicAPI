namespace ALIVEMusicAPI.Models.DTOs;

public class LikesDTO
{
    public int LikeID { get; set; }
    public DateTime Timestamp { get; set; }
    public int TrackID { get; set; }
    public Guid UserID { get; set; }
}
