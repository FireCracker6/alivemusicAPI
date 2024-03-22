namespace ALIVEMusicAPI.Models.DTOs;

public class CommentLikesDTO
{
    public int LikeID { get; set; }
    public DateTime Timestamp { get; set; }
    public int CommentID { get; set; }
    public Guid UserID { get; set; }
}
