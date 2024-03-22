using System.ComponentModel.DataAnnotations;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;

namespace ALIVEMusicAPI.Models.Entities;

public class Likes
{
    [Key]
    public int LikeID { get; set; }
    public DateTime Timestamp { get; set; }

    // Foreign key for Track
    public int TrackID { get; set; }
    // Navigation property for Track
    public virtual Track Track { get; set; }

    // Foreign key for User
    public Guid UserID { get; set; }
    // Navigation property for User
    public virtual ApplicationUser User { get; set; }
}