using System.ComponentModel.DataAnnotations;
using CollaborateMusicAPI.Contexts;

namespace ALIVEMusicAPI.Models.Entities;

public class CommentLikes
{
    [Key]
    public int LikeID { get; set; }
    public DateTime Timestamp { get; set; }

    // Foreign key for Comment
    public int CommentID { get; set; }
    // Navigation property for Comment
    public virtual Comment Comment { get; set; }

    // Foreign key for User
    public Guid UserID { get; set; }
    // Navigation property for User
    public virtual ApplicationUser User { get; set; }

}
