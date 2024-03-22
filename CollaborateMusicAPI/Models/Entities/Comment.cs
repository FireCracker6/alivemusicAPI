using System.ComponentModel.DataAnnotations;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;

namespace ALIVEMusicAPI.Models.Entities;
public class Comment
{
    [Key]
    public int CommentID { get; set; }
    public string Content { get; set; } = null!;
    public DateTime Timestamp { get; set; }

    // Foreign key for User
    public Guid UserID { get; set; }
    // Navigation property for User
    public virtual ApplicationUser User { get; set; } = null!;

    // Foreign key for Track
    public int TrackID { get; set; }
    // Navigation property for Track
    public virtual Track Track { get; set; } = null!;

    // Foreign key for Artist
    public int ArtistID { get; set; }
    // Navigation property for Artist
    public virtual Artist Artist { get; set; } = null!;

    // Foreign key for Parent Comment
    public int? ParentCommentID { get; set; }
    // Navigation property for Parent Comment
    public virtual Comment ParentComment { get; set; }

    // Collection navigation property for Child Comments
    public virtual ICollection<Comment> ChildComments { get; set; } = new List<Comment>();

    public ICollection<CommentClosure> Ancestors { get; set; }
    public ICollection<CommentClosure> Descendants { get; set; }

    public virtual ICollection<CommentLikes> CommentLikes { get; set; }

}
